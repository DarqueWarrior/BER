namespace PeopleTracker.BerService.Contracts
{
   using System.ComponentModel.DataAnnotations;

   public class CredentialModel
   {
      [Required]
      public string UserAgent { get; set; }
   }
}
