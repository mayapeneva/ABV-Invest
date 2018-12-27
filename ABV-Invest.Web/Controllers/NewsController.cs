namespace ABV_Invest.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Common;
    using Microsoft.AspNetCore.Mvc;
    using Services.Contracts;
    using ViewModels;

    public class NewsController : Controller
    {
        private readonly INewsService newsService;

        public NewsController(INewsService newsService)
        {
            this.newsService = newsService;
        }

        public IActionResult Load()
        {
            var rssModels = new List<RSSFeedViewModel>();

            this.newsService.LoadNewsFromInvestor(rssModels);

            this.newsService.LoadNewsFromCapital(rssModels, Constants.CapitalRSS1);
            this.newsService.LoadNewsFromCapital(rssModels, Constants.CapitalRSS2);

            this.newsService.LoadNewsFromX3News(rssModels);
            if (!rssModels.Any())
            {
                this.ViewData["Error"] = Messages.NoNews;
                return this.View();
            }

            return this.View(rssModels.OrderByDescending(m => m.PublishedDate));
        }
    }
}