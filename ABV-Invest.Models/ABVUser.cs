namespace ABV_Invest.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;

    public class AbvUser : IdentityUser
    {
        public AbvUser()
        {
            this.Portfolio = new HashSet<DailySecuritiesPerIssuer>();
            this.Deals = new HashSet<DailyDeal>();
        }

        public string FullName { get; set; }

        public virtual ICollection<DailySecuritiesPerIssuer> Portfolio { get; set; }

        public virtual ICollection<DailyDeal> Deals { get; set; }

        public virtual Balance Balance { get; set; }
    }
}