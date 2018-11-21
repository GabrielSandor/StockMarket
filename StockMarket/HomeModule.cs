using Nancy;

namespace StockMarket
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get("/", _ => "");
        }
    }
}
