namespace ABV_Invest.BindingModels.Uploads.Portfolios
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("AccountData")]
    public class AccountData
    {
        [XmlElement("Quantity")]
        [Required]
        public string Quantity { get; set; }

        [XmlElement("OpenPrice")]
        [Required]
        public string OpenPrice { get; set; }

        [XmlElement("MarketPrice")]
        [Required]
        public string MarketPrice { get; set; }

        [XmlElement("MarketValue")]
        [Required]
        public string MarketValue { get; set; }

        [XmlElement("MarketDate")]
        [Required]
        public string MarketDate { get; set; }

        [XmlElement("Result")]
        [Required]
        public string Result { get; set; }

        [XmlElement("ResultBGN")]
        [Required]
        public string ResultBGN { get; set; }
    }
}