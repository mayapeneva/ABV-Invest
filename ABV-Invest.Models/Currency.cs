namespace ABV_Invest.Models
{
    using Base;
    using System.ComponentModel.DataAnnotations;

    public class Currency : BaseEntity<int>
    {
        [Required]
        [RegularExpression("^[A-Z]{3}$")]
        public string Code { get; set; }
    }
}