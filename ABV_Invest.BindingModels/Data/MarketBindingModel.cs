namespace ABV_Invest.BindingModels.Data
{
    using System.ComponentModel.DataAnnotations;

    public class MarketBindingModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}