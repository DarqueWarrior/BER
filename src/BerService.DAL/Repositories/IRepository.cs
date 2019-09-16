namespace BerService.DAL.Repositories
{
   using BerService.Model;
   using System.Collections.Generic;
   using System.Threading.Tasks;

   public interface IRepository
   {
      IEnumerable<Record> ListRecords();

      Task<Record> FindRecord(string applicationName, string dataType, string version);

      Task<int> DeleteRecord(string applicationName, string dataType, string version);

      /// <summary>
      /// Inserts for updates a record.
      /// </summary>
      /// <param name="record"></param>
      Task<Record> UpsertRecord(Record record);
   }
}
