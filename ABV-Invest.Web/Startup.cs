namespace ABV_Invest.Web
{
    using System.Collections.Generic;
    using ABV_Invest.Models;
    using AutoMapper;
    using BindingModels;
    using Data;
    using DTOs;
    using Extensions;
    using Firewall;
    using Mapping;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Services;
    using Services.Contracts;
    using Services.EmailSender;
    using ViewModels;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AutoMapperConfig.RegisterMappings(
                typeof(PortfolioDto).Assembly,
                typeof(PortfolioViewModel).Assembly,
                typeof(DealDto).Assembly,
                typeof(DealViewModel).Assembly,
                typeof(BalanceDto).Assembly,
                typeof(BalanceViewModel).Assembly,
                typeof(DateChosenBindingModel).Assembly);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<AbvDbContext>(options =>
                options.UseSqlServer(this.Configuration.GetConnectionString("DefaultConnection"))
                    .UseLazyLoadingProxies());

            services.AddIdentity<AbvInvestUser, IdentityRole>()
                .AddEntityFrameworkStores<AbvDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.AddMvc(options => options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Application services
            services.AddScoped<IPortfoliosService, PortfoliosService>();
            services.AddScoped<IDealsService, DealsService>();
            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IBalancesService, BalancesService>();
            services.AddScoped<IDataService, DataService>();

            services.AddSingleton<IEmailSender, EmailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseFirewall(
                FirewallRulesEngine
                .DenyAllAccess()
                .ExceptFromLocalhost()
                .ExceptFromCountries(new List<CountryCode> { CountryCode.BG }));

            app.UseSeedRolesMiddleware();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}