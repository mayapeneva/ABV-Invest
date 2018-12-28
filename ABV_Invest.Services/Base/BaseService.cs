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
    }
}