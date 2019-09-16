namespace BerService.Client
{
   using System.Net.Http;
   using System.Threading.Tasks;

   public class BerFunctionClient : BaseClient
   {
      private readonly string _functionKey;

      /// <summary>
      /// Constructs a BER Server client to retrieve values.
      /// </summary>
      /// <param name="baseAddress">The base address of the BER Service. Do not include the /api/auth/token</param>
      /// <param name="functionKey">The function key for calling an Azure function.</param>
      /// <param name="version">The version of the value to retrieve.</param>
      /// <param name="appName">The name of the application to search.</param>
      /// <param name="apiVersion">The version of the API of the BER Server to use. Default to 1.0;</param>
      public BerFunctionClient(string baseAddress, string functionKey, string version, string appName, string apiVersion = "1.0") :
         base(baseAddress, version, appName, apiVersion)
      {
         this._functionKey = functionKey;
      }

      protected override string PrepareClient(HttpClient client, string dataType)
      {
         return $"/api/records/{this._appName}/{dataType}/{this._version}?code={this._functionKey}";
      }

      protected override Task<string> PrepareClientAsync(HttpClient client, string dataType)
      {
         return Task.FromResult<string>($"/api/records/{this._appName}/{dataType}/{this._version}?code={this._functionKey}");
      }
   }
}