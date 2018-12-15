namespace ABV_Invest.Models
{
    using System.ComponentModel.DataAnnotations;
    using Base;

    public class Market : BaseEntity<int>
    {
        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        public Currency Currency { get; set; }
        public int CurrencyId { get; set; }
    }
}