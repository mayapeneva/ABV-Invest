namespace ABV_Invest.BindingModels.Data
{
    using System.ComponentModel.DataAnnotations;

    public class SecurityBindingModel
    {
        [Required]
        [DataType(DataType.Text)]
        public string Issuer { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z0-9]{12}$", ErrorMessage = "ISIN кодът трябва да е дълъг 12 символа и да съдържа главни латински букви и цифри.")]
        public string ISIN { get; set; }

        [RegularExpression(@"^[A-Z0-9]{3,4}$", ErrorMessage = "БФБ кодът трябва да е дълъг 3 или 4 символа и да съдържа цифри и/или главни латински букви.")]
        public string BfbCode { get; set; }

        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Кодът на валутата трябва да е дълъг 3 символа и да съдържа само главни латински букви.")]
        public string Currency { get; set; }
    }
}