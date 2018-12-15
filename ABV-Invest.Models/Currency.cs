namespace ABV_Invest.Models
{
    using System.ComponentModel.DataAnnotations;
    using Base;

    public class Currency : BaseEntity<int>
    {
        [Required]
        [RegularExpression("^[A-Z]{3}$")]
        public string Code { get; set; }
    }
}