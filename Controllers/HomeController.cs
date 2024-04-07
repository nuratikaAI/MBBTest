using MBBTest_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace MBBTest_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        string? apiBaseUrl = "";

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            apiBaseUrl = _configuration.GetValue<string>("WebAPIBaseUrl");
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult MemberRegistration()
        {
            return View();
        }

        public IActionResult MemberList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MemberRegistration(MemberModel freelancer)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7229/api/Freelancers");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<MemberModel>("Freelancers", freelancer);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(freelancer);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            List<MemberModel> EmpInfo = new List<MemberModel>();
            using (var client = new HttpClient())
            {
                if (apiBaseUrl == null)
                {
                    throw new Exception("Unable to Connect With Web API");
                }

                //Passing service base url
                client.BaseAddress = new Uri(apiBaseUrl);
                client.DefaultRequestHeaders.Clear();
                //Define request data format
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient
                HttpResponseMessage Res = await client.GetAsync("Freelancers");
                //Checking the response is successful or not which is sent using HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;
                    //Deserializing the response recieved from web api and storing into the Employee list
                    EmpInfo = JsonConvert.DeserializeObject<List<MemberModel>>(EmpResponse);
                }
                //returning the employee list to view
                return View(EmpInfo);
            }
        }

    }
}
