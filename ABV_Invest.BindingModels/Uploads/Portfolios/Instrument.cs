﻿namespace ABV_Invest.BindingModels.Uploads.Portfolios
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("Instrument")]
    public class Instrument
    {
        [XmlElement("ISIN")]
        [Required]
        [RegularExpression(@"^[A-Z0-9]{12}$")]
        public string ISIN { get; set; }

        [XmlElement("NewCode")]
        [RegularExpression(@"^[A-Z0-9]{3,4}$")]
        public string NewCode { get; set; }

        [XmlElement("Issuer")]
        [Required]
        [DataType(DataType.Text)]
        public string Issuer { get; set; }

        [XmlElement("Currency")]
        [Required]
        [RegularExpression(@"^[A-Z]{3}$")]
        public string Currency { get; set; }

        [XmlElement("MaturityDate")]
        [Required]
        public string MaturityDate { get; set; }
    }
}