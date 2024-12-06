using Microsoft.AspNetCore.Mvc;

namespace PschologyProject.Controllers
{
    public class PsychologistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
