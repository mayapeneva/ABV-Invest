using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ABV_Invest.Data
{
    public class ABVInvestDbContext : IdentityDbContext
    {
        public ABVInvestDbContext(DbContextOptions<ABVInvestDbContext> options)
            : base(options)
        {
        }
    }
}