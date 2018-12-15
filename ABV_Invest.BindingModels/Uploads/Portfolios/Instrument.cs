namespace ABV_Invest.BindingModels.Uploads.Portfolios
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Instrument
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]{12}$")]
        public string ISIN { get; set; }

        [RegularExpression(@"^[A-Z0-9]{3,4}$")]
        public string NewCode { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Issuer { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z]{3}$")]
        public string Currency { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime MaturityDate { get; set; }
    }
}