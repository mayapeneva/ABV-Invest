namespace ABV_Invest.Web.Extensions
{
    using System;
    using Common;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public static class DateValidator
    {
        private const string BeginningDate = "01/01/2016";

        public static bool ValidateDate(DateTime dateChosen)
        {
            if (dateChosen.Date > DateTime.UtcNow
                || dateChosen.Date < DateTime.Parse(BeginningDate))
            {
                return false;
            }

            return true;
        }
    }
}