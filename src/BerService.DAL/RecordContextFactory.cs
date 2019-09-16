namespace BerService.DAL
{
   using Microsoft.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore.Design;

   public class RecordContextFactory : IDesignTimeDbContextFactory<RecordContext>
   {
      public RecordContext CreateDbContext(string[] args)
      {
         var ob = new DbContextOptionsBuilder<RecordContext>();
         ob.UseSqlServer("Data Source=localhost\\sqlexpress;Initial Catalog=berRecords;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

         return new RecordContext(ob.Options);
      }
   }
}
