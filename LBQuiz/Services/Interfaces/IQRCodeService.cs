namespace LBQuiz.Services.Interfaces
{
    public interface IQRCodeService
    {
        Task<string> GetQRImageFromApiAsync(string joinCode, string? nickname = null);
    }
}
