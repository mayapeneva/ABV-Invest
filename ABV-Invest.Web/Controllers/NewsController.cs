namespace ABV_Invest.Web.Controllers
{
    using Common;
    using Extensions.Contracts;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Linq;
    using ViewModels;

    public class NewsController : Controller
    {
        private readonly IRSSFeedParser rssFeedParser;

        public NewsController(IRSSFeedParser rssFeedParser)
        {
            this.rssFeedParser = rssFeedParser;
        }

        public IActionResult Load()
        {
            var rssModels = new List<RSSFeedViewModel>();
            this.rssFeedParser.LoadNewsFromInvestor(rssModels);

            this.rssFeedParser.LoadNewsFromCapital(rssModels, Constants.CapitalRSS1);
            this.rssFeedParser.LoadNewsFromCapital(rssModels, Constants.CapitalRSS2);

            // this.rssFeedParser.LoadNewsFromX3News(rssModels);
            if (!rssModels.Any())
            {
                this.ViewData[Constants.Error] = Messages.NoNews;
                return this.View();
            }

            return this.View(rssModels.OrderByDescending(m => m.PublishedDate));
        }
    }
}