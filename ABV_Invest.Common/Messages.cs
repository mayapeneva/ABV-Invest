namespace ABV_Invest.Common
{
    public class Messages
    {
        // General
        public const string WrongDate = "Моля, изберете дата след 01/01/2016г. и не по-късна от {0}г. Не забравяйте да прикачите файлa";

        // BalancesController
        public const string NoBalance = "Няма налична информация за баланса Ви към тази дата. Моля, изберете дата след 01/01/2016г. и не по-късна от {0}г.";

        // DataController
        public const string WrongCurrencyData = "Валутният код се състои от 3 главни латински букви.";

        public const string CurrencyExists = "Валута с този код вече съществува или въведените данни не са правилни.";

        public const string WrongMarketData = "Името на пазара е неправилно.";

        public const string MarketExists = "Пазар с това име вече съществува или въведените данни не са правилни.";

        public const string WrongSecurityData = "ISIN кодът се състои от 12 цифри или главни латински букви. БФБ кодът се състои от 3 или 4 цифри или главни латински букви.";

        public const string SecurityExists = "Ценни книжа с този ISIN код вече съществуват или въведените данни не са правилни.";

        // DealsController
        public const string NoDeals = "Няма налична информация за Ваши сделки към тази дата. Моля, изберете дата след 01/01/2016г. и не по-късна от {0}г.";

        // NewsController
        public const string NoNews = "В момента няма новини, които да ви покажем.";

        // PortfoliosController
        public const string NoPortfolio = "Няма налична информация за портфолиото Ви към тази дата. Моля, изберете дата след 01/01/2016г. и не по-късна от {0}г.";

        // Identity
        public const string CantLoadUser = "Не беше възможно да заредим данните на потребител с ID '{0}'.";

        public const string MistakeWhenSaving = "Възникна грешка при запазването на {0} за потребител с ID '{1}'.";

        public const string ProfileChanged = "Профилът ви беше успешно променен.";

        public const string MistakeWhenDeleting = "Възникна грешка при изтриването на потребител с ID '{0}'.";

        public const string InvalidLogInAttempt = "Неуспешен опит за влизане в сайта.";
        public const string LockedAccount = "Регистрацията на потребителя е блокирана.";
        public const string UserLoggedIn = "Потребителят вече е влязъл в сайта.";

        public const string UserLoggedOut = "Потребителят е излязъл от сайта.";
        public const string UserExists = "Това потребителско име е вече заето.";

        // PortfoliosService and DealsServie
        public const string CouldNotUploadInformation = "Файлът, който качихте е празен или информацията в него е с неправилен формат.";

        public const string CurrencyCannotBeCreated = "- Валута с код {0} за клиент с потребителско име {1} не може да бъдат регистрирана поради неправилен код. Валутата е за ценни книжа с Емитент: {2}, ISIN: {3}, БФБ код {4}.";

        public const string DailyDealsAlredyExist = "- Сделките за клиент с потребителско име {0} за дата {1} вече са регистрирани.";

        public const string DailyDealsCannotBeCreated = "- Сделките за клиент с потребителско име {0} не могат да бъдат регистрирани за дата {1} поради неправилни данни.";

        public const string DealCannotBeRegistered = "Сделка {0} с ценни книжа с ISIN: {1} за клиент с потребителско име {2} не може да бъде регистрирана поради неправилни данни: {3}: {4}.";

        public const string DealRowCannotBeCreated = "- Сделка {0} с ценни книжа с ISIN: {1} за клиент с потребителско име {2} не може да бъде регистрирана поради неправилни данни: Количество: {3}, Единична цена: {4}.";

        public const string MarketDoesNotExist = "- Сделка {0} с ценни книжа с ISIN: {1} за клиент с потребителско име {2} не може да бъде регистрирана поради неправилни данни: MIC код на пазара: {3}.";

        public const string PortfolioCannotBeCreated = "- Портфолио за клиент с потребителско име {0} не може да бъдат създадено за дата {1} поради неправилни данни.";

        public const string SecurityCannotBeCreated = "- Ценни книжа за клиент с потребителско име {0} не могат да бъдат регистрирани поради неправилни данни: Емитент: {1}, ISIN: {2}, БФБ код {3}, Валута: {4}.";

        public const string SecurityCannotBeRegistered = "- Ценни книжа за клиент с потребителско име {0} не могат да бъдат регистрирани поради неправилни данни: {1}: {2}.";

        public const string SecurityExistsInThisPortfolio = "- Ценни книжа за клиент с потребителско име {0} не могат да бъдат регистрирани повторно за дата {1}: Емитент: {2}, ISIN: {3}, БФБ код {4}, Валута: {5}.";

        public const string UserDoesNotExist = "- Клиент с потребителско име {0} не е регистриран и всички записи за този клиент са пропуснати.";

        public const string UploadingSuccessfull = "Качването на информация приключи успешно.";
    }
}