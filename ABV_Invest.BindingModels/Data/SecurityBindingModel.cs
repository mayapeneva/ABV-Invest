namespace ABV_Invest.BindingModels.Data
{
    using Common;
    using System.ComponentModel.DataAnnotations;

    public class SecurityBindingModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Issuer { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z0-9]{12}$", ErrorMessage = Messages.IsinError)]
        public string ISIN { get; set; }

        [RegularExpression(@"^[A-Z0-9]{3,4}$", ErrorMessage = Messages.BfbCodeError)]
        public string BfbCode { get; set; }

        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = Messages.CurrencyCodeError)]
        public string Currency { get; set; }
    }
}