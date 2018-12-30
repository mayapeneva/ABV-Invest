namespace ABV_Invest.BindingModels.Data
{
    using System.ComponentModel.DataAnnotations;

    public class MarketBindingModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        [RegularExpression("[A-Z]{4}")]
        public string MIC { get; set; }
    }
}