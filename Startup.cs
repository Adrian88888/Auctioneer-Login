using Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Auctioneer.Hubs;

namespace Auctioneer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           
            services.AddDbContext<Database.Data.ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<Database.Data.ApplicationDbContext>();
            services.AddControllersWithViews();

            services.AddSignalR();

            services.AddMvc(options =>
            {
                // This pushes users to login if not authenticated
                options.Filters.Add(new AuthorizeFilter());
            });

            services.AddScoped<Database.Repository.IAuctionRepository, Database.Repository.AuctionRepository>();
            services.AddScoped<Database.Repository.IBidRepository, Database.Repository.BidRepository>();
            services.AddScoped<Database.Repository.ICarBrandRepository, Database.Repository.CarBrandRepository>();
            services.AddScoped<Database.Repository.ICarFeaturesRepository, Database.Repository.CarFeaturesRepository>();
            services.AddScoped<Database.Repository.ICarTypeRepository, Database.Repository.CarTypeRepository>();
            services.AddScoped<BidService>();
            services.AddScoped<BalanceService>();
            services.AddScoped<EmailSender>();
            services.AddScoped<DummyService>();

            services.AddAuthorization(options => {
                options.AddPolicy("readpolicy",
                    builder => builder.RequireRole("Admin", "Manager"));
                options.AddPolicy("writepolicy",
                    builder => builder.RequireRole("Admin"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Auction}/{action=Index}/{id?}");

                endpoints.MapHub<RealtimeDataHub>("/realtimeDataHub");

                endpoints.MapRazorPages();
            });
        }
    }
}
