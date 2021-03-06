﻿namespace ABV_Invest.Common.BindingModels.Uploads.Portfolios
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("New", IncludeInSchema = true)]
    public class PortfolioRowBindingModel
    {
        [XmlElement("Client")]
        [Required]
        public Client Client { get; set; }

        [XmlElement("Instrument")]
        [Required]
        public Instrument Instrument { get; set; }

        [XmlElement("AccountData")]
        [Required]
        public AccountData AccountData { get; set; }

        [XmlElement("Other")]
        [Required]
        public Other Other { get; set; }
    }
}