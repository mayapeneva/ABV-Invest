namespace ABV_Invest.Common.BindingModels
{
    using Common;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.ComponentModel.DataAnnotations;

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