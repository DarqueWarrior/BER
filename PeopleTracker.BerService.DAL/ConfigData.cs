namespace PeopleTracker.BerService.Controllers.DAL
{
   /// <summary>
   /// To use this library with a .net core Web API there must be a section 
   /// added to the appsettings.json like the following:
   /// "ConnectionStrings": {
   ///    "DefaultConnection": "<connectionString>"
   /// }
   /// </summary>
   public class ConfigData
   {
      public string DefaultConnection { get; set; }
   }
}
