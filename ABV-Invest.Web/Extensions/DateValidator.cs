namespace ABV_Invest.Web.Extensions
{
    using System;
    using Common;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public static class DateValidator
    {
        public static bool ValidateDate(DateTime dateChosen)
        {
            if (dateChosen.Date > DateTime.UtcNow
                || dateChosen.Date < DateTime.Parse(Constants.DateTimeShortParseFormat))
            {
                return false;
            }

            return true;
        }
    }
}