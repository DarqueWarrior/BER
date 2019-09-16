namespace PeopleTracker.BerService.DAL.Models
{
   using Microsoft.EntityFrameworkCore;

   public class RecordContext : DbContext
   {
      public RecordContext(DbContextOptions<RecordContext> options)
          : base(options)
      {
      }

      /// <summary>
      /// The only way to configure a composite key with EF core is with the fluent API
      /// </summary>
      /// <param name="modelBuilder"></param>
      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<Record>().HasKey(r => new { r.ApplicationName, r.DataType, r.Version });
      }

      public DbSet<Record> Records { get; set; }
   }
}
