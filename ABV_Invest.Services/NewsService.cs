namespace ABV_Invest.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Xml;
    using Common;
    using Contracts;
    using ViewModels;

    public class NewsService : INewsService
    {
        public void LoadNewsFromInvestor(List<RSSFeedViewModel> rssModels)
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

        public void LoadNewsFromCapital(List<RSSFeedViewModel> rssModels, string capitalRss)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(capitalRss);
            var feeds = xmlDoc.DocumentElement.FirstChild.ChildNodes;

            foreach (XmlNode feed in feeds)
            {
                if (feed.Name == "item")
                {
                    var summaryRaw = feed["description"].InnerText;

                    // Clean the feedDescription from html tags, which should not be part of it
                    var startingIndex = summaryRaw.IndexOf(" /><br />", StringComparison.InvariantCulture) + " /><br />".Length;
                    var lenght = summaryRaw.LastIndexOf("<br />", StringComparison.InvariantCulture);

                    if (lenght == -1)
                    {
                        lenght = summaryRaw.LastIndexOf(".", StringComparison.InvariantCulture);
                    }

                    if (lenght == -1 || lenght < startingIndex)
                    {
                        lenght = summaryRaw.Length - 1;
                    }

                    var subSummary = summaryRaw.Substring(startingIndex, lenght - startingIndex);

                    // Replace the unnecessary symbols and shorten the description length
                    var summary = subSummary.Replace("<br />", " ");
                    if (summary.Length > 150)
                    {
                        summary = summary.Substring(0, 150) + "...";
                    }

                    // Create the RSSModel
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

        public void LoadNewsFromX3News(List<RSSFeedViewModel> rssModels)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("windows-1251");

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