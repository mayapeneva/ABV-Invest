﻿// <auto-generated />
using System;
using ABV_Invest.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ABV_Invest.Data.Migrations
{
    [DbContext(typeof(AbvDbContext))]
    [Migration("20181129202831_AddIdsOfRelevantCollectionEntitiesInDealAndSecuritiesPerClient")]
    partial class AddIdsOfRelevantCollectionEntitiesInDealAndSecuritiesPerClient
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ABV_Invest.Models.AbvInvestUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<int>("BalanceId");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FullName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AbvInvestUsers");
                });

            modelBuilder.Entity("ABV_Invest.Models.Balance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AbvInvestUserId");

                    b.Property<decimal>("ActualProfit");

                    b.Property<decimal>("ActualProfitPercentage");

                    b.Property<decimal>("Cash");

                    b.Property<decimal>("MoneyInvested");

                    b.Property<decimal>("PossibleProfit");

                    b.Property<decimal>("TotalSecuritiesMarketPrice");

                    b.HasKey("Id");

                    b.HasIndex("AbvInvestUserId")
                        .IsUnique()
                        .HasFilter("[AbvInvestUserId] IS NOT NULL");

                    b.ToTable("Balances");
                });

            modelBuilder.Entity("ABV_Invest.Models.Currency", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Abbreviation");

                    b.Property<string>("Code");

                    b.HasKey("Id");

                    b.ToTable("Currencies");
                });

            modelBuilder.Entity("ABV_Invest.Models.DailyDeals", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AbvInvestUserId");

                    b.Property<DateTime>("Date");

                    b.HasKey("Id");

                    b.HasIndex("AbvInvestUserId");

                    b.ToTable("DailyDeals");
                });

            modelBuilder.Entity("ABV_Invest.Models.DailySecuritiesPerClient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AbvInvestUserId");

                    b.Property<DateTime>("Date");

                    b.HasKey("Id");

                    b.HasIndex("AbvInvestUserId");

                    b.ToTable("DailySecuritiesPerClient");
                });

            modelBuilder.Entity("ABV_Invest.Models.Deal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CurrencyId");

                    b.Property<int>("DailyDealsId");

                    b.Property<string>("DealType")
                        .IsRequired();

                    b.Property<decimal>("Fee");

                    b.Property<int>("MarketId");

                    b.Property<decimal>("Price");

                    b.Property<int>("Quantity");

                    b.Property<int>("SecurityId");

                    b.Property<DateTime>("Settlement");

                    b.Property<decimal>("TotalPrice");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("DailyDealsId");

                    b.HasIndex("MarketId");

                    b.HasIndex("SecurityId");

                    b.ToTable("Deals");
                });

            modelBuilder.Entity("ABV_Invest.Models.Issuer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Issuers");
                });

            modelBuilder.Entity("ABV_Invest.Models.Market", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Markets");
                });

            modelBuilder.Entity("ABV_Invest.Models.SecuritiesPerClient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal>("AveragePriceBuy");

                    b.Property<int>("DailySecuritiesPerClientId");

                    b.Property<decimal>("MarketPrice");

                    b.Property<decimal>("PortfolioShare");

                    b.Property<decimal>("Profit");

                    b.Property<decimal>("ProfitPercentаge");

                    b.Property<int>("Quantity");

                    b.Property<int>("SecurityId");

                    b.Property<decimal>("TotalMarketPrice");

                    b.Property<decimal>("TotalPriceBuy");

                    b.HasKey("Id");

                    b.HasIndex("DailySecuritiesPerClientId");

                    b.HasIndex("SecurityId");

                    b.ToTable("SecuritiesPerClient");
                });

            modelBuilder.Entity("ABV_Invest.Models.Security", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BfbCode");

                    b.Property<string>("Isin");

                    b.Property<int>("IssuerId");

                    b.Property<string>("SecuritiesType")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("IssuerId");

                    b.ToTable("Securities");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("ABV_Invest.Models.Balance", b =>
                {
                    b.HasOne("ABV_Invest.Models.AbvInvestUser", "AbvInvestUser")
                        .WithOne("Balance")
                        .HasForeignKey("ABV_Invest.Models.Balance", "AbvInvestUserId");
                });

            modelBuilder.Entity("ABV_Invest.Models.DailyDeals", b =>
                {
                    b.HasOne("ABV_Invest.Models.AbvInvestUser", "AbvInvestUser")
                        .WithMany("Deals")
                        .HasForeignKey("AbvInvestUserId");
                });

            modelBuilder.Entity("ABV_Invest.Models.DailySecuritiesPerClient", b =>
                {
                    b.HasOne("ABV_Invest.Models.AbvInvestUser", "AbvInvestUser")
                        .WithMany("Portfolio")
                        .HasForeignKey("AbvInvestUserId");
                });

            modelBuilder.Entity("ABV_Invest.Models.Deal", b =>
                {
                    b.HasOne("ABV_Invest.Models.Currency", "Currency")
                        .WithMany()
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ABV_Invest.Models.DailyDeals", "DailyDeals")
                        .WithMany("Deals")
                        .HasForeignKey("DailyDealsId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ABV_Invest.Models.Market", "Market")
                        .WithMany()
                        .HasForeignKey("MarketId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ABV_Invest.Models.Security", "Security")
                        .WithMany()
                        .HasForeignKey("SecurityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ABV_Invest.Models.SecuritiesPerClient", b =>
                {
                    b.HasOne("ABV_Invest.Models.DailySecuritiesPerClient", "DailySecuritiesPerClient")
                        .WithMany("SecuritiesPerIssuerCollection")
                        .HasForeignKey("DailySecuritiesPerClientId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ABV_Invest.Models.Security", "Security")
                        .WithMany()
                        .HasForeignKey("SecurityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ABV_Invest.Models.Security", b =>
                {
                    b.HasOne("ABV_Invest.Models.Issuer", "Issuer")
                        .WithMany("Securities")
                        .HasForeignKey("IssuerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("ABV_Invest.Models.AbvInvestUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("ABV_Invest.Models.AbvInvestUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ABV_Invest.Models.AbvInvestUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("ABV_Invest.Models.AbvInvestUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
