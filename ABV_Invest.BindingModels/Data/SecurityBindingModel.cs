namespace ABV_Invest.BindingModels.Data
{
    using System.ComponentModel.DataAnnotations;

    public class SecurityBindingModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Issuer { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z0-9]{12}$")]
        public string ISIN { get; set; }

        [RegularExpression(@"^[A-Z0-9]{3,4}$")]
        public string BfbCode { get; set; }

        public string Currency { get; set; }
    }
}