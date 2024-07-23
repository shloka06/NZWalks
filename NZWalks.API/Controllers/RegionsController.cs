using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Net.WebSockets;

namespace NZWalks.API.Controllers
{
    // https://localhost:1234/api/regions
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;

        public RegionsController(NZWalksDbContext dbContext, IRegionRepository regionRepository, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
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
            //var regionsDto = new List<RegionDto>();
            //foreach (var region in regionsDomain)
            //{
            //    regionsDto.Add(new RegionDto()
            //    {
            //        Id = region.Id,
            //        Code = region.Code,
            //        Name = region.Name,
            //        RegionImageUrl = region.RegionImageUrl
            //    });
            //}
            
            // Using Automapper - Map<Destination>(Source)
            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);

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
            //var region = await dbContext.Region.FirstOrDefaultAsync(r => r.Id == id);
            var regionDomain = await regionRepository.GetByIdAsync(id);

            // Map Domain Models to DTOs
            if (regionDomain != null)
            {
                //var regionDto = new RegionDto()
                //{
                //    Id = region.Id,
                //    Code = region.Code,
                //    Name = region.Name,
                //    RegionImageUrl = region.RegionImageUrl
                //};

                //// Return DTOs back to Client
                //return Ok(regionDto);

                return Ok(mapper.Map<RegionDto>(regionDomain));
            }

            return NotFound();
        }

        // POST TO CREATE NEW REGION
        // POST: https://localhost:1234/api/regions
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
            // Map/Convert DTO to Domain Model
            //var regionDomainModel = new Region
            //{
            //    Code = addRegionRequestDto.Code,
            //    Name = addRegionRequestDto.Name,
            //    RegionImageUrl = addRegionRequestDto.RegionImageUrl
            //};
            var regionDomainModel = mapper.Map<Region>(addRegionRequestDto);

            // Use Domain Model to Create 
            //await dbContext.Region.AddAsync(regionDomainModel);
            //await dbContext.SaveChangesAsync();
            var region = await regionRepository.CreateAsync(regionDomainModel);

            // Map Domain Model back to DTO
            //var regionDto = new RegionDto 
            //{ 
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return CreatedAtAction(nameof(GetById), new { id = regionDomainModel.Id }, regionDto);
        }

        // UPDATE REGION
        // PUT: https://localhost:1234/api/regions/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
            // Below code now handled in RegionRepository.
            // Check if Region Exists
            //var regionDomainModel = await dbContext.Region.FirstOrDefaultAsync(r => r.Id == id);

            // Map DTO to Domain Model
            //var regionDomainModel = new Region { 
            //    Code = updateRegionRequestDto.Code,
            //    Name = updateRegionRequestDto.Name,
            //    RegionImageUrl= updateRegionRequestDto.RegionImageUrl
            //};
            var regionDomainModel = mapper.Map<Region>(updateRegionRequestDto);

            regionDomainModel = await regionRepository.UpdateAsync(id, regionDomainModel);

            if (regionDomainModel == null)
            {
                return NotFound();
            }

            //await dbContext.SaveChangesAsync();

            // Convert Domain Model back to DTO
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDto);
        }

        // DELETE REGION
        // DELETE: https://localhost:1234/api/regions/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            // Check if Region Exists
            //var regionDomainModel = await dbContext.Region.FirstOrDefaultAsync(r => r.Id == id);
            var regionDomainModel = await regionRepository.DeleteAsync(id);
            
            if (regionDomainModel == null)
            {
                return NotFound();
            }

            // Delete Region
            //dbContext.Region.Remove(regionDomainModel); // Remove doesn't have an async version.
            //await dbContext.SaveChangesAsync();

            // Optional - Return Deleted Region - 
            // Map Region Domain Model to DTO
            //var regionDto = new Region()
            //{
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl
            //};
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);

            return Ok(regionDto);

            // Else, just return Empty OK Response -
            // return Ok();
        }
    }
}
