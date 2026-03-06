using LBQuiz.Services.Interfaces;
using Microsoft.JSInterop;

namespace LBQuiz.Services
{
    public class LocalStorageService : ILocalStorageService
    {
        private readonly IJSRuntime _js;
        public LocalStorageService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task SetLocalStorage(string key, string value)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", key, value, 30);
        }
        public async Task<string> GetLocalStorage(string key)
        {
            string returnString = "";
            if (!string.IsNullOrEmpty(key))
            {
                returnString += await _js.InvokeAsync<string>("localStorage.getItem", key);
            }
            return returnString;
           
        }
    }
}
