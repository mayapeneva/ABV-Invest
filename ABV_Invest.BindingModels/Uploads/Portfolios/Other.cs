namespace ABV_Invest.BindingModels.Uploads.Portfolios
{
    using System.ComponentModel.DataAnnotations;

    public class Other
    {
        [Required]
        [Range(0, 79228162514264337593543950.000)]
        public decimal YieldPercent { get; set; }

        [Required]
        [Range(0, 79228162514264337593543950.000)]
        public decimal RelativePart { get; set; }
    }
}