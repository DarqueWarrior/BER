namespace PeopleTracker.BerService
{
   using AutoMapper;
   using Microsoft.AspNetCore.Builder;
   using Microsoft.AspNetCore.Hosting;
   using Microsoft.AspNetCore.Mvc;
   using Microsoft.AspNetCore.Mvc.Versioning;
   using Microsoft.Extensions.Configuration;
   using Microsoft.Extensions.DependencyInjection;
   using Microsoft.Extensions.Logging;
   using Microsoft.IdentityModel.Tokens;
   using PeopleTracker.BerService.Controllers.DAL;
   using PeopleTracker.BerService.DAL.Repositories;
   using System.Text;

   public class Startup
   {
      public Startup(IConfiguration configuration)
      {
         Configuration = configuration;
      }

      public IConfiguration Configuration { get; }

      /// <summary>
      /// This method gets called by the runtime. Use this method to add services to the container.
      /// </summary>
      /// <param name="services"></param>
      public void ConfigureServices(IServiceCollection services)
      {
         services.AddScoped<IRepository, SqlServerRepository>();

         services.Configure<TokenData>(Configuration.GetSection("Tokens"));
         services.Configure<ConfigData>(Configuration.GetSection("ConnectionStrings"));

         // Used to map contract types to/from DAL types.
         services.AddAutoMapper();

         services.AddAuthentication().AddJwtBearer(cfg =>
         {
            cfg.SaveToken = true;
            cfg.RequireHttpsMetadata = false;

            cfg.TokenValidationParameters = new TokenValidationParameters
            {
               ValidateLifetime = true,
               ValidateIssuerSigningKey = true,
               ValidIssuer = Configuration["Tokens:Issuer"],
               ValidAudience = Configuration["Tokens:Audience"],
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
            };
         });

         services.AddLogging(logging =>
         {
            logging.AddConsole();
            logging.AddDebug();
         });

         services.AddMvc()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

         services.AddApiVersioning(cfg =>
         {
            cfg.DefaultApiVersion = new ApiVersion(0, 1);
            cfg.AssumeDefaultVersionWhenUnspecified = true;
            cfg.ReportApiVersions = true;
            cfg.ApiVersionReader = ApiVersionReader.Combine(
               new QueryStringApiVersionReader(),
               new HeaderApiVersionReader()
               {
                  HeaderNames = { "api-version" }
               });
         });
      }

      /// <summary>
      /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
      /// </summary>
      /// <param name="app"></param>
      /// <param name="env"></param>
      public void Configure(IApplicationBuilder app, IHostingEnvironment env)
      {
         if (env.IsDevelopment())
         {
            app.UseDeveloperExceptionPage();
         }
         else
         {
            app.UseHsts();
         }

         app.UseAuthentication();
         app.UseHttpsRedirection();
         app.UseMvc();
      }
   }
}
