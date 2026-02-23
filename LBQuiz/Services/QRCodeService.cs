using Azure;
using LBQuiz.Services.Interfaces;

namespace LBQuiz.Services
{
    public class QRCodeService : IQRCodeService
    {
        private readonly IConfiguration _configuration;
        public string PathString { get; set; }
        public string UriString { get; set; }

        public QRCodeService(IConfiguration config)
        {
            _configuration = config;
        }

        public async Task GetQRImageFromApiAsync(string joinCode)
        {
            var apiKey = _configuration["ApiNinjas:ApiKey"];

            var url = $"https://github.com/OsLe99/LBQuiz";

            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

            var response = await client.GetAsync($"https://api.api-ninjas.com/v1/qrcode?data={url}&format=jpg");

           

            if (response.IsSuccessStatusCode)
            {
                var base64String = await response.Content.ReadAsStringAsync();

                var imageBytes = Convert.FromBase64String(base64String);

                var filePath = Path.Combine("wwwroot", "images", "qrcode.jpg");

                await File.WriteAllBytesAsync(filePath, imageBytes);
            }
        }

    }
}
