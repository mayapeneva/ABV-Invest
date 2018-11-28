namespace ABV_Invest.BindingModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class DateChosenBindingModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}