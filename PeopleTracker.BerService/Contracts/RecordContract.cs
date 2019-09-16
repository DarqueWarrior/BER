namespace PeopleTracker.BerService.Contracts
{
   using System;
   using System.ComponentModel.DataAnnotations;

   /// <summary>
   /// This class is used to map from the DAL to the contract of the API Version.
   /// At first it might feel redundant and easier to just return the model classes
   /// from the DAL. However, if the DAL changes so would the contract. With this 
   /// class you control the contract regardless of how the DAL changes. 
   /// </summary>
   public class RecordContract
   {
      [Required]
      public string ApplicationName { get; set; }

      [Required]
      public string Version { get; set; }

      [Required]
      public string DataType { get; set; }

      [Required]
      public string Value { get; set; }

      public DateTime? DateCreated { get; set; }

      public DateTime? DateModified { get; set; }
   }
}
