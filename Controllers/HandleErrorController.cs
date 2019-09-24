using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProgrammingChallange.Controllers
{
    /// <summary>
    /// Controller for custom error handling
    /// </summary>
    public class HandleErrorController : Controller
    {
        // GET: HandleError
        /// <summary>
        /// Handles errors that are redirected from other controllers
        /// </summary>
        /// <param name="errorMsg"> The string error message from the previous controller</param>
        /// <returns>The error view with error message</returns>
        public ActionResult Error(string errorMsg)
        {
            ViewData["errorMessage"] = errorMsg;

            return View();
        }
    }
}