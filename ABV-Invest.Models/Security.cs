namespace ABV_Invest.Models
{
    using Enums;

    public class Security : BaseEntity
    {
        public virtual Issuer Issuer { get; set; }
        public int IssuerId { get; set; }

        public SecuritiesType SecuritiesType { get; set; }

        public string Isin { get; set; }

        public string BfbCode { get; set; }
    }
}