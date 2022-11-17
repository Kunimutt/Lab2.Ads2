using Lab2.Ads2.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace Lab2.Ads2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SelectAnnons()
        {
            List<Annons> annonser = new List<Annons>();
            AdsMetoder am = new AdsMetoder();
            string error = "";
            annonser = am.SelectAnnonsLista(out error);
            foreach (Annons annons in annonser)
            {
                if (annons.ad_pris == 40)
                {
                    annons.al_foretag = true;
                }
            }

            ViewBag.error = error;
            return View(annonser);
        }

      

        //Radio är du prenumerant?
        [HttpGet]
        public IActionResult PrenCheck()
        {
            return View();
        }
        //Radio är du prenumerant?
        [HttpPost]
        public IActionResult PrenCheck(bool val)
        {
            if (val == true)
            {
                return RedirectToAction("PrenNrCheck");
            }
            else
            {
                return RedirectToAction("BusinessForm");
            }
        }

        //Formulär fyll i prennr
        [HttpGet]
        public IActionResult PrenNrCheck()
        {
            return View();
        }
       

        //Formulär fyll i prennr, GET mot API
        [HttpPost]
        public async Task<IActionResult> PrenNrCheck(int id)
        {
            ViewModelAdsPren wmap = new ViewModelAdsPren();
            HttpClient client = new();
            client.BaseAddress = new Uri("https://localhost:7124/");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));



            HttpResponseMessage responseMessage = await client.GetAsync("Prenumerant/id?id=" + id);
            responseMessage.EnsureSuccessStatusCode();

            if (responseMessage.IsSuccessStatusCode)
            {
                string apiResponse = await responseMessage.Content.ReadAsStringAsync();
                wmap.pren = JsonConvert.DeserializeObject<Pren>(apiResponse);

                if (wmap.pren.pr_fnamn == null)
                {
                    return RedirectToAction("PrenNrCheckFail");
                }
                else
                {
                    return View("PrenForm", wmap);
                }
            }

            return View();
        }

        public IActionResult PrenNrCheckFail()
        {
            return View();

        }

        //Annonsformulär som delvis ska autofyllas i med p
        [HttpGet]
        public IActionResult PrenForm(Pren pren)
        {
            return View(pren);
        }

        //OBS FUNGERAR EJ I NULÄGET
        [HttpPost]
        //public async Task<IActionResult> PrenForm(ViewModelAdsPren wmap)
        public async Task<IActionResult> PrenForm(Pren pren, Annons annons)
        {
            //int id = pren.pr_prennr;
            HttpClient client = new();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //string data = JsonConvert.SerializeObject(wmap.pren);
            //StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
            StringContent content = new StringContent(JsonConvert.SerializeObject(pren), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync("https://localhost:7124/Prenumerant/id?id=" + pren.pr_prennr, content);
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                string apiResponse = await response.Content.ReadAsStringAsync();

            }
            else
            {
                return RedirectToAction("PrenFormApiFail");
            }
            AdsMetoder am = new AdsMetoder();
            int i = 0;
            string error = "";

            i = am.InsertPrenAnnons(annons, out error);
            if (error == "")
            {
                return RedirectToAction("PrenFormSuccess");
            }
            else
            {
                return RedirectToAction("PrenFormFail");
            }

        }
        //Tidigare Annonsformulär som delvis ska fyllas i med p
        //[HttpGet]
        //public async Task<IActionResult> PrenForm(Pren p)
        //{

        //    return View();
        //}

        public IActionResult PrenFormApiFail()
        {
            return View();

        }

        public IActionResult PrenFormSuccess()
        {
            return View();

        }

        public IActionResult PrenFormFail()
        {
            return View();
        }
        [HttpGet]
        public IActionResult BusinessForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult BusinessForm(Annons annons, Annonsor annonsor)
        {
            AdsMetoder am = new AdsMetoder();
            int i = 0;
            string error = "";
            i = am.InsertAnnonsorAndAnnons(annons, annonsor, out error);

            if (error == "")
            {
                return RedirectToAction("BusinessFormSuccess");
            }
            else
            {
                return RedirectToAction("BusinessFormFail");
            }

        }

        public IActionResult BusinessFormSuccess()
        {
            return View();
        }

        public IActionResult BusinessFormFail()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}