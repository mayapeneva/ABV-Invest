namespace ABV_Invest.Data
{
    using Models;
    using Models.Enums;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using System;

    public class AbvDbContext : IdentityDbContext<AbvInvestUser>
    {
        public AbvDbContext(DbContextOptions<AbvDbContext> options)
            : base(options)
        {
        }

        public DbSet<AbvInvestUser> AbvInvestUsers { get; set; }

        public DbSet<DailySecuritiesPerClient> DailySecuritiesPerClient { get; set; }

        public DbSet<SecuritiesPerClient> SecuritiesPerClient { get; set; }

        public DbSet<Security> Securities { get; set; }

        public DbSet<DailyDeals> DailyDeals { get; set; }

        public DbSet<Deal> Deals { get; set; }

        public DbSet<DailyBalance> DailyBalances { get; set; }

        public DbSet<Balance> Balances { get; set; }

        public DbSet<Currency> Currencies { get; set; }

        public DbSet<Issuer> Issuers { get; set; }

        public DbSet<Market> Markets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AbvInvestUser>()
                .ToTable("AbvInvestUsers");

            builder.Entity<Deal>()
                .Property(d => d.DealType)
                .HasConversion(dt => dt.ToString(), dt => (DealType)Enum.Parse(typeof(DealType), dt));

            builder.Entity<Security>()
                .Property(s => s.SecuritiesType)
                .HasConversion(st => st.ToString(), st => (SecuritiesType)Enum.Parse(typeof(SecuritiesType), st));
        }
    }
}