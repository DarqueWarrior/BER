namespace BerService.Model
{
   /// <summary>
   /// Interface used with DI for the connection string for the DAL. Each
   /// client can register their own implementation with DI to provided
   /// the connection string to the DAL.
   /// </summary>
   public interface IConnectionInfo
   {
      string ConnectionString { get; set; }
   }
}
