namespace ABV_Invest.Common
{
    public class Constants
    {
        // GENERAL
        public const string DateTimeParseFormatLong = "ddd, dd MMM yyyy HH:mm:ss zzz";

        public const string DateTimeParseFormat = "dd/MM/yyyy";
        public const string TextJson = "text/json";
        public const string JsonContentType = "application/json";

        // ROLES
        public const string Admin = "Admin";

        public const string User = "User";

        // AREAS
        public const string Administration = "Administration";

        // CONTROLLERS
        public const string Error = "Error";

        public const string Date = "Date";
        public const string DetailsAction = "Details";
        public const string ChooseDateAction = "ChooseDate";
        public const string AddAction = "Add";
        public const string XmlRootAttr = "WebData";

        // SERVICES
        public const string Buy = "BUY";

        public const string Sell = "SELL";

        // RSS FEED PARSER
        public const string CapitalRSS1 = "https://www.capital.bg/rss/?rubrid=2272";

        public const string CapitalRSS2 = "https://www.capital.bg/rss/?rubrid=3060";
        public const string InvestorRSS = "https://www.investor.bg/news/rss/last/10/";
        public const string X3NewsRSS = "http://www.x3news.com/?page=RSSFeed";

        // IDENTITY
        public const string ChangePassword = "./ChangePassword";

        public const string ConfirmEmail = "/Account/ConfirmEmail";
        public const string ForgotPassword = "./ForgotPasswordConfirmation";
        public const string Index = "/Index";
        public const string Lockout = "./Lockout";
        public const string PasswordChange = "Промяна на парола";
        public const string PINRegex = "^\\d{5}$";
        public const string ResetPassword = "/Account/ResetPassword";
        public const string ResetPasswordConfirmation = "./ResetPasswordConfirmation";
        public const string SetPassword = "./SetPassword";
        public const string UserNameRegex = "^[A-Z0-9]{5}$|^[A-Z0-9]{10}$";
    }
}