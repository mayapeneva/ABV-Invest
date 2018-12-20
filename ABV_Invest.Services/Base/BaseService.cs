namespace ABV_Invest.Services.Base
{
    using Data;
    using DataAnotations = System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public abstract class BaseService
    {
        protected BaseService(AbvDbContext db)
        {
            this.Db = db;
        }

        protected AbvDbContext Db { get; set; }

        protected static bool IsValid(object obj)
        {
            var validationContext = new DataAnotations.ValidationContext(obj);
            var validationResult = new List<DataAnotations.ValidationResult>();

            return DataAnotations.Validator.TryValidateObject(obj, validationContext, validationResult, true);
        }
    }
}