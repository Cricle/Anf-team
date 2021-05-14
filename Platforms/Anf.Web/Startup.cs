using Anf.Easy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Anf.KnowEngines;
using System.IO;
using Anf.Easy.Store;
using System;
using Anf.Web.Services;
using Anf.Easy.Visiting;
using Anf.Platform;
using Anf.Engine;
using StackExchange.Redis;
using Anf.Web.Hubs;
using System.Reflection;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Threading.Tasks;
using Anf.ChannelModel.Mongo;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Caching.Distributed;
using Anf.ResourceFetcher;
using Anf.ChannelModel.Entity;
using Anf.ResourceFetcher.Fetchers;
using Anf.WebService;
using Anf.ResourceFetcher.Redis;

namespace Anf.Web
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
            AppEngine.Reset(services);
            AppEngine.AddServices(NetworkAdapterTypes.HttpClient);
            var store = FileStoreService.FromMd5Default(Path.Combine(Environment.CurrentDirectory, XComicConst.CacheFolderName));

            services.AddSingleton<IComicSaver>(store);
            services.AddSingleton<IStoreService>(store);
            services.AddSingleton<IResourceFactoryCreator<Stream>, PlatformResourceCreatorFactory<Stream>>();
            services.AddSingleton<ProposalEngine>();
            services.AddScoped<IComicVisiting<Stream>, WebComicVisiting>();
            services.AddKnowEngines();
            services.AddControllersWithViews();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
            services.AddSignalR()
                .AddAzureSignalR();
            services.AddDbContext<AnfDbContext>((x, y) =>
            {
                var config = x.GetRequiredService<IConfiguration>();
                y.UseSqlServer(config["ConnectionStrings:anfdb"]);
            }).AddIdentity<AnfUser,AnfRole>(x=> 
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredUniqueChars = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<AnfDbContext>();
            services.AddSingleton<IDistributedCache, RedisCache>();
            services.AddOptions<RedisCacheOptions>()
                .Configure(x => x.Configuration = Configuration["ConnectionStrings:CacheConnection"]);
            services.AddResponseCompression();
            services.AddScoped<UserService>();
            services.AddScoped<UserIdentityService>();
            services.AddSwaggerGen(c=> 
            {
                c.SwaggerDoc("Anf", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Anf API"
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddAuthorization();
            services.AddAuthentication(options => 
            {
                options.AddScheme<AnfAuthenticationHandler>(AnfAuthenticationHandler.SchemeName, "default scheme");
                options.DefaultAuthenticateScheme = AnfAuthenticationHandler.SchemeName;
                options.DefaultChallengeScheme = AnfAuthenticationHandler.SchemeName;
            });
            services.AddScoped<AnfAuthenticationHandler>();
            services.AddOptions<FetchOptions>();
            services.AddOptions<ResourceLockOptions>();
            services.AddFetcherProvider()
                .AddRedisFetcherProvider()
                .AddMssqlResourceFetcher()
                .AddDefaultFetcherProvider();
            services.AddResourceFetcher()
                .AddMssqlResourceFetcher()
                .AddRedisFetcherProvider();
            services.AddScoped<IDbContextTransfer, AnfDbContextTransfer>();
            services.AddRedis();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseResponseCompression();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAzureSignalR(builder =>
            {
                builder.MapHub<ReadingHub>("/hubs/v1/reading");
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/Anf/swagger.json", "Anf API");
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    //spa.UseAngularCliServer(npmScript: "start");
                }
            });

            app.ApplicationServices.UseKnowEngines();
            //using (var s = app.ApplicationServices.GetServiceScope())
            //{
            //    var db = s.ServiceProvider.GetRequiredService<AnfDbContext>();
            //    db.Database.EnsureCreated();
            //}
            //var scope = app.ApplicationServices.CreateScope();
            //_ = AnfMongoDbExtensions.InitMongoAsync(scope);
        }
    }
}
