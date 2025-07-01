namespace Kinodev.Functions.Services
{
    public interface IPdfService
    {
        Task<byte[]> GeneratePdfAsync(string htmlContent);
    }   
}