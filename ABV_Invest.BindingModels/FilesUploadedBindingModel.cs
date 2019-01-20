namespace ABV_Invest.BindingModels
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.ComponentModel.DataAnnotations;
    using Common;

    public class FilesUploadedBindingModel
    {
        [Required]
        [DataType(DataType.Date, ErrorMessage = Messages.DateError)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Upload, ErrorMessage = Messages.FileError)]
        public IFormFile XMLFile { get; set; }
    }
}