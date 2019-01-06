namespace ABV_Invest.Web.Extensions.Contracts
{
    using ViewModels;

    using System.Collections.Generic;

    public interface IRSSFeedParser
    {
        void LoadNewsFromInvestor(List<RSSFeedViewModel> rssModels);

        void LoadNewsFromCapital(List<RSSFeedViewModel> rssModels, string capitalRss);

        void LoadNewsFromX3News(List<RSSFeedViewModel> rssModels);
    }
}