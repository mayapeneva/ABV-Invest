namespace ABV_Invest.Models
{
    using System.Collections.Generic;
    using Base;

    public class Issuer : BaseEntity<int>
    {
        public Issuer()
        {
            this.Securities = new HashSet<Security>();
        }

        public string Name { get; set; }

        public virtual ICollection<Security> Securities { get; set; }
    }
}