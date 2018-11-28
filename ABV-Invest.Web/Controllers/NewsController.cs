namespace ABV_Invest.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels;

    public class NewsController : Controller
    {
        private const string CapitalRSS1 = "https://www.capital.bg/rss/?rubrid=2272";
        private const string CapitalRSS2 = "https://www.capital.bg/rss/?rubrid=3060";
        private const string InvestorRSS = "https://www.investor.bg/news/rss/last/10/";
        private const string X3NewsRSS = "http://www.x3news.com/?page=RSSFeed";

        public IActionResult Index()
        {
            var rssModels = new List<RSSFeedViewModel>();
            LoadNewsFromInvestor(rssModels);

            LoadNewsFromCapital(rssModels, CapitalRSS1);
            LoadNewsFromCapital(rssModels, CapitalRSS2);

            LoadNewsFromX3News(rssModels);

            return this.View(rssModels.OrderByDescending(m => m.PublishedDate));
        }

        private static void LoadNewsFromInvestor(List<RSSFeedViewModel> rssModels)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(InvestorRSS);
            var feeds = xmlDoc.DocumentElement.FirstChild.ChildNodes;

            foreach (XmlNode feed in feeds)
            {
                if (feed.Name == "item")
                {
                    rssModels.Add(new RSSFeedViewModel
                    {
                        Title = feed["title"].InnerText,
                        Uri = feed["link"].InnerText,
                        PublishedDate = DateTime.Parse(feed["pubDate"].InnerText),
                        Summary = feed["description"].InnerText
                    });
                }
            }
        }

        private static void LoadNewsFromCapital(List<RSSFeedViewModel> rssModels, string capitalRss)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(capitalRss);
            var feeds = xmlDoc.DocumentElement.FirstChild.ChildNodes;

            foreach (XmlNode feed in feeds)
            {
                if (feed.Name == "item")
                {
                    var summaryRaw = feed["description"].InnerText;
                    var startingIndex = summaryRaw.IndexOf("<br />", StringComparison.InvariantCulture) + " <br />".Length - 1;
                    var lenght = summaryRaw.IndexOf(". ", StringComparison.InvariantCulture);

                    if (lenght == -1)
                    {
                        lenght = summaryRaw.IndexOf(".<", StringComparison.InvariantCulture);
                    }

                    var summary = summaryRaw.Substring(startingIndex, lenght - startingIndex);
                    summary = summary.Replace("<br /><br />", " ");
                    rssModels.Add(new RSSFeedViewModel
                    {
                        Title = feed["title"].InnerText,
                        Uri = feed["link"].InnerText,
                        PublishedDate = DateTime.Parse(feed["pubDate"].InnerText),
                        Summary = summary
                    });
                }
            }
        }

        private static void LoadNewsFromX3News(List<RSSFeedViewModel> rssModels)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("windows-1254");

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(X3NewsRSS);
            var feeds = xmlDoc.DocumentElement.FirstChild.ChildNodes;

            foreach (XmlNode feed in feeds)
            {
                if (feed.Name == "item")
                {
                    var model = new RSSFeedViewModel
                    {
                        Title = feed["title"].InnerText,
                        Uri = feed["link"].InnerText,
                        Summary = feed["description"].InnerText
                    };

                    var ifParsed = DateTime.TryParseExact(feed["pubDate"].InnerText, "dd/MM/yyyy", CultureInfo.GetCultureInfo("bg-BG"), DateTimeStyles.AssumeLocal, out DateTime pubDate);
                    if (ifParsed)
                    {
                        model.PublishedDate = pubDate;
                    }

                    rssModels.Add(model);
                }
            }
        }
    }
}