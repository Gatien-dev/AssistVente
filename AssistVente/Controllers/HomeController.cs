using AssistVente.Filters;
using AssistVente.Models;
using System;
using System.Web.Mvc;

namespace IdentitySample.Controllers
{
    [Authorize]
    [LogFilter]
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            //Utilities.sendMail(Utilities.generateStockEmailHtml(), "Assist-vente: Etat du stock du :" + DateTime.Now.ToShortDateString());
            return View();
        }

        [HttpGet]
        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
