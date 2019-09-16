namespace BerService.DAL.Repositories
{
   using BerService.Model;
   using Microsoft.Azure.Services.AppAuthentication;
   using Microsoft.EntityFrameworkCore;
   using System;
   using System.Collections.Generic;
   using System.Data.SqlClient;
   using System.Threading.Tasks;

   public class SqlServerRepository : IRepository
   {
      private readonly RecordContext _db;

      /// <summary>
      /// To use this class with a .net core Web API there must be a section 
      /// added to the appsettings.json like the following:
      /// "ConnectionStrings": {
      ///    "DefaultConnection": "<connectionString>"
      /// }
      /// </summary>
      /// <param name="configData"></param>
      public SqlServerRepository(IConnectionInfo connectionInfo)
      {
         var ob = new DbContextOptionsBuilder<RecordContext>();

         var connection = new SqlConnection
         {
            ConnectionString = connectionInfo.ConnectionString
         };

         var connStr = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"];

         // DefaultConnection contains database.windows.net means app is running in Azure with the SQLDB connection string you configured
         if (connectionInfo.ConnectionString.IndexOf("database.windows.net") != -1 &&
             connectionInfo.ConnectionString.IndexOf("Password") == -1)
         {
            connection.AccessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result;
         }

         ob.UseSqlServer(connection);

         _db = new RecordContext(ob.Options);
         _db.Database.Migrate();
      }

      public IEnumerable<Record> ListRecords()
      {
         return _db.Records;
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