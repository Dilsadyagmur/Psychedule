using Psy.DAL.Repositories.Abstract;
using Microsoft.AspNetCore.Mvc;
using Psy.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace PschologyProject.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PsychologistsController : Controller
    {
        private readonly IUnitOfWork _psycDb;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;


        public PsychologistsController(IUnitOfWork unitOfWork, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _psycDb = unitOfWork;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();

        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new
            {
                data = _psycDb.Psychologists.GetAll(p => p.User.IsDeleted == false).Select(p => new
                {
                    p.User.Id,
                    p.User.UserName,
                    p.User.Email,
                    p.User.PhoneNumber,
                }

            ).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Add(AppUser user, Psychologists psychologists)
        {

            var username = user.UserName.Replace(" ", "");
            var newUser = new AppUser()
            {
                UserName = username,
                Email = user.Email,
            };

            var res = await _userManager.CreateAsync(user, "Admin1234");

            if (res.Succeeded)
            {
                psychologists.UserId = user.Id;
                _psycDb.Psychologists.Add(psychologists);
                _psycDb.Save();
            }
            else
            {
                return Content("Error");
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult Delete(int psychologistid)
        {
            _psycDb.Users.DeleteById(psychologistid);

            _psycDb.Save();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user==null)
            {
                return BadRequest();
            }
             return View(user);
        }

    }
}
