namespace ABV_Invest.BindingModels.Uploads.Portfolios
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AccountData
    {
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Quantity { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal OpenPrice { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal MarketPrice { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal MarketValue { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime MarketDate { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Result { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal ResultBGN { get; set; }
    }
}