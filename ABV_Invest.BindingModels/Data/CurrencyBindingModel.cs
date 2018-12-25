namespace ABV_Invest.BindingModels.Data
{
    using System.ComponentModel.DataAnnotations;

    public class CurrencyBindingModel
    {
        [Required]
        [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Кодът на валутата трябва да е дълъг 3 символа и да съдържа само главни латински букви.")]
        public string Code { get; set; }
    }
}