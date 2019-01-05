namespace ABV_Invest.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class AbvInvestUser : IdentityUser
    {
        public AbvInvestUser()
        {
            this.Portfolio = new HashSet<DailySecuritiesPerClient>();
            this.Deals = new HashSet<DailyDeals>();
            this.Balances = new HashSet<DailyBalance>();
        }

        [RegularExpression(@"^\d{5}$")]
        public string PIN { get; set; }

        [DataType(DataType.Text)]
        [MinLength(4)]
        public string FullName { get; set; }

        public virtual ICollection<DailySecuritiesPerClient> Portfolio { get; set; }

        public virtual ICollection<DailyDeals> Deals { get; set; }

        public virtual ICollection<DailyBalance> Balances { get; set; }

        public int BalanceId { get; set; }
    }
}