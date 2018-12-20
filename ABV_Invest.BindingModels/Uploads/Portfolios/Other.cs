namespace ABV_Invest.BindingModels.Uploads.Portfolios
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Other")]
    public class Other
    {
        [XmlAttribute("YieldPercent")]
        [Required]
        [Range(0, 79228162514264337593543950.000)]
        public decimal YieldPercent { get; set; }

        [XmlAttribute("RelativePart")]
        [Required]
        [Range(0, 79228162514264337593543950.000)]
        public decimal RelativePart { get; set; }
    }
}