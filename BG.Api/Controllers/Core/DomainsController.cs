using BG.Model.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using PrjBase.CachingBase;
using PrjBase.Data;
using RestBase;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace BG.API.Controllers.Core
{
    #region Domain Web API

    [ApiController]
    [Route("api/Core/[controller]")]
    public class DomainsController : ControllerBase
    {
        private readonly ILogger<DomainsController> _logger;
        private readonly ICoreServices CoreServices;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;

        private IDomainServices DomainServices { get { return this.CoreServices.Domain; } }

        //* Ctor
        public DomainsController(ICoreServices coreServices
                                        , ILogger<DomainsController> logger
                                        , IMemoryCache memoryCache
                                        , IDistributedCache distributedCache)
        {
            this.CoreServices = coreServices;
            _logger = logger;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDomain(Domain domain)
        {
            //* Validate
            if (domain is null)
                return BadRequest("invalid domain parameter: null");
            else if (domain.DomainID > 0)
            {
                return BadRequest($"Invalid domain ID, It should not have a value and it has: {domain.DomainID}");
            }

            //* Proceed
            Domain insertedDomain = await this.DomainServices.Insert(domain);

            return CreatedAtAction(
                actionName: nameof(GetDomain),
                routeValues: new { id = insertedDomain.DomainID },
                value: insertedDomain
            );
        }

        [HttpGet()]
        [Route("GetAll")]             //* api/Core/Domains/GetAll
        public async Task<ActionResult<IEnumerable<Domain>>> GetDomains()
        {
            try
            {
                List<Domain> domains = await this.DomainServices.GetAll();

                if (domains == null)
                    return NotFound();
                else
                    return Ok(domains);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError
                                    , "Error getting stored data from the database");
            }
        }

        [HttpGet()]
        [Route("Search")]             //* api/Core/Domains/Search?dataToSearch=something
        public async Task<ActionResult<IEnumerable<Domain>>> Search(
                                        [FromQuery] string? dataToSearch)
        {
            try
            {
                if (dataToSearch is null)
                    return await this.GetDomains();

                List<Domain> domains = await this.DomainServices.Search(dataToSearch);

                if (domains == null)
                    return NotFound();
                else
                    return Ok(domains);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError
                                    , "Error getting stored data from the database");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDomain(int id)
        {
            try
            {
                //* Validate
                if (id <= 0)
                    return BadRequest($"Invalid DomainID value: {id}");

                //* Proceed
                Domain? domain = await this.DomainServices.GetByID(id, doLoadLists: true);

                if (domain == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(domain);
                }

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError
                                    , "Error getting stored data from the database");
            }
        }


        [HttpGet()]
        [ResponseCache(CacheProfileName = "AnyRespCacheLocation-60")]
        [SwaggerOperation(
            Summary = "Get a list of Page Indexed, Page Sized, Sorted and Filtered data to Display in a Table.",
            Description = "Retrieves a list of Domains " +
            "with custom paging, sorting, and filtering rules.")]
        public async Task<RestResponse<List<Domain>>> Get(
                        [FromQuery]
                        [SwaggerParameter("An object (possibly DTO) that can be used " +
                                "to customize some retrieval parameters.")]
                        GridDataRequest<Domain> gridDataRequest
        )
        {
            gridDataRequest.SortColumn ??= "DomainID";    //if SortColumn null, override

            string cacheKey = $"{gridDataRequest.GetType()}"
                                + $"-{JsonSerializer.Serialize(gridDataRequest)}";

            _logger.LogInformation(500151, "Get start. cacheKey : {cacheKey}", cacheKey);

            List<Domain>? domains = null;

            if (_memoryCache.TryGetValue<List<Domain>>(cacheKey, out domains))
            {
                _logger.LogInformation(500151, "Data fetched from cached");
            }
            else
            {
                #region Get data from database and cache it

                //* Data is not cached, get it from DBMS and cache it

                domains = await this.DomainServices
                                .GetAll_Paged_Sorted_andFiltered(
                                    pageIndex: gridDataRequest.PageIndex
                                    , pageSize: gridDataRequest.PageSize
                                    , sortColumn: gridDataRequest.SortColumn
                                    , sortOrder: gridDataRequest.SortOrder!
                                    , filterQuery: gridDataRequest.FilterQuery
                                    );

                _memoryCache.Set(cacheKey, domains, new TimeSpan(0, 0, 20));

                _logger.LogInformation(500151, "Data fetched from database");

                #endregion
            }

            //* Distributed caching

            List<Domain>? domains_distributed = null;

            if (_distributedCache.TryGetValue<List<Domain>>(cacheKey, out domains_distributed))
            {
                _logger.LogInformation(500151, "Data fetched from distributed cached");
            }
            else
            {
                #region Get data from database and (distributed) cache it

                //* Data is not cached, get it from DBMS and cache it

                domains_distributed = await this.DomainServices
                                .GetAll_Paged_Sorted_andFiltered(
                                    pageIndex: gridDataRequest.PageIndex
                                    , pageSize: gridDataRequest.PageSize
                                    , sortColumn: gridDataRequest.SortColumn
                                    , sortOrder: gridDataRequest.SortOrder!
                                    , filterQuery: gridDataRequest.FilterQuery
                                    );

                _distributedCache.Set(cacheKey, domains_distributed, new TimeSpan(0, 0, 40));

                _logger.LogInformation(500151, "Data fetched from database (distributed)");

                #endregion
            }


            _logger.LogInformation(500151
            , "Get end. domains.Count: {domains_Count}", domains!.Count);

            #region Create REST Response

            var restResponse = new RestResponse<List<Domain>>()
            {
                Data = domains,
                PageIndex = gridDataRequest.PageIndex,
                PageSize = gridDataRequest.PageSize,
                RecordCount = domains.Count,
                Links = new List<ResponseLink> {
                    new ResponseLink(
                        Url.Action(
                            null,
                            "Domains",
                            new { gridDataRequest.PageIndex, gridDataRequest.PageSize
                                , gridDataRequest.SortColumn, gridDataRequest.SortOrder
                                , gridDataRequest.FilterQuery },
                            Request.Scheme)!,
                        "self",
                        "GET"),
                }
            };

            #endregion

            return restResponse;
        }


        [HttpGet()]
        [Route("Count")]         //* api/Core/Domains/Count
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                int count = await this.DomainServices.GetAllRecordCount();

                return Ok(count);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError
                                    , "Error getting stored data from the database");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDomain(int id, Domain domain)
        {
            //* Validate
            if (domain is null)
                return BadRequest("invalid domain parameter: null");
            else if (id <= 0)
            {
                return BadRequest($"Invalid id parameter. It should have a value; but it has: {id}");
            }

            //* Proceed
            domain.DomainID = id;
            var pkid = await this.DomainServices.Update(domain);
            //TODO: return 201 if a new breakfast was created
            if (pkid > 0)
                return NoContent();
            else
                return NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDomain(int id)
        {
            //* Validate
            if (id <= 0)
                return BadRequest($"Invalid DomainID value: {id}");

            //* Proceed
            await this.DomainServices.Delete(id);
            return NoContent();
        }
    }
    //

    #endregion

    #region Error Handling

    //* At program.cs add
    //app.UseExceptionHandler("/error")

    //* Add Controller Error
    //public class ErrorsController: ControllerBase
    //{
    //[Route("/error")]
    //public IActionResult Error()
    //{
    //return Problem();
    //}
    //}

    #endregion
}