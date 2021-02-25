namespace ABV_Invest.Models
{
    using Base;
    using System.ComponentModel.DataAnnotations;

    public class Market : BaseEntity<int>
    {
        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Required]
        [RegularExpression("[A-Z]{4}")]
        public string MIC { get; set; }
    }
}