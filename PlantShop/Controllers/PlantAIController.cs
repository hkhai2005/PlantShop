using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PlantShop.Models;

namespace PlantShop.Controllers
{
    public class PlantAIController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        public PlantAIController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                ViewBag.Error = "Please upload an image.";
                return View();
            }
            string fileName =
    Guid.NewGuid().ToString() +
    Path.GetExtension(image.FileName);

            string uploadPath =
                Path.Combine(
                    _environment.WebRootPath,
                    "uploads"
                );

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            string filePath =
                Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // đường dẫn ảnh để hiện ra view
            string imageUrl = "/uploads/" + fileName;

            ViewBag.UploadedImage = imageUrl;
            // 🔑 YOUR API KEY
            string apiKey = "2b102sj4hBTuHIIxxOULSeS7";

            using (var client = new HttpClient())
            {
                // ✅ Pl@ntNet endpoint
                string url =
                    $"https://my-api.plantnet.org/v2/identify/all?api-key={apiKey}";

                // multipart/form-data
                using (var content = new MultipartFormDataContent())
                {
                    // image stream
                    var streamContent =
                        new StreamContent(image.OpenReadStream());

                    streamContent.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue(
                            image.ContentType
                        );

                    // "images" là field bắt buộc của Pl@ntNet
                    content.Add(
                        streamContent,
                        "images",
                        image.FileName
                    );

                    // optional organs
                    content.Add(
                        new StringContent("leaf"),
                        "organs"
                    );

                    var response = await client.PostAsync(url, content);

                    var result =
                        await response.Content.ReadAsStringAsync();

                    // DEBUG
                    Console.WriteLine(result);

                    if (!response.IsSuccessStatusCode)
                    {
                        ViewBag.Error = result;
                        return View();
                    }

                    JObject obj;

                    try
                    {
                        obj = JObject.Parse(result);
                    }
                    catch
                    {
                        ViewBag.Error =
                            "Invalid JSON response: " + result;

                        return View();
                    }

                    var firstResult =
                    obj["results"]?.First();

                    if (firstResult == null)
                    {
                        ViewBag.Error =
                            "No plant detected.";

                        return View();
                    }

                    PlantResultViewModel model =
                        new PlantResultViewModel
                        {
                            PlantName =
                                firstResult["species"]?["scientificNameWithoutAuthor"]
                                ?.ToString(),

                            Probability = Math.Round(
                                (double)firstResult["score"] * 100,
                                2
                            ),

                            Description =
                                firstResult["species"]?["commonNames"]?
                                .FirstOrDefault()?.ToString(),

                                ImageUrl =
                                firstResult["images"]?[0]?["url"]?["o"]
                                ?.ToString(),

                                Family =
                                firstResult["species"]?["family"]?["scientificName"]
                                ?.ToString(),

                                Genus =
                                firstResult["species"]?["genus"]?["scientificName"]
                                ?.ToString()



                            };

                    return View(model);
                }
            }
        }
    }
}