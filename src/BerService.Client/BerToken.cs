namespace BerService.Client
{
   using System;

   public class BerToken
   {
      /// <summary>
      /// JWT Token returned from BER Server
      /// </summary>
      public string Token { get; set; }

      /// <summary>
      /// Expiration date of the token
      /// </summary>
      public DateTime Expiration { get; set; }
   }
}
