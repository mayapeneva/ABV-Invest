namespace ABV_Invest.Services
{
    using System.Linq;
    using AutoMapper;
    using Base;
    using Contracts;
    using Data;
    using DTOs;

    public class UsersService : BaseService, IUsersService
    {
        public UsersService(AbvDbContext db)
            : base(db)
        {
        }

        public T GetUserByUserName<T>(string username)
        {
            var user = this.Db.AbvInvestUsers.SingleOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return default(T);
            }

            return Mapper.Map<T>(user);
        }
    }
}