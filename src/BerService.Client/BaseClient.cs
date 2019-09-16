namespace BerService.Client
{
   using Newtonsoft.Json;
   using System;
   using System.Diagnostics;
   using System.Net.Http;
   using System.Net.Http.Headers;
   using System.Threading.Tasks;

   public abstract class BaseClient
   {
      protected readonly string _version;
      protected readonly string _appName;
      protected readonly string _apiVersion;
      protected readonly string _baseAddress;

      /// <summary>
      /// Constructs a BER Server client to retrieve values.
      /// </summary>
      /// <param name="baseAddress">The base address of the BER Service. Do not include the /api/auth/token</param>
      /// <param name="userAgent">The user agent is passed to the BER Service as identification of the caller.</param>
      /// <param name="version">The version of the value to retrieve.</param>
      /// <param name="appName">The name of the application to search.</param>
      /// <param name="apiVersion">The version of the API of the BER Server to use. Default to 1.0;</param>
      public BaseClient(string baseAddress, string version, string appName, string apiVersion = "1.0")
      {
         this._version = version;
         this._appName = appName;
         this._apiVersion = apiVersion;
         this._baseAddress = baseAddress;
      }

      protected abstract string PrepareClient(HttpClient client, string dataType);

      protected abstract Task<string> PrepareClientAsync(HttpClient client, string dataType);

      /// <summary>
      /// Returns the value found or null if the record was not found.
      /// </summary>
      /// <param name="dataType">The value to retrieve.</param>
      /// <returns>string or null</returns>
      public async Task<string> GetAsync(string dataType)
      {
         string result = null;

         // Use the BER service to lookup the back-end URL to use.
         using (var client = GetClient(_baseAddress))
         {
            var requestUri = await PrepareClientAsync(client, dataType);
            var response = await client.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
               var content = await response.Content.ReadAsStringAsync();
               var record = JsonConvert.DeserializeObject<BerRecord>(content);

               result = record.Value;
            }
            else
            {
               Debug.WriteLine("Could not find back-end service to use");
            }
         }

         return result;
      }

      public string Get(string dataType)
      {
         string result = null;

         // Use the BER service to lookup the back-end URL to use.
         using (var client = GetClient(_baseAddress))
         {
            var requestUri = PrepareClient(client, dataType);
            var response = client.GetAsync(requestUri).Result;

            if (response.IsSuccessStatusCode)
            {
               var content = response.Content.ReadAsStringAsync().Result;
               var record = JsonConvert.DeserializeObject<BerRecord>(content);

               result = record.Value;
            }
            else
            {
               Debug.WriteLine("Could not find back-end service to use");
            }
         }

         return result;
      }

      protected HttpClient GetClient(string url)
      {
         var client = new HttpClient
         {
            MaxResponseContentBufferSize = 256000,
            BaseAddress = new Uri(url)
         };

         // Add an Accept header for JSON format.
         client.DefaultRequestHeaders.Accept.Add(
             new MediaTypeWithQualityHeaderValue("application/json"));

         // Set the api-version to use.
         client.DefaultRequestHeaders.Add("api-version", this._apiVersion);

         return client;
      }
   }
}