namespace ABV_Invest.Web
{
    using ABV_Invest.Models;
    using AutoMapper;
    using BindingModels;
    using Common;
    using Data;
    using DTOs;
    using Extensions;
    using Mapping;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
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
                typeof(Deal).Assembly,
                typeof(DealsDto).Assembly,
                typeof(DealsViewModel).Assembly,
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

            services.AddAutoMapper();
            services.AddAntiforgery();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Application services
            services.AddScoped<IPortfoliosService, PortfoliosService>();
            services.AddScoped<IDealsService, DealsService>();
            services.AddScoped<INewsService, NewsService>();
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