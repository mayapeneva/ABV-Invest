namespace ABV_Invest.BindingModels.Data
{
    using System.ComponentModel.DataAnnotations;

    public class CurrencyBindingModel
    {
        [Required]
        [RegularExpression("^[A-Z]{3}$")]
        public string Code { get; set; }
    }
}