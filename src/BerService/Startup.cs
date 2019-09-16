namespace BerService
{
   using AutoMapper;
   using BerService.DAL;
   using BerService.DAL.Repositories;
   using BerService.Model;
   using Microsoft.AspNetCore.Builder;
   using Microsoft.AspNetCore.Hosting;
   using Microsoft.AspNetCore.Mvc;
   using Microsoft.AspNetCore.Mvc.Versioning;
   using Microsoft.Extensions.Configuration;
   using Microsoft.Extensions.DependencyInjection;
   using Microsoft.Extensions.Logging;
   using Microsoft.IdentityModel.Tokens;
   using Swashbuckle.AspNetCore.Swagger;
   using System;
   using System.Collections.Generic;
   using System.Linq;
   using System.Text;
   using System.Threading.Tasks;

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
         services.AddScoped<IConnectionInfo, ConnectionInfo>();

         services.Configure<TokenData>(Configuration.GetSection("Tokens"));
         services.Configure<ConfigData>(Configuration.GetSection("ConnectionStrings"));

         // Used to map contract types to/from DAL types.
         var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.ManifestModule.Name == "BerService.Model.dll").ToArray();
         services.AddAutoMapper(cfg =>
         {
            cfg.CreateMap<Model.Record, Model.Contracts.RecordContract>();
            cfg.CreateMap<Model.Contracts.RecordContract, Model.Record>();
         },
         assemblies);

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

         services.AddSwaggerGen(c =>
         {
            c.SwaggerDoc("v1", new Info
            {
               Title = "BER API",
               Version = "v1"
            });

            var security = new Dictionary<string, IEnumerable<string>>
            {
               { "Bearer", new string[] { }},
            };

            c.AddSecurityDefinition("Bearer", new ApiKeyScheme
            {
               Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
               Name = "Authorization",
               In = "header",
               Type = "apiKey"
            });

            c.AddSecurityRequirement(security);
         });

         services.AddApiVersioning(cfg =>
         {
            cfg.DefaultApiVersion = new ApiVersion(1, 0);
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

         app.UseSwagger();
         app.UseSwaggerUI(c =>
         {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "BER API V1");
         });

         app.Run(async (context) => await Task.Run(() => context.Response.Redirect("/swagger")));
      }
   }
}
