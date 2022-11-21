using Lab2.Ads2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace Lab2.Ads2.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        private readonly IWebHostEnvironment _hostEnvironment;
        public HomeController(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
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
            var valutor = Valutor.GetAll();
            var model = new ViewModelAdsAnnonsor();
            model.ValutorSelectList = new List<SelectListItem>();
            foreach (var valuta in valutor)
            {
                model.ValutorSelectList.Add(new SelectListItem { Text = valuta.valutanamn, Value = valuta.valutanamn });
            }
            return View(model);
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

        public async Task<IActionResult> ExportPren()
        {
            List<Pren> pren = new List<Pren>();
            HttpClient client = new();
            client.BaseAddress = new Uri("https://localhost:7124/");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage responseMessage = await client.GetAsync("Prenumerant");
            responseMessage.EnsureSuccessStatusCode();

            if (responseMessage.IsSuccessStatusCode)
            {
                string apiResponse = await responseMessage.Content.ReadAsStringAsync();
                pren = JsonConvert.DeserializeObject<List<Pren>>(apiResponse);

                //CreateXMLFile();
                AddRecordtoXMLfile(pren);
                
                //AddRecordtoXMLfileManualInput();

                return View(pren);
            }
            return RedirectToAction("ExportPrenFail");
        }

        public IActionResult ExportPrenFail()
        {
            return View();
        }
        //Rensar Xml-fil vid start av export av data till fil i metoden AddRecordsXMLfile      

        static void DeleteXMLRecords()
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\Users\bosa0003\source\repos\Lab2.Ads2\Lab2.Ads2\Prenumeranter.xml");
            var root = doc.SelectSingleNode("Prenumeranter");
            root.RemoveAll();
            doc.Save(@"C:\Users\bosa0003\source\repos\Lab2.Ads2\Lab2.Ads2\Prenumeranter.xml");

        }

        //Skapar ursprunglig Xml-fil för användare/prenumeranter Prenumeranter.xml används bara en gång
        //eller för att skapa xml-filer att använda för test av import-funktion ex importfile3      

        static void CreateXMLFile()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Prenumeranter");
            doc.AppendChild(root);
            //doc.Save(@"C:\Users\bosa0003\source\repos\Lab2.Ads2\Lab2.Ads2\Demofil.xml");
            doc.Save(@"C:\Users\bosa0003\source\repos\Lab2.Ads2\Lab2.Ads2\Prenumeranter.xml");

        }

        //Läser av lista alla prenumeranter och lägger till det i Xml-fil user.xml      

        static void AddRecordtoXMLfile(List<Pren> p)
        {
            DeleteXMLRecords();
            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\Users\bosa0003\source\repos\Lab2.Ads2\Lab2.Ads2\Prenumeranter.xml");
            XmlNode root = doc.SelectSingleNode("Prenumeranter");
            foreach (var item in p)
            {
                XmlElement pren = doc.CreateElement("Prenumerant");
                root.AppendChild(pren);
                XmlAttribute id = doc.CreateAttribute("id");
                id.Value = doc.SelectNodes("Prenumeranter/Prenumerant").Count.ToString();
                pren.Attributes.Append(id);
                XmlElement prennr = doc.CreateElement("Prenumerationsnummer");
                prennr.InnerText = item.pr_prennr.ToString();
                pren.AppendChild(prennr);
                XmlElement fnamn = doc.CreateElement("Förnamn");
                fnamn.InnerText = item.pr_fnamn;
                pren.AppendChild(fnamn);
                XmlElement enamn = doc.CreateElement("Efternamn");
                enamn.InnerText = item.pr_efternamn;
                pren.AppendChild(enamn);
                XmlElement persnr = doc.CreateElement("Personnummer");
                persnr.InnerText = item.pr_personnr.ToString();
                pren.AppendChild(persnr);
                XmlElement telnr = doc.CreateElement("Telefonnummer");
                telnr.InnerText = item.pr_telefonnr;
                pren.AppendChild(telnr);
                XmlElement utadress = doc.CreateElement("Utdelningsadress");
                utadress.InnerText = item.pr_utadress;
                pren.AppendChild(utadress);
                XmlElement postnr = doc.CreateElement("Postnummer");
                postnr.InnerText = item.pr_postnr.ToString();
                pren.AppendChild(postnr);
                XmlElement ort = doc.CreateElement("Ort");
                ort.InnerText = item.pr_ort;
                pren.AppendChild(ort);
            }
            doc.Save(@"C:\Users\bosa0003\source\repos\Lab2.Ads2\Lab2.Ads2\Prenumeranter.xml");


        }

        //Används för att fylla på en fil att använda vid test av import (är inte nödvändig)
        static void AddRecordtoXMLfileManualInput()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\Users\bosa0003\source\repos\Lab2.Ads2\Lab2.Ads2\Demofil.xml");
            XmlNode root = doc.SelectSingleNode("Prenumeranter");

            XmlElement pren = doc.CreateElement("Prenumerant");
            root.AppendChild(pren);
            XmlAttribute id = doc.CreateAttribute("id");
            id.Value = doc.SelectNodes("Prenumeranter/Prenumerant").Count.ToString();
            pren.Attributes.Append(id);
            XmlElement prennr = doc.CreateElement("Prenumerationsnummer");
            prennr.InnerText = "7";
            pren.AppendChild(prennr);
            XmlElement fnamn = doc.CreateElement("Förnamn");
            fnamn.InnerText = "Julia";
            pren.AppendChild(fnamn);
            XmlElement enamn = doc.CreateElement("Efternamn");
            enamn.InnerText = "Roberts";
            pren.AppendChild(enamn);
            XmlElement persnr = doc.CreateElement("Personnummer");
            persnr.InnerText = "6802425831";
            pren.AppendChild(persnr);
            XmlElement telnr = doc.CreateElement("Telefonnummer");
            telnr.InnerText = "0771750081";
            pren.AppendChild(telnr);
            XmlElement utadress = doc.CreateElement("Utdelningsadress");
            utadress.InnerText = "Los Angesles road 73";
            pren.AppendChild(utadress);
            XmlElement postnr = doc.CreateElement("Postnummer");
            postnr.InnerText = "78952";
            pren.AppendChild(postnr);
            XmlElement ort = doc.CreateElement("Ort");
            ort.InnerText = "Los Angeles";
            pren.AppendChild(ort);

            doc.Save(@"C:\Users\bosa0003\source\repos\Lab2.Ads2\Lab2.Ads2\Demofil.xml");

        }

        //Skapa formulär där person kan ladda upp fil för import av prenumeranter
        [HttpGet]
        public IActionResult TestAvInmatning()
        {
            return View();
        }


        //Sparar ner den importerade filen i wwwroot och kallar på metoden TestAvImportAsync, avslutar med att visa alla användare
        [HttpPost]
        public async Task<IActionResult> TestAvInmatning(IList<IFormFile> files)
        {
            string filename = "";
            foreach (IFormFile source in files)
            {
                filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');
                filename = this.EnsureCorrectFilename(filename);
                using (FileStream output = System.IO.File.Create(this.GetPathAndFilename(filename)))
                    await source.CopyToAsync(output);
            }
            string filnamn = filename;
            await TestAvImportAsync(filnamn);
            ViewBag.filnamn = filnamn;
            return RedirectToAction("ExportPren");
        }
        //Läser av filen med importerade prenumeranter till prenDetail objekt gör sedan en PostAsync för att spara i DB
        static async Task TestAvImportAsync(string filnamn)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\Users\bosa0003\source\repos\Lab2.Ads2\Lab2.Ads2\wwwroot\Uploads\" + filnamn);

            XmlNodeList nodes = doc.SelectNodes("Prenumeranter /Prenumerant");
            foreach (XmlNode node in nodes)
            {
                Pren detail = new Pren();
                detail.pr_prennr = Convert.ToInt32(node["Prenumerationsnummer"].InnerText);
                detail.pr_fnamn = node["Förnamn"].InnerText;
                detail.pr_efternamn = node["Efternamn"].InnerText;
                detail.pr_personnr = node["Personnummer"].InnerText;
                detail.pr_telefonnr = node["Telefonnummer"].InnerText;
                detail.pr_utadress = node["Utdelningsadress"].InnerText;
                detail.pr_postnr = Convert.ToInt32(node["Postnummer"].InnerText);
                detail.pr_ort = node["Ort"].InnerText;

                HttpClient client = new();
                StringContent sContent = new StringContent(JsonConvert.SerializeObject(detail), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync("https://localhost:7124/Prenumerant", sContent);
            }

        }

        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);
            return filename;
        }

        private string GetPathAndFilename(string filename)
        {
            return this._hostEnvironment.WebRootPath + "/Uploads/" + filename;
        }

        //Skapar en uppdaterad user.xml fil med data från DB via API.
        static async Task GetUserforDownload()
        {
            List<Pren> prenumeranter = new List<Pren>();
            HttpClient client = new HttpClient();


            client.BaseAddress = new Uri("https://localhost:7124/");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage responseMessage = await client.GetAsync("Prenumerant");
            responseMessage.EnsureSuccessStatusCode();



            if (responseMessage.IsSuccessStatusCode)
            {
                string apiResponse = await responseMessage.Content.ReadAsStringAsync();
                prenumeranter = JsonConvert.DeserializeObject<List<Pren>>(apiResponse);

                AddRecordtoXMLfile(prenumeranter);
            }



        }
        //Laddar ner Premumeramt.xml filen, kallar först på GetUserforDownload för att få en uppdaterad lista från DB
        public async Task<ActionResult> DownloadFileAsync()
        {
            await GetUserforDownload();
            string filePath = "Prenumeranter.xml";
            byte[] fileBytes = GetFile(@"C:\Users\bosa0003\source\repos\Lab2.Ads2\Lab2.Ads2\Prenumeranter.xml");
            return File(
                fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filePath);
        }



        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }     



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}