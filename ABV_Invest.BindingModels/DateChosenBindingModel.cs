namespace ABV_Invest.BindingModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class DateChosenBindingModel
    {
        [Required]
        [DataType(DataType.Date, ErrorMessage = "Моля, въведете дата с правилен формат.")]
        public DateTime Date { get; set; }
    }
}