namespace ABV_Invest.BindingModels.Uploads.Portfolios
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Client")]
    public class Client
    {
        [XmlAttribute("CDNumber")]
        [Required]
        [RegularExpression(@"^[A-Z0-9]{5}$|^[A-Z0-9]{10}$")]
        public string CDNNumber { get; set; }

        [XmlAttribute("Name")]
        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}