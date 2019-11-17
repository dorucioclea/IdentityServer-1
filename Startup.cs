using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServerAspNetIdentity.Data;
using IdentityServerAspNetIdentity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using StackExchange.Redis;

namespace IdentityServerAspNetIdentity
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }


        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetValue<string>("UserDB_ConnString")));

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration.GetValue<string>("Google_ClientID");
                    options.ClientSecret = Configuration.GetValue<string>("Google_ClientSecret");
                });

            var redisConnection = ConnectionMultiplexer.Connect(Configuration.GetValue<string>("Cache_ConnString"));
            IIdentityServerBuilder builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
                options.IssuerUri = Configuration.GetValue<string>("IssuerUri");
                options.PublicOrigin = options.IssuerUri;
            })
                .AddSigningCredential(new X509Certificate2(GetSecretAsBytes("PS_Certs_Pfx"), GetSecretAsString("PS_Certs_PfxPassword")))
                .AddInMemoryIdentityResources(Config.GetIdentityResources(Configuration))
                .AddInMemoryApiResources(Config.GetApis(Configuration))
                .AddInMemoryClients(Config.GetClients(Configuration))
                .AddAspNetIdentity<ApplicationUser>()
                .AddOperationalStore(options =>
                {
                    options.RedisConnectionMultiplexer = redisConnection;
                    options.Db = 1;
                })
                .AddRedisCaching(options =>
                {
                    options.RedisConnectionMultiplexer = redisConnection;
                    options.KeyPrefix = "IDS_Cache_";
                });

            services.AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseForwardedHeaders();

            app.UseStaticFiles();
            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();
        }

        public byte[] GetSecretAsBytes(string key)
        {
            const string DOCKER_SECRET_PATH = "/run/secrets/";
            if (Directory.Exists(DOCKER_SECRET_PATH))
            {
                IFileProvider provider = new PhysicalFileProvider(DOCKER_SECRET_PATH);
                IFileInfo fileInfo = provider.GetFileInfo(key);
                if (fileInfo.Exists)
                {
                    using (Stream stream = fileInfo.CreateReadStream())
                    using (var memstream = new MemoryStream())
                    {
                        stream.CopyTo(memstream);
                        return memstream.ToArray();
                    }
                }
            }
            return default(Byte[]);
        }

        public string GetSecretAsString(string key)
        {
            const string DOCKER_SECRET_PATH = "/run/secrets/";
            if (Directory.Exists(DOCKER_SECRET_PATH))
            {
                IFileProvider provider = new PhysicalFileProvider(DOCKER_SECRET_PATH);
                IFileInfo fileInfo = provider.GetFileInfo(key);
                if (fileInfo.Exists)
                {
                    using (Stream stream = fileInfo.CreateReadStream())
                    using (var streamReader = new StreamReader(stream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            return null;
        }
    }
}