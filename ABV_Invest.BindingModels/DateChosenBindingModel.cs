﻿namespace ABV_Invest.BindingModels
{
    using Common;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class DateChosenBindingModel
    {
        [Required]
        [DataType(DataType.Date, ErrorMessage = Messages.DateError)]
        public DateTime Date { get; set; }
    }
}