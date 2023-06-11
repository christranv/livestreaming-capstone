using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Stream.API.Controllers
{
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}