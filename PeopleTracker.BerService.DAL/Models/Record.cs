namespace PeopleTracker.BerService.DAL.Models
{
   using System;

   public class Record
   {
      public string ApplicationName { get; set; }
      public string Version { get; set; }
      public string DataType { get; set; }
      public string Value { get; set; }
      public DateTime? DateCreated { get; set; }
      public DateTime? DateModified { get; set; }
   }
}
