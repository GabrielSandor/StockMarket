using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using Nancy;
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

            Get("/stockExchange", _ => "Hello from StockExchangeModule!");

            Post("/stockExchange", parameters =>
            {
                try
                {
                    var contentTypeRegex = new Regex("^multipart/form-data;\\s*boundary=(.*)$", RegexOptions.IgnoreCase);
                    Stream bodyStream;

                    if (contentTypeRegex.IsMatch(Request.Headers.ContentType))
                    {
                        var boundary = contentTypeRegex.Match(Request.Headers.ContentType).Groups[1].Value;
                        var multipart = new HttpMultipart(Request.Body, boundary);
                        bodyStream = multipart.GetBoundaries().First().Value;
                    }
                    else
                    {
                        // Regular model binding goes here.
                        bodyStream = Request.Body;
                    }

                    using (var archive = new ZipArchive(bodyStream))
                    {
                        foreach (var entry in archive.Entries)
                        {
                            using (var entryStream = entry.Open())
                            {
                                var rawTicks = DecodeQrCode(entryStream);

                                var ticks = rawTicks.Split(' ').Select(double.Parse).ToArray();

                                var result = _bestTradesFinder.Find(ticks);

                                if (result.Success)
                                {

                                }
                            }
                        }
                    }

                    return HttpStatusCode.OK;
                }
                catch (Exception ex)
                {
                    return HttpStatusCode.InternalServerError;
                }
            });
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
    }
}
