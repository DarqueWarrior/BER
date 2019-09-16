[assembly: Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup(typeof(BerService.FunctionApp.Startup))]

namespace BerService.FunctionApp
{
   using AutoMapper;
   using BerService.DAL.Repositories;
   using BerService.Model.Contracts;
   using Microsoft.AspNetCore.Http;
   using Microsoft.AspNetCore.Mvc;
   using Microsoft.Azure.WebJobs;
   using Microsoft.Azure.WebJobs.Extensions.Http;
   using Microsoft.Extensions.Logging;
   using Newtonsoft.Json;
   using System;
   using System.Collections.Generic;
   using System.IO;
   using System.Threading.Tasks;

   public class Records
   {
      private readonly IRepository _repo;

      /// <summary>
      /// Used to map the raw DAL model objects to Contract objects.
      /// The contract objects let us control the version of the API
      /// even when the DAL changes. 
      /// </summary>
      private readonly IMapper _mapper;

      public Records(IRepository repo, IMapper mapper)
      {
         _repo = repo;
         _mapper = mapper;
      }

      [FunctionName("List")]
      public IActionResult List(
       [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Records")]
       HttpRequest req,
       ILogger log)
      {
         try
         {
            log.LogInformation($"Listing records.");

            var records = _repo.ListRecords();

            var list = new List<RecordContract>();
            foreach (var record in records)
            {
               list.Add(_mapper.Map<RecordContract>(record));
            }

            return new OkObjectResult(list);
         }
         catch (Exception e)
         {
            log.LogError(e.Message);
         }

         return new BadRequestObjectResult("Could not get record");
      }

      [FunctionName("Get")]
      public async Task<IActionResult> Get(
             [HttpTrigger(AuthorizationLevel.Function, "get", Route = "Records/{appName}/{dataType}/{version}")]
             HttpRequest req,
             string appName, string dataType, string version,
             ILogger log)
      {
         try
         {
            log.LogInformation($"Getting record {appName}/{dataType}/{version}.");

            var record = await _repo.FindRecord(appName, dataType, version);

            if (record == null)
            {
               return new NotFoundObjectResult($"Could not find {appName}/{dataType}/{version}.");
            }

            return new OkObjectResult(_mapper.Map<RecordContract>(record));
         }
         catch (Exception e)
         {
            log.LogError(e.Message);
         }

         return new BadRequestObjectResult("Could not get record");
      }

      [FunctionName("Post")]
      public async Task<IActionResult> Post(
             [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Records")]
             HttpRequest req,
             ILogger log)
      {
         RecordContract recordContract = null;

         try
         {
            log.LogInformation("Posting a new record.");

            //read json object from request body
            var content = await new StreamReader(req.Body).ReadToEndAsync();
            recordContract = JsonConvert.DeserializeObject<Model.Contracts.RecordContract>(content);

            var record = _mapper.Map<Model.Record>(recordContract);

            var result = await _repo.UpsertRecord(record);

            if (result.DateModified == null)
            {
               var newUri = $"Records/{record.ApplicationName}/{record.DataType}/{record.Version}";
               return new CreatedResult(newUri, _mapper.Map<RecordContract>(record));
            }

            return new OkObjectResult(_mapper.Map<RecordContract>(record));
         }
         catch (Exception e)
         {
            log.LogError(e.Message);
         }

         log.LogWarning($"Problem saving {recordContract?.ApplicationName}/{recordContract?.DataType}/{recordContract?.Version}.");

         return new BadRequestObjectResult("Could not update record");
      }

      [FunctionName("Delete")]
      public async Task<IActionResult> Delete(
             [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "Records/{appName}/{dataType}/{version}")]
             HttpRequest req,
             string appName, string dataType, string version,
             ILogger log)
      {
         try
         {
            log.LogInformation($"Deleting record {appName}/{dataType}/{version}.");

            await _repo.DeleteRecord(appName, dataType, version);

            return new OkObjectResult(null);
         }
         catch (Exception e)
         {
            log.LogError(e.Message);
         }

         log.LogWarning($"Problem deleting record {appName}/{dataType}/{version}.");

         return new BadRequestObjectResult("Could not delete record");
      }
   }
}
