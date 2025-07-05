namespace Kinodev.Functions.Configurations
{
    public class FunctionsServiceApiUrlsSettings
    {
        public bool IgnoreSslErrors { get; set; }
        public required string StorageServiceUrl { get; set; }
        public required string EmailServiceUrl { get; set; }
        public required string DomainServiceUrl { get; set; }
    }
}