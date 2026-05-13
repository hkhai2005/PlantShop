using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PlantShop.Models;
using System.Text;

namespace PlantShop.Controllers
{
    public class PlantAIController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public PlantAIController(
    IWebHostEnvironment environment,
    IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
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
                client.DefaultRequestHeaders.Add(
    "User-Agent",
    "PlantShopApp/1.0"
);
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

                    string plantName =
    firstResult["species"]?["commonNames"]?
    .FirstOrDefault()?.ToString();

                    if (string.IsNullOrEmpty(plantName))
                    {
                        plantName =
                            firstResult["species"]?["scientificNameWithoutAuthor"]
                            ?.ToString();
                    }
                    Console.WriteLine("PLANT NAME: " + plantName);

                    ViewBag.PlantNameDebug = plantName;
                    string wikiDescription = "";
                    string wikiUrlPage = "";
                    try
                    {
                        string wikiUrl =
                            $"https://en.wikipedia.org/api/rest_v1/page/summary/{Uri.EscapeDataString(plantName)}";

                        var wikiResponse =
                            await client.GetAsync(wikiUrl);

                        var wikiResult =
                            await wikiResponse.Content.ReadAsStringAsync();

                        Console.WriteLine(wikiResult);
                        Console.WriteLine(wikiResult);

                        ViewBag.WikiRaw = wikiResult;
                        if (wikiResponse.IsSuccessStatusCode)
                        {
                            JObject wikiObj =
                                JObject.Parse(wikiResult);
                            wikiUrlPage =
                            wikiObj["content_urls"]?["desktop"]?["page"]
                            ?.ToString();
                            wikiDescription =
                                wikiObj["extract"]?.ToString();
                        }
                        else
                        {
                            wikiDescription =
                                "Wikipedia information not found.";
                        }
                    }
                    catch (Exception ex)
                    {
                        wikiDescription =
                            ex.Message;
                    }
                    //                    string prompt =
                    //                        $@"
                    //                        Give detailed care information about {plantName}.

                    //                        Include:
                    //                        - Description
                    //                        - Watering
                    //                        - Sunlight
                    //                        - Temperature
                    //                        - Humidity
                    //                        - Soil
                    //                        - Fertilizer
                    //                        - Toxicity
                    //                        - Care Tips

                    //                        Format nicely.
                    //                        ";


                    //                    var geminiUrl =
                    //$"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={geminiKey}";

                    //                    var geminiBody = new
                    //                    {
                    //                        contents = new[]
                    //                        {
                    //        new
                    //        {
                    //            parts = new[]
                    //            {
                    //                new
                    //                {
                    //                    text = prompt
                    //                }
                    //            }
                    //        }
                    //    }
                    //                    };

                    //                    var json =
                    //                        Newtonsoft.Json.JsonConvert.SerializeObject(
                    //                            geminiBody
                    //                        );

                    //                    var geminiContent =
                    //                        new StringContent(
                    //                            json,
                    //                            Encoding.UTF8,
                    //                            "application/json"
                    //                        );

                    //                    var geminiResponse =
                    //                        await client.PostAsync(
                    //                            geminiUrl,
                    //                            geminiContent
                    //                        );

                    //                    var geminiResult =
                    //                        await geminiResponse.Content.ReadAsStringAsync();
                    //                    Console.WriteLine(geminiResult);

                    //                    ViewBag.GeminiRaw = geminiResult;
                    //                    string aiText = "";

                    //                    try
                    //                    {
                    //                        JObject geminiObj =
                    //                            JObject.Parse(geminiResult);

                    //                        aiText =
                    //                            geminiObj["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]
                    //                            ?.ToString();
                    //                    }
                    //                    catch (Exception ex)
                    //                    {
                    //                        aiText =
                    //                            "AI ERROR: " + ex.Message;

                    //                        ViewBag.GeminiRaw = geminiResult;
                    //                    }



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
                                ?.ToString(),

                            WikiDescription = wikiDescription,


                            WikiUrl = wikiUrlPage

                        };

                    return View(model);
                }
            }
        }
    }
}