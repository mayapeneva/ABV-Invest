﻿namespace ABV_Invest.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Common;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels;

    public class NewsController : Controller
    {
        public IActionResult Index()
        {
            var rssModels = new List<RSSFeedViewModel>();
            LoadNewsFromInvestor(rssModels);

            LoadNewsFromCapital(rssModels, Constants.CapitalRSS1);
            LoadNewsFromCapital(rssModels, Constants.CapitalRSS2);

            LoadNewsFromX3News(rssModels);

            return this.View(rssModels.OrderByDescending(m => m.PublishedDate));
        }

        private static void LoadNewsFromInvestor(List<RSSFeedViewModel> rssModels)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Constants.InvestorRSS);
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
            xmlDoc.Load(Constants.X3NewsRSS);
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

                    var ifParsed = DateTime.TryParseExact(feed["pubDate"].InnerText, Constants.DateTimeParseFormat, CultureInfo.GetCultureInfo("bg-BG"), DateTimeStyles.AssumeLocal, out DateTime pubDate);
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