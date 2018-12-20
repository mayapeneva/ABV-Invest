namespace ABV_Invest.BindingModels.Uploads.Portfolios
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("AccountData")]
    public class AccountData
    {
        [XmlAttribute("Quantity")]
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Quantity { get; set; }

        [XmlAttribute("OpenPrice")]
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal OpenPrice { get; set; }

        [XmlAttribute("MarketPrice")]
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal MarketPrice { get; set; }

        [XmlAttribute("MarketValue")]
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal MarketValue { get; set; }

        [XmlAttribute("MarketDate")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime MarketDate { get; set; }

        [XmlAttribute("Result")]
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Result { get; set; }

        [XmlAttribute("ResultBGN")]
        [Required]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal ResultBGN { get; set; }
    }
}