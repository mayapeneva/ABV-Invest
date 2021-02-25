namespace ABV_Invest.Common.BindingModels.Data
{
    using Common;
    using System.ComponentModel.DataAnnotations;

    public class CurrencyBindingModel
    {
        [Required]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = Messages.CurrencyCodeError)]
        public string Code { get; set; }
    }
}