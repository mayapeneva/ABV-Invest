namespace ABV_Invest.Models
{
    using System.ComponentModel.DataAnnotations;
    using Base;
    using Enums;

    public class Security : BaseEntity<int>
    {
        private int typeCodeStartIndex = 2;
        private int typeCodeLenght = 2;

        private SecuritiesType securitiesType;

        public virtual Issuer Issuer { get; set; }
        public int IssuerId { get; set; }

        public SecuritiesType? SecuritiesType
        {
            get => this.securitiesType;
            set
            {
                if (this.ISIN.StartsWith("BG"))
                {
                    var typeCode = this.ISIN.Substring(this.typeCodeStartIndex, this.typeCodeLenght);
                    if (typeCode == "11")
                    {
                        this.securitiesType = (SecuritiesType)1;
                    }
                    else if (typeCode == "40")
                    {
                        this.securitiesType = (SecuritiesType)2;
                    }
                    else if (typeCode == "21")
                    {
                        this.securitiesType = (SecuritiesType)3;
                    }
                    else if (typeCode == "90")
                    {
                        this.securitiesType = (SecuritiesType)4;
                    }
                    else if (typeCode == "92")
                    {
                        this.securitiesType = (SecuritiesType)5;
                    }
                    else
                    {
                        this.securitiesType = (SecuritiesType)6;
                    }
                }
            }
        }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9]{12}$")]
        public string ISIN { get; set; }

        [RegularExpression(@"^[A-Z0-9]{3,4}$")]
        public string BfbCode { get; set; }

        public Currency Currency { get; set; }
    }
}