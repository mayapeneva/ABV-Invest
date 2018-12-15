namespace ABV_Invest.BindingModels.Uploads.Portfolios
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    [DisplayName("New")]
    public class PortfolioRowBindingModel
    {
        [Required]
        public Client Client { get; set; }

        [Required]
        public Instrument Instrument { get; set; }

        [Required]
        public AccountData AccountData { get; set; }

        [Required]
        public Other Other { get; set; }
    }
}