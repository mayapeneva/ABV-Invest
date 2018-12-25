namespace ABV_Invest.BindingModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    public class FilesUploadedBindingModel
    {
        [Required]
        [DataType(DataType.Date, ErrorMessage = "Моля, въведете дата с правилен формат.")]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Upload, ErrorMessage = "Моля, изберете файл с правилен формат.")]
        public IFormFile XMLFile { get; set; }
    }
}