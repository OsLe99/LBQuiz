namespace LBQuiz.Services.Interfaces
{
    public interface IQRCodeService
    {
        Task GetQRImageFromApiAsync(string joinCode);
    }
}
