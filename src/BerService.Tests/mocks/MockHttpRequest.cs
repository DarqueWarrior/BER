namespace BerService.Tests.mocks
{
   using Microsoft.AspNetCore.Http;
   using System;
   using System.IO;
   using System.Threading;
   using System.Threading.Tasks;

   internal class MockHttpRequest : HttpRequest
   {
      public override HttpContext HttpContext => throw new NotImplementedException();

      public override string Method { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public override string Scheme { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public override bool IsHttps { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public override HostString Host { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public override PathString PathBase { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public override PathString Path { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public override QueryString QueryString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public override IQueryCollection Query { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public override string Protocol { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

      public override IHeaderDictionary Headers => throw new NotImplementedException();

      public override IRequestCookieCollection Cookies { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public override long? ContentLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public override string ContentType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
      public override Stream Body { get; set; }

      public override bool HasFormContentType => throw new NotImplementedException();

      public override IFormCollection Form { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

      public override Task<IFormCollection> ReadFormAsync(CancellationToken cancellationToken = default)
      {
         throw new NotImplementedException();
      }
   }
}
