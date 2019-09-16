namespace PeopleTracker.BerService
{
   /// <summary>
   /// This class is used to read the configuration data needed to create
   /// and validate JWT tokens.
   /// </summary>
   public class TokenData
   {
      public string Key { get; set; }
      public string Issuer { get; set; }
      public string Audience { get; set; }

      /// <summary>
      /// The magic string supplied by the Mobile application to get a token.
      /// For a production solution this needs to read values from
      /// a data store and use proper authentication. 
      /// </summary>
      public string Mobile { get; set; }

      /// <summary>
      /// The magic string supplied by the Web application to get a token.
      /// For a production solution this needs to read values from
      /// a data store and use proper authentication. 
      /// </summary>
      public string Web { get; set; }
   }
}
