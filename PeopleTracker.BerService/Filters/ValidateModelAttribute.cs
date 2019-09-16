namespace PeopleTracker.BerService.Filters
{
   using Microsoft.AspNetCore.Mvc;
   using Microsoft.AspNetCore.Mvc.Filters;

   /// <summary>
   /// This attribute removes the need to check model state in each action. 
   /// </summary>
   public class ValidateModelAttribute : ActionFilterAttribute
   {
      public override void OnActionExecuting(ActionExecutingContext context)
      {
         base.OnActionExecuting(context);

         if (!context.ModelState.IsValid)
         {
            context.Result = new BadRequestObjectResult(context.ModelState);
         }
      }
   }
}
