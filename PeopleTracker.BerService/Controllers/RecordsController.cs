namespace PeopleTracker.BerService.Controllers
{
   using AutoMapper;
   using Microsoft.AspNetCore.Authentication.JwtBearer;
   using Microsoft.AspNetCore.Authorization;
   using Microsoft.AspNetCore.Mvc;
   using Microsoft.Extensions.Logging;
   using PeopleTracker.BerService.Contracts;
   using PeopleTracker.BerService.DAL.Models;
   using PeopleTracker.BerService.DAL.Repositories;
   using PeopleTracker.BerService.Filters;
   using System.Threading.Tasks;

   /// <summary>
   /// The main controller of the BER service
   /// </summary>
   [Route("api/[controller]")]
   [ApiController]
   [ValidateModel]
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
   public class RecordsController : ControllerBase
   {
      private readonly IRepository _repo;
      private readonly ILogger<RecordsController> _logger;

      /// <summary>
      /// Used to map the raw DAL model objects to Contract objects.
      /// The contract objects let us control the version of the API
      /// even when the DAL changes. 
      /// </summary>
      private readonly IMapper _mapper;

      public RecordsController(IRepository repo, ILogger<RecordsController> logger, IMapper mapper)
      {
         _repo = repo;
         _logger = logger;
         _mapper = mapper;
      }

      /// <summary>
      /// Returns a Record object the value populated.
      /// GET api/<controller>/appName/dataType/version
      /// </summary>
      /// <param name="appName">The name of the application to find.</param>
      /// <param name="dataType">The data type to find.</param>
      /// <param name="version">The version of the application calling this method.</param>
      /// <returns>Record</returns>
      [HttpGet("{appName}/{dataType}/{version}", Name = "RecordGet")]
      public async Task<IActionResult> Get(string appName, string dataType, string version)
      {
         try
         {
            _logger.LogInformation($"Getting record {appName}/{dataType}/{version}.");

            var record = await _repo.FindRecord(appName, dataType, version);

            if(record == null)
            {
               return NotFound($"Could not find {appName}/{dataType}/{version}.");
            }

            return Ok(_mapper.Map<RecordContract>(record));
         }
         catch (System.Exception e)
         {
            _logger.LogError(e.Message);
         }

         return BadRequest("Could not get record");
      }

      /// <summary>
      /// POST api/<controller>
      /// There is only ever one row per version of data. The composite
      /// key is application name, data type and version. This action 
      /// performs and Update if the item exist or an Insert if the item
      /// does not. We call this an UpSert. 
      /// </summary>
      /// <param name="record">the record to create or update</param>
      /// <returns></returns>
      [HttpPost]
      public async Task<IActionResult> Post(RecordContract recordContract)
      {
         try
         {
            _logger.LogInformation("Posting a new record.");

            var record = _mapper.Map<Record>(recordContract);

            var result = await _repo.UpsertRecord(record);

            if (result.DateModified == null)
            {
               var newUri = Url.Link("RecordGet", new { appName = record.ApplicationName, dataType = record.DataType, version = record.Version });
               return Created(newUri, _mapper.Map<RecordContract>(record));
            }

            return Ok(_mapper.Map<RecordContract>(result));
         }
         catch (System.Exception e)
         {
            _logger.LogError(e.Message);
         }

         _logger.LogWarning($"Problem saving {recordContract.ApplicationName}/{recordContract.DataType}/{recordContract.Version}.");

         return BadRequest("Could not update record");
      }

      /// <summary>
      ///  DELETE api/<controller>/appName/dataType/version
      /// </summary>
      /// <param name="appName"></param>
      /// <param name="dataType"></param>
      /// <param name="version"></param>
      /// <returns></returns>
      [HttpDelete("{appName}/{dataType}/{version}")]
      public async Task<IActionResult> Delete(string appName, string dataType, string version)
      {
         try
         {
            _logger.LogInformation($"Deleting record {appName}/{dataType}/{version}.");

            await _repo.DeleteRecord(appName, dataType, version);

            return Ok();
         }
         catch (System.Exception e)
         {
            _logger.LogError(e.Message);
         }

         _logger.LogWarning($"Problem deleting record {appName}/{dataType}/{version}.");

         return BadRequest("Could not delete record");
      }
   }
}