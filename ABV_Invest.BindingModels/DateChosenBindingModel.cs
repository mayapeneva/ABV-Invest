namespace ABV_Invest.BindingModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Common;

    public class DateChosenBindingModel
    {
        [Required]
        [DataType(DataType.Date, ErrorMessage = Messages.DateError)]
        public DateTime Date { get; set; }
    }
}