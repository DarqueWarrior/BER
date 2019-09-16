namespace BerService.Client
{
   using Newtonsoft.Json;
   using System.Net.Http;
   using System.Net.Http.Headers;
   using System.Text;
   using System.Threading.Tasks;

   public class BerClient : BaseClient
   {
      private readonly string _userAgent;
      private const string AuthRequestUri = "/api/auth/token/";

      /// <summary>
      /// Constructs a BER Server client to retrieve values.
      /// </summary>
      /// <param name="baseAddress">The base address of the BER Service. Do not include the /api/auth/token</param>
      /// <param name="userAgent">The user agent is passed to the BER Service as identification of the caller.</param>
      /// <param name="version">The version of the value to retrieve.</param>
      /// <param name="appName">The name of the application to search.</param>
      /// <param name="apiVersion">The version of the API of the BER Server to use. Default to 1.0;</param>
      public BerClient(string baseAddress, string userAgent, string version, string appName, string apiVersion = "1.0") :
         base(baseAddress, version, appName, apiVersion)
      {
         this._userAgent = userAgent;
      }

      protected override string PrepareClient(HttpClient client, string dataType)
      {
         var token = this.Auth();
         client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.Token);
         return $"/api/records/{this._appName}/{dataType}/{this._version}";
      }

      protected override async Task<string> PrepareClientAsync(HttpClient client, string dataType)
      {
         var token = await this.AuthAsync();
         client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token.Token);
         return $"/api/records/{this._appName}/{dataType}/{this._version}";
      }

      private BerToken Auth()
      {
         // Get the JWT from the BER service
         using (var client = this.GetClient(_baseAddress))
         {
            var body = new StringContent($"{{\"UserAgent\": \"{_userAgent}\"}}", Encoding.UTF8, "application/json");

            var response = client.PostAsync(AuthRequestUri, body).Result;

            var tokenJson = response.Content.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<BerToken>(tokenJson);
         }
      }

      private async Task<BerToken> AuthAsync()
      {
         // Get the JWT from the BER service
         using (var client = this.GetClient(_baseAddress))
         {
            var body = new StringContent($"{{\"UserAgent\": \"{_userAgent}\"}}", Encoding.UTF8, "application/json");

            var response = await client.PostAsync(AuthRequestUri, body);

            var tokenJson = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<BerToken>(tokenJson);
         }
      }
   }
}