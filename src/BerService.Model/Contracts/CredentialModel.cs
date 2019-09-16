namespace BerService.Model.Contracts
{
   using System.ComponentModel.DataAnnotations;

   public class CredentialModel
   {
      /// <summary>
      /// Stores the magic string to determine if the caller will get a
      /// JWT token back or not. 
      /// </summary>
      [Required]
      public string UserAgent { get; set; }
   }
}
