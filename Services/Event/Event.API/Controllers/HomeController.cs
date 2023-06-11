using Microsoft.AspNetCore.Mvc;

namespace Event.API.Controllers
{
    [Route("/")]
    [ApiController]
    public class HomeController : Controller
    {
        // GET
        [HttpGet]
        public IActionResult Index()
        {
            return Redirect("~/swagger");
        }
    }
}