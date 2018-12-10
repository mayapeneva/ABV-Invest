namespace ABV_Invest.Models
{
    using Base;

    public class Currency : BaseEntity<int>
    {
        public string Code { get; set; }

        public string Abbreviation { get; set; }
    }
}