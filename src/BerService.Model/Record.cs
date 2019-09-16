namespace BerService.Model
{
   using System;

   /// <summary>
   /// This class defines the records stored in the BER service. Any
   /// change to this class will require a new EF migration be created
   /// to update the DAL.
   /// </summary>
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
