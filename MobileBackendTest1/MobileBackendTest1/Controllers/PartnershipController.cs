using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileBackendTest1.Models;
using MobileBackendTest1.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileBackendTest1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartnershipController : ControllerBase
    {
        private readonly PartnershipService _partnershipService;

        public PartnershipController(PartnershipService partnershipService)
        {
            _partnershipService = partnershipService;
        }

        // Get all partnerships
        [HttpGet]
        public async Task<ActionResult<List<Partnership>>> GetAllPartnerships()
        {
            var partnerships = await _partnershipService.GetAllPartnershipsAsync();
            return Ok(partnerships);
        }

        // Get partnership by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<Partnership>> GetPartnershipById(string id)
        {
            var partnership = await _partnershipService.GetPartnershipByIdAsync(id);
            if (partnership == null)
                return NotFound("Partnership not found.");
            return Ok(partnership);
        }

        // Create a new partnership with file upload
        [HttpPost]
        public async Task<IActionResult> CreatePartnership([FromForm] PartnershipDto partnershipDto)
        {
            if (partnershipDto.OfferFile == null)
                return BadRequest("Offer file is required.");

            //convert PartnershipDto to Partnership model
            var partnership = new Partnership
            {
                BrandName = partnershipDto.BrandName,
                Description = partnershipDto.Description,
            };

            await _partnershipService.CreatePartnershipAsync(partnership, partnershipDto.OfferFile);
            return CreatedAtAction(nameof(GetPartnershipById), new { id = partnership.Id }, partnership);
        }

        // Update an existing partnership with file upload
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePartnership(string id, [FromForm] PartnershipDto partnershipDto)
        {
            if (partnershipDto.OfferFile == null)
                return BadRequest("Offer file is required.");

            var updatedPartnership = new Partnership
            {

                BrandName = partnershipDto.BrandName,
                Description = partnershipDto.Description,
            };
            await _partnershipService.UpdatePartnershipAsync(id, updatedPartnership, partnershipDto.OfferFile);
            return NoContent();
        }

        // Delete a partnership
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePartnership(string id)
        {
            await _partnershipService.DeletePartnershipAsync(id);
            return NoContent();
        }
    }
}
