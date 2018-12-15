namespace ABV_Invest.BindingModels.Uploads.Portfolios
{
    using System.ComponentModel.DataAnnotations;

    public class Client
    {
        [Required]
        [RegularExpression(@"^[A-Z0-9]{5,10}$")]
        public string CDNNumber { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }
    }
}