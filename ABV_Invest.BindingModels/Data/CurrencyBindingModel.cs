namespace ABV_Invest.BindingModels.Data
{
    using System.ComponentModel.DataAnnotations;
    using Common;

    public class CurrencyBindingModel
    {
        [Required]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = Messages.CurrencyCodeError)]
        public string Code { get; set; }
    }
}