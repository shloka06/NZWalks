using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    // https://localhost:1234/api/regions
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
        }

        // GET ALL REGIONS
        // GET: https://localhost:1234/api/regions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Get Data from Database - Domain Models
            //var regionsDomain = await dbContext.Region.ToListAsync();
            var regionsDomain = await regionRepository.GetAllAsync(); // Best Practice - Using Repository Pattern

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
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            // Get Data from Database - Domain Models
            //var region = dbContext.Regions.Find(id); --> Find() takes in Primary Key only. For others, use FirstOrDefault()
            var region = await dbContext.Region.FirstOrDefaultAsync(r => r.Id == id);

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

        // POST TO CREATE NEW REGION
        // POST: https://localhost:1234/api/regions
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            // Map/Convert DTO to Domain Model
            var regionDomainModel = new Region
            {
                Code = addRegionRequestDto.Code,
                Name = addRegionRequestDto.Name,
                RegionImageUrl = addRegionRequestDto.RegionImageUrl
            };

            // Use Domain Model to Create 
            await dbContext.Region.AddAsync(regionDomainModel);
            await dbContext.SaveChangesAsync();

            // Map Domain Model back to DTO
            var regionDto = new RegionDto 
            { 
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };

            return CreatedAtAction(nameof(GetById), new { id = regionDomainModel.Id }, regionDto);
        }

        // UPDATE REGION
        // PUT: https://localhost:1234/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            // Check if Region Exists
            var regionDomainModel = await dbContext.Region.FirstOrDefaultAsync(r => r.Id == id);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // Map DTO to Domain Model
            regionDomainModel.Code = updateRegionRequestDto.Code;
            regionDomainModel.Name = updateRegionRequestDto.Name;
            regionDomainModel.RegionImageUrl = updateRegionRequestDto.RegionImageUrl;

            await dbContext.SaveChangesAsync();

            // Convert Domain Model back to DTO
            var regionDto = new RegionDto 
            { 
                Id=regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };

            return Ok(regionDto);
        }

        // DELETE REGION
        // DELETE: https://localhost:1234/api/regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            // Check if Region Exists
            var regionDomainModel = await dbContext.Region.FirstOrDefaultAsync(r => r.Id == id);
            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // Delete Region
            dbContext.Region.Remove(regionDomainModel); // Remove doesn't have an async version.
            await dbContext.SaveChangesAsync();

            // Optional - Return Deleted Region - 
            // Map Region Domain Model to DTO
            var regionDto = new Region()
            {
                Id = regionDomainModel.Id,
                Code = regionDomainModel.Code,
                Name = regionDomainModel.Name,
                RegionImageUrl = regionDomainModel.RegionImageUrl
            };

            return Ok(regionDto);

            // Else, just return Empty OK Response -
            // return Ok();
        }
    }
}
