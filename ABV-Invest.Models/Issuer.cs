namespace ABV_Invest.Models
{
    using System.Collections.Generic;

    public class Issuer : BaseEntity
    {
        public Issuer()
        {
            this.Securities = new HashSet<Security>();
        }

        public string Name { get; set; }

        public virtual ICollection<Security> Securities { get; set; }
    }
}