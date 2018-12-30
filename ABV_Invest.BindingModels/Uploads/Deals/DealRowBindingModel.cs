namespace ABV_Invest.BindingModels.Uploads.Deals
{
    using System.ComponentModel.DataAnnotations;
    using System.Xml.Serialization;

    [XmlType("New", IncludeInSchema = true)]
    public class DealRowBindingModel
    {
        [XmlElement("Client")]
        [Required]
        public Client Client { get; set; }

        [XmlElement("Instrument")]
        [Required]
        public Instrument Instrument { get; set; }

        [XmlElement("DealData")]
        [Required]
        public DealData DealData { get; set; }
    }
}