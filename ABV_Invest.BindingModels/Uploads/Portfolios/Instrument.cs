namespace ABV_Invest.BindingModels.Uploads.Portfolios
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Instrument")]
    public class Instrument
    {
        [XmlAttribute("ISIN")]
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]{12}$")]
        public string ISIN { get; set; }

        [XmlAttribute("NewCode")]
        [RegularExpression(@"^[A-Z0-9]{3,4}$")]
        public string NewCode { get; set; }

        [XmlAttribute("Issuer")]
        [Required]
        [DataType(DataType.Text)]
        public string Issuer { get; set; }

        [XmlAttribute("Currency")]
        [Required]
        [RegularExpression(@"^[A-Z]{3}$")]
        public string Currency { get; set; }

        [XmlAttribute("MaturityDate")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime MaturityDate { get; set; }
    }
}