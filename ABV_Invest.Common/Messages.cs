namespace ABV_Invest.Common
{
    public class Messages
    {
        // General
        public const string WrongDate = "Моля, изберете дата след 01/01/2016г. и не по-късна от {0}г. Не забравяйте да прикачите файлa";

        // BalancesController
        public const string NoBalance = "Няма налична информация за баланса Ви към тази дата. Моля, изберете дата след 01/01/2016г. и не по-късна от {0}г.";

        // DealsController
        public const string NoDeals = "Няма налична информация за Ваши сделки към тази дата. Моля, изберете дата след 01/01/2016г. и не по-късна от {0}г.";

        // PortfoliosController
        public const string NoPortfolio = "Няма налична информация за портфолиото Ви към тази дата. Моля, изберете дата след 01/01/2016г. и не по-късна от {0}г.";

        // UploadingController
        public const string CouldNotUploadInformation = "Файлът, който качихте е празен или е с неправилен формат.";

        public const string UploadingSuccessfull = "Качването на информация приключи успешно.";
    }
}