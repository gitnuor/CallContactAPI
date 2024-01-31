using CallAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Net.Http;

namespace CallAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult>  Index()
        {
            try {
                //To bypass SSL certificate validation,
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
               
                List<Reservation> reservationlist = new List<Reservation>();
                using (var httpclient = new HttpClient(handler))
                {
                    using (var response = await httpclient.GetAsync("https://localhost:7195/api/Reservation"))
                    { 
                      string apiResponse=await response.Content.ReadAsStringAsync();
                      reservationlist= JsonConvert.DeserializeObject<List<Reservation>>(apiResponse);
                    }
                }
                return View(reservationlist);
            }
            catch (Exception ex)
            {
             return BadRequest(ex.Message); 
            }       
        }

        //public ViewResult GetReservation() => View();
        [HttpGet]
        public IActionResult GetReservation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetReservation(int id)
        {
            Reservation reservation = new Reservation();
            //To bypass SSL certificate validation,
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            using (var httpClient = new HttpClient(handler))
            {
                using (var response = await httpClient.GetAsync("https://localhost:7195/api/Reservation/" + id))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        reservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                    }
                    else
                        ViewBag.StatusCode = response.StatusCode;
                }
            }
            return View(reservation);
        }
        //[HttpGet]
        public ViewResult AddReservation()
        { 
          return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddReservation(Reservation reservation)
        {
            Reservation Recievedreservation = new Reservation();
            //To bypass SSL certificate validation,
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            using (var httpclient = new HttpClient(handler))
            {
                httpclient.DefaultRequestHeaders.Add("Key", "Secret@12");
                StringContent content=new StringContent(JsonConvert.SerializeObject(reservation),Encoding.UTF8, "application/json"); 
                using (var response = await httpclient.PostAsync("https://localhost:7195/api/Reservation",content))
                { 
                  string apiResponse=await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        Recievedreservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        ViewBag.Result = apiResponse;
                        return View();
                    }                        
                }
            }
             return View(Recievedreservation);
        }
        [HttpGet]
        public async Task<IActionResult> UpdateReservation(int id)
        {
            Reservation update = new Reservation();
            //To bypass SSL certificate validation,
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            using (var httpclient = new HttpClient(handler))
            {
                using (var response = await httpclient.GetAsync("https://localhost:7195/api/Reservation/" + id))
                {
                    string apiresponse = await response.Content.ReadAsStringAsync();
                    update = JsonConvert.DeserializeObject<Reservation>(apiresponse);
                }
            }
            return View(update);

        }
        [HttpPost]
        public async Task<IActionResult> UpdateReservation(Reservation reservation)
        {
            Reservation updateReservation = new Reservation();
            //To bypass SSL certificate validation,
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            using (var httpclient = new HttpClient(handler))
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(reservation), Encoding.UTF8, "application/json");
                using (var response = await httpclient.PutAsync("https://localhost:7195/api/Reservation", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    ViewBag.Result = "Success";
                    updateReservation = JsonConvert.DeserializeObject<Reservation>(apiResponse);
                }
            }
            return View(updateReservation);
        }
        [HttpGet]
        public async Task<IActionResult> UpdateReservationPatch(int id)
        { 
            Reservation reservation = new Reservation();
            //To bypass SSL certificate validation,
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            using (var httpclient = new HttpClient(handler))
            {
                using (var response = await httpclient.GetAsync("https://localhost:7195/api/Reservation/" + id))
                { 
                  string apiResponse= await response.Content.ReadAsStringAsync();
                  reservation= JsonConvert.DeserializeObject<Reservation>(apiResponse);
                }
            }
            return View(reservation);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateReservationPatch(int id, Reservation reservation)
        {
            //To bypass SSL certificate validation,
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            using (var httpClient = new HttpClient(handler))
            {
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri("https://localhost:7195/api/Reservation/" + id),
                    Method = new HttpMethod("Patch"),
                    Content = new StringContent("[{ \"op\": \"replace\", \"path\": \"Name\", \"value\": \"" + reservation.Name + "\"},{ \"op\": \"replace\", \"path\": \"StartLocation\", \"value\": \"" + reservation.StartLocation + "\"}]", Encoding.UTF8, "application/json")
                };

                var response = await httpClient.SendAsync(request);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReservation(int ReservationId)
        { 
            Reservation reservation = new Reservation();
            //To bypass SSL certificate validation,
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            using (var htttpclient = new HttpClient(handler))
            {
                using (var response = await htttpclient.DeleteAsync("https://localhost:7195/api/Reservation/" + ReservationId))
                { 
                 string apiResponse= await response.Content.ReadAsStringAsync();
                 
                }
            }
            return RedirectToAction("Index");
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
    }
}