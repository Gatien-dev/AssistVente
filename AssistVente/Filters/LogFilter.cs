using AssistVente.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AssistVente.Filters
{
    public class LogFilter : ActionFilterAttribute, IActionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            { // TODO: Add your acction filter's tasks here

                // Log Action Filter Call
                AssistVenteContext storeDB = new AssistVenteContext();
                var user = HttpContext.Current.User.Identity.Name;

                //var accessedId = string.Empty;
                //if (filterContext.ActionParameters.TryGetValue("id", out object value))
                //{
                //    accessedId = value.ToString();
                //}
                //if (filterContext.ActionDescriptor.ControllerDescriptor.ControllerName == "Home")
                //{
                //    FechGeneralInfos(filterContext, storeDB);
                //}
                //ActionLog log = new ActionLog()
                //{
                //    ID = Guid.NewGuid(),
                //    Controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                //    Action = filterContext.ActionDescriptor.ActionName,
                //    IP = filterContext.HttpContext.Request.UserHostAddress,
                //    DateTime = filterContext.HttpContext.Timestamp,
                //    User = user
                //};
                //if (accessedId != string.Empty)
                //{
                //    log.Action += "/" + accessedId;
                //}
                //storeDB.ActionLogs.Add(log);
                //storeDB.SaveChanges();

            }
            catch (Exception)
            {

                
            }


            this.OnActionExecuting(filterContext);
        }

        private void FechGeneralInfos(ActionExecutingContext filterContext, AssistVenteContext storeDB)
        {
            try
            {

                //(item.DateFinLocation - DateTime.Now).TotalDays < 0
                var expiredLocations = storeDB.Locations.ToList().Where(l => (l.DateFinLocation - DateTime.Now).TotalDays < 0).ToList();
                filterContext.HttpContext.Session.Add("expiredLocations", expiredLocations.Count());
            }
            catch
            {

            }
        }
    }
}