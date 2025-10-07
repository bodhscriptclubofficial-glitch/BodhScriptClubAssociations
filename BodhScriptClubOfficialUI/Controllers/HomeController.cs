using BodhScriptClubOfficialUI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using static BodhScriptClubOfficialUI.GlobalService.GlobalService;

namespace BodhScriptClubOfficialUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ServiceDeclareMethods _serviceDeclare;
        public string BaseUrl="";
        public HomeController(ILogger<HomeController> logger,IConfiguration configuration,ServiceDeclareMethods serviceDeclare)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceDeclare = serviceDeclare;
            BaseUrl = _configuration["Baseurl"];
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Login()
        {
            ViewBag.ApiBaseUrl = BaseUrl;

            return View();
        }
        [HttpPost]
        public async Task <IActionResult> Login([FromBody] Login login)
        {
            try
            {
                string urlParams = "Credentials"; // controller + method
                var output =  await _serviceDeclare.PostMethod(login, BaseUrl, urlParams);
                if (output=="1")
                {
                    HttpContext.Session.SetString("UserId", login.Username); // example
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    return Json(new { success = true, redirectUrl = Url.Action("DashBoard", "Home") });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid credentials" });

                }

            }
            catch (Exception ex) { 
            
            
            }
            return View();
        }
        [HttpGet("Dashboard")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Dashboard()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Home");
            }

            string urlparametrs = "TotalMembers";
            var resultJson = await _serviceDeclare.GetMethod( BaseUrl, urlparametrs);

            int totalMembers = 0;
            if (!string.IsNullOrEmpty(resultJson))
            {
                var obj = JsonSerializer.Deserialize<Member>(resultJson);
                totalMembers = obj?.TotalMembers ?? 0;
            }

         
            ViewBag.TotalMembers = totalMembers;


            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<IActionResult> Logout()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Redirect to login
            return RedirectToAction("Login", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearSession()
        {
            HttpContext.Session.Clear();
            return Ok();
        }



    }
}
