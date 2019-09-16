namespace BerService.FunctionApp
{
   using BerService.Model;
   using Microsoft.Extensions.Configuration;

   /// <summary>
   /// Implements IConnectionInfo for use with DI for the
   /// repository.
   /// </summary>
   public class ConnectionInfo : IConnectionInfo
   {
      public ConnectionInfo()
      {
         var config = new ConfigurationBuilder().AddJsonFile("local.settings.json", optional: true, reloadOnChange: true).AddEnvironmentVariables().Build();

         this.ConnectionString = config.GetConnectionString("DefaultConnection");
      }

      public string ConnectionString { get; set; }
   }
}
