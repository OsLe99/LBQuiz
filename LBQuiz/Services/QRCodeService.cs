using Azure;
using LBQuiz.Services.Interfaces;

namespace LBQuiz.Services
{
    public class QRCodeService : IQRCodeService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public string PathString { get; set; }
        public string UriString { get; set; }

        public QRCodeService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = config;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetQRImageFromApiAsync(string joinCode)
        {
            var request = _httpContextAccessor.HttpContext?.Request;

            var baseUrl = $"{request?.Scheme}://{request?.Host}";

            var url = $"{baseUrl}/quiz/{joinCode}";

            //"/quiz/{LobbyCode}"

            var apiKey = _configuration["ApiNinjas:ApiKey"];

            //var url = $"https://github.com/OsLe99/LBQuiz";

            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

            var response = await client.GetAsync($"https://api.api-ninjas.com/v1/qrcode?data={url}&format=jpg");

            var pathString = "";

            if (response.IsSuccessStatusCode)
            {
                var base64String = await response.Content.ReadAsStringAsync();

                var imageBytes = Convert.FromBase64String(base64String);

                var filePath = Path.Combine("wwwroot", "images", "qrcode.jpg");
                Console.WriteLine( filePath);
                pathString = filePath.Replace("wwwroot", "").Replace("\\", "/");

                await File.WriteAllBytesAsync(filePath, imageBytes);

            }
            return pathString;
        }

    }
}
