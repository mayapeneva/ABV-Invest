namespace ABV_Invest.Web.Extensions
{
    using Common;
    using Contracts;
    using Enums;
    using ViewModels;

    using System;
    using System.Collections.Generic;
    using System.Xml;

    public class RSSFeedParser : IRSSFeedParser
    {
        private const string Item = "item";
        private const string Title = "title";
        private const string Link = "link";
        private const string PubDate = "pubDate";
        private const string Description = "description";

        private readonly DateTime twoWeeksBackDate = DateTime.UtcNow.Subtract(new TimeSpan(14, 0, 0, 0));

        public void LoadNewsFromInvestor(List<RSSFeedViewModel> rssModels)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Constants.InvestorRSS);
            var feeds = xmlDoc.DocumentElement.FirstChild.ChildNodes;

            foreach (XmlNode feed in feeds)
            {
                if (feed.Name == Item)
                {
                    var publishDate = DateTime.Parse(feed[PubDate].InnerText);
                    if (publishDate > this.twoWeeksBackDate)
                    {
                        rssModels.Add(new RSSFeedViewModel
                        {
                            Title = feed[Title].InnerText,
                            Uri = feed[Link].InnerText,
                            PublishedDate = publishDate,
                            Summary = feed[Description].InnerText
                        });
                    }
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
                if (feed.Name == Item)
                {
                    var publishDate = DateTime.Parse(feed[PubDate].InnerText);
                    if (publishDate > this.twoWeeksBackDate)
                    {
                        var summaryRaw = feed[Description].InnerText;

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
                            Title = feed[Title].InnerText,
                            Uri = feed[Link].InnerText,
                            PublishedDate = publishDate,
                            Summary = summary
                        });
                    }
                }
            }
        }

        public void LoadNewsFromX3News(List<RSSFeedViewModel> rssModels)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Constants.X3NewsRSS);
            if (xmlDoc.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
            {
                XmlDeclaration dec = (XmlDeclaration)xmlDoc.FirstChild;
                dec.Encoding = "windows-1252";
            }
            else
            {
                var xmlDecl = xmlDoc.CreateXmlDeclaration("1.0", null, null);
                xmlDecl.Encoding = "windows-1252";
                xmlDoc.InsertBefore(xmlDecl, xmlDoc.DocumentElement);
            }

            var feeds = xmlDoc.DocumentElement.FirstChild.ChildNodes;
            foreach (XmlNode feed in feeds)
            {
                if (feed.Name == Item)
                {
                    var model = new RSSFeedViewModel
                    {
                        Title = feed[Title].InnerText,
                        Uri = feed[Link].InnerText,
                        Summary = feed[Description].InnerText
                    };

                    var notParsedDate = feed[PubDate].InnerText.ToLower().Replace(" ", "");
                    var dayStartIndex = notParsedDate.IndexOf(",", StringComparison.InvariantCulture) + 1;
                    var ifDayParsed = int.TryParse(notParsedDate.Substring(dayStartIndex, 2), out int day);

                    var monthStartIndex = dayStartIndex + 2;
                    var monthString = notParsedDate.Substring(monthStartIndex, 3).Trim('.');
                    var ifMonthParsed = Enum.TryParse(typeof(Month), monthString, out var monthEnum);

                    var yearStartIndex = notParsedDate.IndexOf(".", StringComparison.InvariantCulture) + 1;
                    var ifYearParsed = int.TryParse(notParsedDate.Substring(yearStartIndex, 4), out int year);

                    if (ifDayParsed && ifMonthParsed && ifYearParsed)
                    {
                        var month = (Month)monthEnum;
                        model.PublishedDate = new DateTime(year, (int)month, day);
                    }

                    rssModels.Add(model);
                }
            }
        }
    }
}