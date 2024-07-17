using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Controllers
{
    // https://localhost:1234/api/regions
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        public RegionsController(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        

        // GET ALL REGIONS
        // GET: https://localhost:1234/api/regions
        [HttpGet]
        public IActionResult GetAll()
        {
            // Get Data from Database - Domain Models
            var regionsDomain = dbContext.Region.ToList();

            // Map Domain Models to DTOs
            var regionsDto = new List<RegionDto>();
            foreach (var region in regionsDomain)
            {
                regionsDto.Add(new RegionDto()
                {
                    Id = region.Id,
                    Code = region.Code,
                    Name = region.Name,
                    RegionImageUrl = region.RegionImageUrl
                });
            }

            // Return DTOs back to Client
            return Ok(regionsDto);
        }

        // GET SINGLE REGION (Get Region By ID)
        // GET: https://localhost:1234/api/regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            // Get Data from Database - Domain Models
            //var region = dbContext.Regions.Find(id); --> Find() takes in Primary Key only. For others, use FirstOrDefault()
            var region = dbContext.Region.FirstOrDefault(r => r.Id == id);

            // Map Domain Models to DTOs
            if (region != null)
            {
                var regionDto = new RegionDto()
                {
                    Id = region.Id,
                    Code = region.Code,
                    Name = region.Name,
                    RegionImageUrl = region.RegionImageUrl
                };

                // Return DTOs back to Client
                return Ok(regionDto);
            }

            return NotFound();
        }
    }
}
