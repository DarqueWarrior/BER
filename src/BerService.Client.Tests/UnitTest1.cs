using System;
using Xunit;

namespace BerService.Client.Tests
{
   public class UnitTest1
   {
      [Fact]
      public void FunctionTest()
      {
         var target = new BerFunctionClient("https://website4yehpvjvgqcrs.azurewebsites.net",
            "68uRjgDYcBG9kQdkZcTH5mSKkRscLz6vMXOsBW081bkRBxzfZnz6cg==", "1.0", "test");

         var actual = target.Get("test");

         Assert.Equal("pass", actual);
      }

      [Fact]
      public async void FunctionTestAsync()
      {
         var target = new BerFunctionClient("https://website4yehpvjvgqcrs.azurewebsites.net",
            "68uRjgDYcBG9kQdkZcTH5mSKkRscLz6vMXOsBW081bkRBxzfZnz6cg==", "1.0", "test");

         var actual = await target.GetAsync("test");

         Assert.Equal("pass", actual);
      }

      [Fact]
      public void WebAppTest()
      {
         var target = new BerClient("https://website3e3dz42ajqbea.azurewebsites.net",
            "someString", "1.0", "test");

         var actual = target.Get("test");

         Assert.Equal("pass", actual);
      }

      [Fact]
      public async void WebAppTestAsync()
      {
         var target = new BerClient("https://website3e3dz42ajqbea.azurewebsites.net",
            "someString", "1.0", "test");

         var actual = await target.GetAsync("test");

         Assert.Equal("pass", actual);
      }
   }
}
