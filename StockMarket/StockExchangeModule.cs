using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nancy;
using Newtonsoft.Json.Linq;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace StockMarket
{
    public class StockExchangeModule : NancyModule
    {
        private readonly BestTradesFinder _bestTradesFinder;

        public StockExchangeModule()
        {
            _bestTradesFinder = new BestTradesFinder();

            Get("/stockExchange", _ => "");

            Post("/stockExchange", parameters =>
            {
                try
                {
                    var requestBodyStream = ExtractBodyStream();

                    var inputStreams = ExtractArchive(requestBodyStream);

                    var response = ComputeResponse(inputStreams);

                    var jsonBytes = BuildJsonResponse(response);
                    return new Response
                    {
                        StatusCode = HttpStatusCode.OK,
                        ContentType = "application/json",
                        Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
                    };
                }
                catch
                {
                    return HttpStatusCode.InternalServerError;
                }
            });
        }

        private static byte[] BuildJsonResponse(IEnumerable<BuyLowSellHigh> response)
        {
            var jobj = new JObject();
            foreach (var buyLowSellHigh in response)
            {
                jobj.Add(new JProperty(buyLowSellHigh.FileName,
                    new JObject(
                        new JProperty("buyPoint", buyLowSellHigh.Buy.ToString()),
                        new JProperty("sellPoint", buyLowSellHigh.Sell.ToString())
                    )));
            }

            var jsonBytes = Encoding.UTF8.GetBytes(jobj.ToString());
            return jsonBytes;
        }

        private IEnumerable<BuyLowSellHigh> ComputeResponse(List<InputStream> inputStreams)
        {
            var response = new ConcurrentBag<BuyLowSellHigh>();

            Parallel.ForEach(inputStreams, inputStream =>
            {
                try
                {
                    var rawTicks = DecodeQrCode(inputStream.MemoryStream);

                    var ticks = rawTicks.Split(' ').Select(double.Parse).ToArray();

                    var result = _bestTradesFinder.Find(ticks);

                    if (result.Success)
                    {
                        response.Add(new BuyLowSellHigh
                        {
                            FileName = inputStream.FileName,
                            Buy = result.Buy,
                            Sell = result.Sell
                        });
                    }
                }
                catch
                {
                }
            });
            return response;
        }

        private static List<InputStream> ExtractArchive(Stream bodyStream)
        {
            var inputStreams = new List<InputStream>();

            using (var archive = new ZipArchive(bodyStream))
            {
                foreach (var entry in archive.Entries)
                {
                    using (var entryStream = entry.Open())
                    {
                        var memoryStream = new MemoryStream();
                        entryStream.CopyTo(memoryStream);

                        inputStreams.Add(new InputStream
                        {
                            FileName = entry.Name,
                            MemoryStream = memoryStream
                        });
                    }
                }
            }

            return inputStreams;
        }

        private Stream ExtractBodyStream()
        {
            var contentTypeRegex = new Regex("^multipart/form-data;\\s*boundary=(.*)$", RegexOptions.IgnoreCase);
            Stream bodyStream = null;

            if (contentTypeRegex.IsMatch(Request.Headers.ContentType))
            {
                var boundary = contentTypeRegex.Match(Request.Headers.ContentType).Groups[1].Value;
                var multipart = new HttpMultipart(Request.Body, boundary);
                bodyStream = multipart.GetBoundaries().First().Value;
            }

            return bodyStream;
        }

        private static string DecodeQrCode(Stream entryStream)
        {
            var qrBitmap = new Bitmap(entryStream);

            var source = new BitmapLuminanceSource(qrBitmap);
            var bitmap = new BinaryBitmap(new HybridBinarizer(source));

            var reader = new QRCodeReader();
            var result = reader.decode(bitmap);

            return result.Text;
        }

        private class BuyLowSellHigh
        {
            public string FileName { get; set; }
            public double Buy { get; set; }
            public double Sell { get; set; }
        }

        private class InputStream
        {
            public string FileName { get; set; }
            public MemoryStream MemoryStream { get; set; }
        }
    }
}
