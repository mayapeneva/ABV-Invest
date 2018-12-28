namespace ABV_Invest.Common
{
    public class Messages
    {
        // General
        public const string WrongDate = "Моля, изберете дата след 01/01/2016г. и не по-късна от {0}г. Не забравяйте да прикачите файлa";

        // BalancesController
        public const string NoBalance =
            "Няма налична информация за баланса Ви към тази дата. Моля, изберете дата след 01/01/2016г. и не по-късна от {0}г.";

        // DataController
        public const string WrongCurrencyData = "Валутният код се състои от 3 главни латински букви.";

        public const string CurrencyExists = "Валута с този код вече съществува или въведените данни не са правилни.";

        public const string WrongMarketData = "Името на пазара е неправилно.";
        public const string MarketExists = "Пазар с това име вече съществува или въведените данни не са правилни.";

        public const string WrongSecurityData =
            "ISIN кодът се състои от 12 цифри или главни латински букви. БФБ кодът се състои от 3 или 4 цифри или главни латински букви.";

        public const string SecurityExists = "Ценни книжа с този ISIN код вече съществуват или въведените данни не са правилни.";

        // DealsController
        public const string NoDeals = "Няма налична информация за Ваши сделки към тази дата. Моля, изберете дата след 01/01/2016г. и не по-късна от {0}г.";

        // NewsController
        public const string NoNews = "В момента няма новини, които да ви покажем.";

        // PortfoliosController
        public const string NoPortfolio = "Няма налична информация за портфолиото Ви към тази дата. Моля, изберете дата след 01/01/2016г. и не по-късна от {0}г.";

        // UploadingController
        public const string CouldNotUploadInformation = "Файлът, който качихте е празен или информацията в него е с неправилен формат.";

        public const string UploadingSuccessfull = "Качването на информация приключи успешно.";
    }
}