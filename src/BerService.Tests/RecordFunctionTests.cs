namespace BerService.Tests
{
   using AutoMapper;
   using BerService.DAL.Repositories;
   using BerService.FunctionApp;
   using BerService.Model.Contracts;
   using BerService.Tests.mocks;
   using Microsoft.AspNetCore.Mvc;
   using Microsoft.Extensions.Logging;
   using NSubstitute;
   using System;
   using System.Collections.Generic;
   using System.IO;
   using System.Threading.Tasks;
   using Xunit;

   public class RecordFunctionTests
   {
      private readonly string _record = "{'ApplicationName': 'appName', 'Version': '1.0', 'DataType': 'api', 'Value': 'test'}";

      [Fact]
      [Trait("Category", "FunctionTests")]
      public void List()
      {
         // Arrange
         var mapper = Substitute.For<IMapper>();
         var repo = Substitute.For<IRepository>();
         repo.ListRecords().Returns(x =>
         {
            return new List<Model.Record>() {
               new Model.Record {
                  ApplicationName = "appName",
                  DataType = "dataType",
                  Value = "test",
                  Version = "1.0",
               }
            };
         });
         var logger = Substitute.For<ILogger<Records>>();

         var target = new Records(repo, mapper);

         // Act
         var result = target.List(null, logger);

         // Assert
         Assert.IsType<OkObjectResult>(result);
      }

      [Fact]
      [Trait("Category", "FunctionTests")]
      public void List_Throws()
      {
         // Arrange
         var mapper = Substitute.For<IMapper>();
         var repo = Substitute.For<IRepository>();
         repo.ListRecords().Returns(x => { throw new System.Exception("Boom"); });
         var logger = Substitute.For<ILogger<Records>>();

         var target = new Records(repo, mapper);

         // Act
         var result = target.List(null, logger);

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
      [Trait("Category", "FunctionTests")]
      public void Get_NotFound()
      {
         // Arrange
         var mapper = Substitute.For<IMapper>();
         var repo = Substitute.For<IRepository>();
         var logger = Substitute.For<ILogger<Records>>();

         var target = new Records(repo, mapper);

         // Act
         var result = target.Get(null, "testAppName", "dataType", "version1", logger).Result;

         // Assert
         Assert.IsType<NotFoundObjectResult>(result);
      }

      [Fact]
      [Trait("Category", "FunctionTests")]
      public void Get_Throws()
      {
         // Arrange
         var mapper = Substitute.For<IMapper>();
         var repo = Substitute.For<IRepository>();
         repo.FindRecord(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromException<Model.Record>(new System.Exception("Boom")));
         var logger = Substitute.For<ILogger<Records>>();

         var target = new Records(repo, mapper);

         // Act
         var result = target.Get(null, "testAppName", "dataType", "version1", logger).Result;

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
      [Trait("Category", "FunctionTests")]
      public void Get_Ok()
      {
         // Arrange
         var expectedCreatedDate = DateTime.UtcNow;
         var expectedModifiedDate = DateTime.UtcNow.AddMinutes(1);
         var configuration = new MapperConfiguration(cfg =>
         {
            cfg.CreateMap<Model.Record, Model.Contracts.RecordContract>();
         });
         var mapper = configuration.CreateMapper();

         var repo = Substitute.For<IRepository>();
         repo.FindRecord(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(new Model.Record
            {
               ApplicationName = "Unit Tests",
               DataType = "Test Result",
               DateCreated = expectedCreatedDate,
               DateModified = expectedModifiedDate,
               Version = "1.0",
               Value = "Pass"
            });
         var logger = Substitute.For<ILogger<Records>>();

         var target = new Records(repo, mapper);

         // Act
         var result = target.Get(null, "testAppName", "dataType", "version1", logger).Result;

         // Assert
         Assert.IsType<OkObjectResult>(result);
         Assert.Equal("Pass", ((Model.Contracts.RecordContract)((OkObjectResult)result).Value).Value);
         Assert.Equal(expectedCreatedDate, ((Model.Contracts.RecordContract)((OkObjectResult)result).Value).DateCreated);
         Assert.Equal(expectedModifiedDate, ((Model.Contracts.RecordContract)((OkObjectResult)result).Value).DateModified);
      }

      [Fact]
      [Trait("Category", "FunctionTests")]
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

         var req = new MockHttpRequest
         {
            Body = RecordFunctionTests.GenerateStreamFromString(_record)
         };

         var mapper = Substitute.For<IMapper>();
         var repo = Substitute.For<IRepository>();
         repo.UpsertRecord(Arg.Any<Model.Record>()).Returns(Task.FromException<Model.Record>(new System.Exception("Boom")));
         var logger = Substitute.For<ILogger<Records>>();

         var target = new Records(repo, mapper);

         // Act
         var result = target.Post(req, logger).Result;

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
      [Trait("Category", "FunctionTests")]
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

         var req = new MockHttpRequest
         {
            Body = RecordFunctionTests.GenerateStreamFromString(_record)
         };
         var mapper = Substitute.For<IMapper>();
         var repo = Substitute.For<IRepository>();
         repo.UpsertRecord(Arg.Any<Model.Record>()).Returns(Task.FromResult<Model.Record>(new Model.Record
         {
            DateModified = DateTime.Now
         }));

         var logger = Substitute.For<ILogger<Records>>();

         var target = new Records(repo, mapper);

         // Act
         var result = target.Post(req, logger).Result;

         // Assert
         Assert.IsType<OkObjectResult>(result);
      }

      [Fact]
      [Trait("Category", "FunctionTests")]
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

         var record = new Model.Record
         {
            ApplicationName = "testAppName",
            DataType = "dataType",
            Version = "version1",
            Value = "value"
         };

         var req = new MockHttpRequest
         {
            Body = RecordFunctionTests.GenerateStreamFromString(_record)
         };

         var mapper = Substitute.For<IMapper>();
         mapper.Map<Model.Record>(Arg.Any<RecordContract>()).Returns(record);

         var repo = Substitute.For<IRepository>();
         repo.UpsertRecord(Arg.Any<Model.Record>()).Returns(Task.FromResult<Model.Record>(record));

         var logger = Substitute.For<ILogger<Records>>();

         var target = new Records(repo, mapper);

         // Act
         var result = target.Post(req, logger).Result;

         // Assert
         Assert.IsType<CreatedResult>(result);
      }

      [Fact]
      [Trait("Category", "FunctionTests")]
      public void Delete()
      {
         // Arrange
         var mapper = Substitute.For<IMapper>();
         var repo = Substitute.For<IRepository>();
         repo.DeleteRecord(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromResult<int>(1));

         var logger = Substitute.For<ILogger<Records>>();

         var target = new Records(repo, mapper);

         // Act
         var result = target.Delete(null, "testAppName", "dataType", "version1", logger).Result;

         // Assert
         Assert.IsType<OkObjectResult>(result);
      }

      [Fact]
      [Trait("Category", "FunctionTests")]
      public void Delete_Throws()
      {
         // Arrange
         var mapper = Substitute.For<IMapper>();
         var repo = Substitute.For<IRepository>();
         repo.DeleteRecord(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(Task.FromException<int>(new System.Exception("Boom")));

         var logger = Substitute.For<ILogger<Records>>();

         var target = new Records(repo, mapper);

         // Act
         var result = target.Delete(null, "testAppName", "dataType", "version1", logger).Result;

         // Assert
         logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString() == "Boom"),
            null,
            Arg.Any<Func<object, Exception, string>>());

         Assert.IsType<BadRequestObjectResult>(result);
      }

      public static Stream GenerateStreamFromString(string s)
      {
         var stream = new MemoryStream();
         var writer = new StreamWriter(stream);
         writer.Write(s);
         writer.Flush();
         stream.Position = 0;
         return stream;
      }
   }
}
