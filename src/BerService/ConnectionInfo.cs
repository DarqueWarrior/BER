namespace BerService
{
   using BerService.DAL;
   using BerService.Model;
   using Microsoft.Extensions.Options;

   /// <summary>
   /// Implements IConnectionInfo for use with DI for the
   /// repository.
   /// </summary>
   public class ConnectionInfo : IConnectionInfo
   {
      public ConnectionInfo(IOptions<ConfigData> configData)
      {
         this.ConnectionString = configData.Value.DefaultConnection;
      }

      public string ConnectionString { get; set; }
   }
}
