namespace PeopleTracker.BerService.DAL.Repositories
{
   using System;
   using System.Data.SqlClient;
   using System.Threading.Tasks;
   using Microsoft.Azure.Services.AppAuthentication;
   using Microsoft.EntityFrameworkCore;
   using Microsoft.Extensions.Options;
   using PeopleTracker.BerService.Controllers.DAL;
   using PeopleTracker.BerService.DAL.Models;

   public class SqlServerRepository : IRepository
   {
      private readonly RecordContext _db;

      /// <summary>
      /// To use this class with a .net core Web API there must be a section 
      /// added to the appsettings.json like the following:
      /// "DAL": {
      ///    "DefaultConnection": "<connectionString>"
      /// }
      /// </summary>
      /// <param name="configData"></param>
      public SqlServerRepository(IOptions<ConfigData> configData)
      {
         var ob = new DbContextOptionsBuilder<RecordContext>();         

         var connection = new SqlConnection
         {
            ConnectionString = configData.Value.DefaultConnection
         };

         // DefaultConnection does not contain localhost\sqlexpress means app is running in Azure with the SQLDB connection string you configured
         if (configData.Value.DefaultConnection.IndexOf("localhost\\sqlexpress") == -1)
         {
            connection.AccessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result;
         }

         ob.UseSqlServer(connection);

         _db = new RecordContext(ob.Options);
         _db.Database.Migrate();
      }

      public async Task<Record> FindRecord(string applicationName, string dataType, string version)
      {
         return await _db.Records.FindAsync(applicationName, dataType, version);
      }

      public async Task<Record> UpsertRecord(Record record)
      {
         var result = await FindRecord(record.ApplicationName, record.DataType, record.Version);

         if (result != null)
         {
            result.DateModified = DateTime.UtcNow;
            result.Value = record.Value;
         }
         else
         {
            result = record;
            record.DateCreated = DateTime.UtcNow;
            _db.Records.Add(record);
         }

         await _db.SaveChangesAsync();

         return result;
      }

      public async Task<int> DeleteRecord(string applicationName, string dataType, string version)
      {
         var result = await FindRecord(applicationName, dataType, version);

         if (result != null)
         {
            _db.Records.Remove(result);
            return await _db.SaveChangesAsync();
         }

         return 0;
      }
   }
}