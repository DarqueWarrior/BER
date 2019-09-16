namespace PeopleTracker.BerService.DAL.Repositories
{
   using PeopleTracker.BerService.DAL.Models;
   using System.Threading.Tasks;

   public interface IRepository
   {
      Task<Record> FindRecord(string applicationName, string dataType, string version);

      Task<int> DeleteRecord(string applicationName, string dataType, string version);

      /// <summary>
      /// Inserts for updates a record.
      /// </summary>
      /// <param name="record"></param>
      Task<Record> UpsertRecord(Record record);
   }
}
