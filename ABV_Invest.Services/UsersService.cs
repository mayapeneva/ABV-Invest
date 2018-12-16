namespace ABV_Invest.Services
{
    using System.Linq;
    using AutoMapper;
    using Contracts;
    using Data;
    using DTOs;

    public class UsersService : IUsersService
    {
        private readonly AbvDbContext db;

        public UsersService(AbvDbContext db)
        {
            this.db = db;
        }

        public UserDto GetUserByUserName(string username)
        {
            var user = this.db.AbvInvestUsers.SingleOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return null;
            }

            return Mapper.Map<UserDto>(user);
        }
    }
}