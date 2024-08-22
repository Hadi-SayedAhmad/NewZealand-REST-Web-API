using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Text.Json;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class RegionsController : ControllerBase
    {
        
        private readonly IRegionRepository IRR;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger1;

        public RegionsController(IRegionRepository IRR, IMapper mapper, ILogger<RegionsController> logger1)
        {
           
            this.IRR = IRR;
            this.mapper = mapper;
            this.logger1 = logger1;
        }

        [HttpGet]
        //[Authorize(Roles = "Reader, Writer")]
        public async Task<IActionResult> GetAll()
        {
            
            logger1.LogInformation("GetAll action method entered!");

            var regions = await IRR.GetAllAsync();
            logger1.LogInformation($"Finished getting all the regions! {JsonSerializer.Serialize(regions)}");
            //map these model to dto
            //var regionsDto = new List<RegionDto>();
            //foreach (var region in regions)
            //{
            //    regionsDto.Add(new RegionDto()
            //    {
            //        Id = region.Id,
            //        Name = region.Name,
            //        Code = region.Code,
            //        RegionImageUrl = region.RegionImageUrl,
            //    });
            //}

            //using automapper
            var regionsDto = mapper.Map<List<RegionDto>>(regions);

            //return dto
            return Ok(regionsDto);
        }


        [HttpGet]
        [Route("{id:Guid}")]
        /*[Authorize(Roles = "Reader, Writer")]*/
        public async Task<IActionResult> GetRegionById([FromRoute] Guid id)
        {
            var region = await IRR.GetByIdAsync(id);
            if (region == null)
            {
                return NotFound();
            }

            //var regionDto = new RegionDto()
            //{
            //    Id = region.Id,
            //    Name = region.Name,
            //    Code = region.Code,
            //    RegionImageUrl = region.RegionImageUrl,
            //};
            var regionDto = mapper.Map<RegionDto>(region);
            return Ok(regionDto);
        }

        [HttpPost]
        [ValidateModel]
        /*[Authorize(Roles = "Writer")]*/
        public async Task<IActionResult> Create([FromBody] RegionRequestDto RRD)
        {

                // map the dto to domain model
                //var regionDomainModel = new Region
                //{
                //    Code = RRD.Code,
                //    Name = RRD.Name,
                //    RegionImageUrl = RRD.RegionImageUrl,
                //};


                var regionDomainModel = mapper.Map<Region>(RRD);


                //use domain to add to db
                var createdRegion = await IRR.CreateAsync(regionDomainModel);

                // map domain model again to dto
                var regionDto = mapper.Map<RegionDto>(createdRegion);
                //var regionDto = new RegionDto
                //{
                //    Id = createdRegion.Id,
                //    Code = createdRegion.Code,
                //    Name = createdRegion.Name,
                //    RegionImageUrl = createdRegion.RegionImageUrl,
                //};

                // this is used to send the resource and the url to this resource using the method get by id in the location key in the headers of the response.
                return CreatedAtAction(nameof(GetRegionById), new { id = regionDto.Id }, regionDto);

        
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        /*[Authorize(Roles = "Writer")]*/
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto URRD)
        {
            
                //map dto to domain model
                var regionDomainModel = mapper.Map<Region>(URRD);
                //var regionDomainModel = new Region
                //{
                //    Code = URRD.Code,
                //    Name = URRD.Name,
                //    RegionImageUrl = URRD.RegionImageUrl,
                //};

                regionDomainModel = await IRR.UpdateAsync(id, regionDomainModel);
                if (regionDomainModel == null)
                {
                    return NotFound();

                }
                var regionDto = mapper.Map<RegionDto>(regionDomainModel);
                //var regionDto = new RegionDto
                //{
                //    Id = regionDomainModel.Id,
                //    Code = regionDomainModel.Code,
                //    Name = regionDomainModel.Name,
                //    RegionImageUrl = regionDomainModel.RegionImageUrl,
                //};

                return Ok(regionDto);
            
            
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        /*[Authorize(Roles = "Writer")]*/
        public async Task<IActionResult> Delete([FromRoute] Guid id) {
            var regionDomainModel = await IRR.DeleteAsync(id);
            if (regionDomainModel == null)
            {
                return NotFound();

            }
            var regionDto = mapper.Map<RegionDto>(regionDomainModel);
            //var regionDto = new RegionDto
            //{
            //    Id = regionDomainModel.Id,
            //    Code = regionDomainModel.Code,
            //    Name = regionDomainModel.Name,
            //    RegionImageUrl = regionDomainModel.RegionImageUrl,
            //};

            return Ok(regionDto);
        }

        
    }
}
