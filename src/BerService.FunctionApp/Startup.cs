namespace BerService.FunctionApp
{
   using AutoMapper;
   using BerService.DAL.Repositories;
   using BerService.Model;
   using Microsoft.Azure.Functions.Extensions.DependencyInjection;
   using Microsoft.Extensions.DependencyInjection;
   using System;
   using System.Linq;

   public class Startup : FunctionsStartup
   {
      public override void Configure(IFunctionsHostBuilder builder)
      {
         builder.Services.AddScoped<IRepository, SqlServerRepository>();
         builder.Services.AddScoped<IConnectionInfo, ConnectionInfo>();

         // Used to map contract types to/from DAL types.
         var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.ManifestModule.Name == "BerService.Model.dll").ToArray();
         builder.Services.AddAutoMapper(cfg =>
         {
            cfg.CreateMap<Model.Record, Model.Contracts.RecordContract>();
            cfg.CreateMap<Model.Contracts.RecordContract, Model.Record>();
         },
         assemblies);
      }
   }
}
