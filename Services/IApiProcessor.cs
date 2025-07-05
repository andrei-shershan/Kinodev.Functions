namespace Kinodev.Functions.Services
{
    public interface IApiProcessor
    {
        Task ProcessOrderCompletedAsync(string stringBody);

        Task ProcessOrderFileCreatedAsync(string stringBody);

        Task ProcessOrderFileUrlAddedAsync(string stringBody);

        Task ProcessEmailSentAsync(string stringBody);
    }
}