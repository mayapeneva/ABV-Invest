namespace ABV_Invest.BindingModels.Uploads.Portfolios
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Other")]
    public class Other
    {
        [XmlElement("YieldPercent")]
        [Required]
        public string YieldPercent { get; set; }

        [XmlElement("RelativePart")]
        [Required]
        public string RelativePart { get; set; }
    }
}