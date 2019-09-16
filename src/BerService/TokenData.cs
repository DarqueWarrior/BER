namespace BerService
{
   /// <summary>
   /// This class is used to read the configuration data needed to create
   /// and validate JWT tokens.
   /// These configuration values are stored in appsettings.json.
   /// </summary>
   public class TokenData
   {
      /// <summary>
      /// Used to create the SigningCredentials for the JWT tokens.
      /// It must be at least 16 characters long. 
      /// </summary>
      public string Key { get; set; }

      public string Issuer { get; set; }

      /// <summary>
      /// The audience of a token is the intended recipient of the token.
      /// The audience value is a string -- typically, the base address of
      /// the resource being accessed, such as https://contoso.com.
      /// </summary>
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
