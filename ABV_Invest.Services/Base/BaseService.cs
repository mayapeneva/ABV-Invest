namespace ABV_Invest.Services.Base
{
    using Data;

    public abstract class BaseService
    {
        protected BaseService(AbvDbContext db)
        {
            this.Db = db;
        }

        protected AbvDbContext Db { get; set; }
    }
}