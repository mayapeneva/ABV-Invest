namespace ABV_Invest.Models
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;
    using Models;

    public class AbvInvestUser : IdentityUser
    {
        public AbvInvestUser()
        {
            this.Portfolio = new HashSet<DailySecuritiesPerClient>();
            this.Deals = new HashSet<DailyDeals>();
        }

        public string FullName { get; set; }

        public virtual ICollection<DailySecuritiesPerClient> Portfolio { get; set; }

        public virtual ICollection<DailyDeals> Deals { get; set; }

        public virtual Balance Balance { get; set; }

        public int BalanceId { get; set; }
    }
}