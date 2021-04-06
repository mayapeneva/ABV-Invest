namespace ABV_Invest.Web
{
    using ABV_Invest.Common.BindingModels;
    using ABV_Invest.Common.DTOs;
    using ABV_Invest.Common.Mapping;
    using ABV_Invest.Models;
    using ABV_Invest.Web.Extensions;
    using ABV_Invest.Web.Extensions.Contracts;
    using ABV_Invest.Web.Extensions.Parsers;
    using Common.EmailSender;
    using Data;
    using Extensions.Seeders;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Rotativa.AspNetCore;
    using Services;
    using Services.Contracts;
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
            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddApplicationInsightsTelemetry();

            // Application services
            services.AddScoped<IPortfoliosService, PortfoliosService>();
            services.AddScoped<IDealsService, DealsService>();
            services.AddScoped<IBalancesService, BalancesService>();
            services.AddScoped<IDataService, DataService>();

            services.AddScoped<IRSSFeedParser, RSSFeedParser>();
            services.AddScoped<IUploadsHelper, UploadsHelper>();
            services.AddSingleton<IEmailSender, EmailSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

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

            RotativaConfiguration.Setup(env.WebRootPath, "Rotativa");
        }
    }
}