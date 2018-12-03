﻿namespace ABV_Invest.ViewModels
{
    using System;

    public class DealsViewModel
    {
        public string DealType { get; set; }

        public string SecurityIssuer { get; set; }

        public string SecurityBfbCode { get; set; }

        public int Quantity { get; set; }

        public string Price { get; set; }

        public decimal TotalPrice { get; set; }

        public decimal Fee { get; set; }

        public string Currency { get; set; }

        public string Settlement { get; set; }

        public string Market { get; set; }
    }
}