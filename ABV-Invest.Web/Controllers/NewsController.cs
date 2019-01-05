namespace ABV_Invest.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Extensions;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels;

    public class NewsController : Controller
    {
        private readonly RSSFeedParser rssFeedParser;

        public NewsController(RSSFeedParser rssFeedParser)
        {
            this.rssFeedParser = rssFeedParser;
        }

        public IActionResult Load()
        {
            var rssModels = new List<RSSFeedViewModel>();

            this.rssFeedParser.LoadNewsFromInvestor(rssModels);

            this.rssFeedParser.LoadNewsFromCapital(rssModels, Constants.CapitalRSS1);
            this.rssFeedParser.LoadNewsFromCapital(rssModels, Constants.CapitalRSS2);

            this.rssFeedParser.LoadNewsFromX3News(rssModels);
            if (!rssModels.Any())
            {
                this.ViewData["Error"] = Messages.NoNews;
                return this.View();
            }

            return this.View(rssModels.OrderByDescending(m => m.PublishedDate));
        }
    }
}