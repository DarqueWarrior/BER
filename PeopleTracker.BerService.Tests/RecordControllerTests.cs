namespace PeopleTracker.BerService.Tests
{
   using AutoMapper;
   using Microsoft.AspNetCore.Mvc;
   using Microsoft.Extensions.Logging;
   using NSubstitute;
   using PeopleTracker.BerService.Contracts;
   using PeopleTracker.BerService.Controllers;
   using PeopleTracker.BerService.DAL.Repositories;
   using System;
   using System.Threading.Tasks;
   using Xunit;

   public class RecordControllerTests
   {
      [Fact]
      public void Get_NotFound()
      {
         // Arrange
         var mapper = Substitute.For<IMapper>();
         var repo = Substitute.For<IRepository>();
         var logger = Substitute.For<ILogger<RecordsController>>();

         var target = new RecordsController(repo, logger, mapper);

         // Act
         var result = target.Get("testAppName", "dataType", "version1").Result;

         // Assert
         Assert.IsType<NotFoundObjectResult>(result);
      }

      [Fact]
      public void Get_Throws()
      {
         // Arrange
         var mapper = Substitute.For<IMapper>();
         var repo = Substitute.For<IRepository>();
         repo.FindRecord(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromException<DAL.Models.Record>(new System.Exception("Boom")));
         var logger = Substitute.For<ILogger<RecordsController>>();

         var target = new RecordsController(repo, logger, mapper);

         // Act
         var result = target.Get("testAppName", "dataType", "version1").Result;

         // Assert
         logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString() == "Boom"),
            null,
            Arg.Any<Func<object, Exception, string>>());

         Assert.IsType<BadRequestObjectResult>(result);
      }

      [Fact]
      public void Get_Ok()
      {
         // Arrange
         var mapper = Substitute.For<IMapper>();
         var repo = Substitute.For<IRepository>();
         repo.FindRecord(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(new DAL.Models.Record());
         var logger = Substitute.For<ILogger<RecordsController>>();

         var target = new RecordsController(repo, logger, mapper);

         // Act
         var result = target.Get("testAppName", "dataType", "version1").Result;

         // Assert
         Assert.IsType<OkObjectResult>(result);
      }

      [Fact]
      public void Post_Throws()
      {
         // Arrange
         var recordContract = new RecordContract
         {
            ApplicationName = "testAppName",
            DataType = "dataType",
            Version = "version1",
            Value = "value",
            DateCreated = DateTime.UtcNow,
            DateModified = DateTime.UtcNow
         };

         var mapper = Substitute.For<IMapper>();
         var repo = Substitute.For<IRepository>();
         repo.UpsertRecord(Arg.Any<DAL.Models.Record>()).Returns(Task.FromException<DAL.Models.Record>(new System.Exception("Boom")));
         var logger = Substitute.For<ILogger<RecordsController>>();

         var target = new RecordsController(repo, logger, mapper);

         // Act
         var result = target.Post(recordContract).Result;

         // Assert
         logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString() == "Boom"),
            null,
            Arg.Any<Func<object, Exception, string>>());

         Assert.IsType<BadRequestObjectResult>(result);
      }

      [Fact]
      public void Post_Update()
      {
         // Arrange
         var recordContract = new RecordContract
         {
            ApplicationName = "testAppName",
            DataType = "dataType",
            Version = "version1",
            Value = "value",
            DateCreated = DateTime.UtcNow,
            DateModified = DateTime.UtcNow
         };

         var mapper = Substitute.For<IMapper>();
         var repo = Substitute.For<IRepository>();
         repo.UpsertRecord(Arg.Any<DAL.Models.Record>()).Returns(Task.FromResult<DAL.Models.Record>(new DAL.Models.Record
         {
            DateModified = DateTime.Now
         }));

         var logger = Substitute.For<ILogger<RecordsController>>();

         var target = new RecordsController(repo, logger, mapper);

         // Act
         var result = target.Post(recordContract).Result;

         // Assert
         Assert.IsType<OkObjectResult>(result);
      }

      [Fact]
      public void Post_Insert()
      {
         // Arrange
         var recordContract = new RecordContract
         {
            ApplicationName = "testAppName",
            DataType = "dataType",
            Version = "version1",
            Value = "value"
         };

         var record = new DAL.Models.Record
         {
            ApplicationName = "testAppName",
            DataType = "dataType",
            Version = "version1",
            Value = "value"
         };

         var mapper = Substitute.For<IMapper>();
         mapper.Map<DAL.Models.Record>(Arg.Any<RecordContract>()).Returns(record);

         var repo = Substitute.For<IRepository>();
         repo.UpsertRecord(Arg.Any<DAL.Models.Record>()).Returns(Task.FromResult<DAL.Models.Record>(record));

         var logger = Substitute.For<ILogger<RecordsController>>();

         var target = new RecordsController(repo, logger, mapper)
         {
            Url = Substitute.For<IUrlHelper>()
         };

         // Act
         var result = target.Post(recordContract).Result;

         // Assert
         Assert.IsType<CreatedResult>(result);
      }

      [Fact]
      public void Delete()
      {
         // Arrange
         var mapper = Substitute.For<IMapper>();
         var repo = Substitute.For<IRepository>();
         repo.DeleteRecord(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<int>(1));

         var logger = Substitute.For<ILogger<RecordsController>>();

         var target = new RecordsController(repo, logger, mapper);

         // Act
         var result = target.Delete("testAppName", "dataType", "version1").Result;

         // Assert
         Assert.IsType<OkResult>(result);
      }

      [Fact]
      public void Delete_Throws()
      {
         // Arrange
         var mapper = Substitute.For<IMapper>();
         var repo = Substitute.For<IRepository>();
         repo.DeleteRecord(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromException<int>(new System.Exception("Boom")));

         var logger = Substitute.For<ILogger<RecordsController>>();

         var target = new RecordsController(repo, logger, mapper);

         // Act
         var result = target.Delete("testAppName", "dataType", "version1").Result;

         // Assert
         logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString() == "Boom"),
            null,
            Arg.Any<Func<object, Exception, string>>());

         Assert.IsType<BadRequestObjectResult>(result);
      }
   }
}