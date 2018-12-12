namespace ABV_Invest.Services.Contracts
{
    using System.Collections.Generic;
    using ViewModels;

    public interface INewsService
    {
        void LoadNewsFromInvestor(List<RSSFeedViewModel> rssModels);

        void LoadNewsFromCapital(List<RSSFeedViewModel> rssModels, string capitalRss);

        void LoadNewsFromX3News(List<RSSFeedViewModel> rssModels);
    }
}