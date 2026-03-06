namespace LBQuiz.Services.Interfaces
{
    public interface ILocalStorageService
    {
        Task SetLocalStorage(string key, string value);
        Task<string> GetLocalStorage(string key);
    }
}
