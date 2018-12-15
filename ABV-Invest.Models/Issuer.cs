namespace ABV_Invest.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Base;

    public class Issuer : BaseEntity<int>
    {
        public Issuer()
        {
            this.Securities = new HashSet<Security>();
        }

        [Required]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        public virtual ICollection<Security> Securities { get; set; }
    }
}