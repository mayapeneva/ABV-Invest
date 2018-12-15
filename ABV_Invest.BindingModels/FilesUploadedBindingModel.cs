namespace ABV_Invest.BindingModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    public class FilesUploadedBindingModel
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile XMLFile { get; set; }
    }
}