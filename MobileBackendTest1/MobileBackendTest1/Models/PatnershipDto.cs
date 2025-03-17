using Microsoft.AspNetCore.Http;

namespace MobileBackendTest1.Models
{
    public class PartnershipDto
    {
        public string BrandName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile OfferFile { get; set; }
    }
}
