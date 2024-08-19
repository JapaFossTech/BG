using BG.Model.Core;
using Infrastructure.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using PrjBase.CachingBase;
using PrjBase.Data;
using PrjBase.ModelBase.Attributes;
using PrjBase.SecurityBase;


//using Microsoft.EntityFrameworkCore;
using RestBase;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace BG.API.Controllers.Core
{
    #region BoardGame Web API

    [ApiController]
    [Route("api/Core/[controller]")]
    public class BoardGamesController : ControllerBase
    {
        private readonly ILogger<BoardGamesController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly ICoreServices CoreServices;

        private IBoardGameServices BoardGameServices { get { return this.CoreServices.BoardGame; } }

        //* Ctor
        public BoardGamesController(ICoreServices coreServices
            , ILogger<BoardGamesController> logger, IMemoryCache memoryCache
            , IDistributedCache distributedCache)
        {
            this.CoreServices = coreServices;
            _logger = logger;
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBoardGame(BoardGame boardGame)
        {
            //* Validate
            if (boardGame is null)
                return BadRequest("invalid boardGame parameter: null");
            else if (boardGame.BoardGameID > 0)
            {
                return BadRequest($"Invalid boardGame ID, It should not have a value and it has: {boardGame.BoardGameID}");
            }

            //* Proceed
            BoardGame insertedBoardGame = await this.BoardGameServices
                                                        .Insert(boardGame);

            return CreatedAtAction(
                actionName: nameof(GetBoardGame),
                routeValues: new { id = insertedBoardGame.BoardGameID },
                value: insertedBoardGame
            );
        }

        [HttpGet()]
        [Route("GetAll")]             //* api/Core/BoardGames/GetAll
        public async Task<ActionResult<IEnumerable<BoardGame>>> GetBoardGames()
        {
            try
            {
                List<BoardGame> boardGames = await this.BoardGameServices.GetAll();

                if (boardGames == null)
                    return NotFound();
                else
                    return Ok(boardGames);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError
                                    , "Error getting stored data from the database");
            }
        }

        [HttpGet()]
        [Route("Search")]         //* api/Core/BoardGames/Search?dataToSearch=something
        public async Task<ActionResult<IEnumerable<BoardGame>>> Search(
                                        [FromQuery] string? dataToSearch)
        {
            try
            {
                if (dataToSearch is null)
                    return await this.GetBoardGames();

                List<BoardGame> boardGames = await this.BoardGameServices.Search(dataToSearch);

                if (boardGames == null)
                    return NotFound();
                else
                    return Ok(boardGames);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError
                                    , "Error getting stored data from the database");
            }
        }

        [HttpGet("{id:int}")]
        [SwaggerOperation(
            Summary = "Get a single board game.",
            Description = "Retrieves a single board game with the given Id.")]
        public async Task<IActionResult> GetBoardGame(
            [CustomKeyValue("x-test-3", "value 3")]
            int id)
        {
            try
            {
                //* Validate
                if (id <= 0)
                    return BadRequest($"Invalid BoardGameID value: {id}");

                //* Proceed
                BoardGame? boardGame = await this.BoardGameServices.GetByID(id, doLoadLists: true);

                if (boardGame == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(boardGame);
                }

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError
                                    , "Error getting stored data from the database");
            }
        }

        [HttpGet()]     //* Name = "GetBoardGamesDto"
        //[Route("GetAllDto")]             //* api/Core/BoardGames/GetAllDto
        //[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 60)]
        [ResponseCache(CacheProfileName = "AnyRespCacheLocation-60")]
        [SwaggerOperation(
            Summary = "Get a list of board games.",
            Description = "Retrieves a list of board games " +
            "with custom paging, sorting, and filtering rules.")]
        public async Task<RestResponse<List<BoardGame>>> Get(
                        [FromQuery]
                        [SwaggerParameter("A DTO object that can be used " +
                                "to customize some retrieval parameters.")]
                        GridDataRequest<BoardGame_ChangeDTO> gridDataRequest
        )
        {
            gridDataRequest.SortColumn ??= "BoardGameID";    //if SortColumn null, override

            string cacheKey = $"{gridDataRequest.GetType()}"
                                + $"-{JsonSerializer.Serialize(gridDataRequest)}";

            _logger.LogInformation(500151, "Get start. cacheKey : {cacheKey}", cacheKey);

            List<BoardGame>? boardGames = null;

            if (_memoryCache.TryGetValue<List<BoardGame>>(cacheKey, out boardGames))
            {
                _logger.LogInformation(500151, "Data fetched from cached");
            }
            else
            {
                #region Get data from database and cache it

                //* Data is not cached, get it from DBMS and cache it

                boardGames = await this.BoardGameServices
                                .GetAll_Paged_Sorted_andFiltered(
                                    pageIndex: gridDataRequest.PageIndex
                                    , pageSize: gridDataRequest.PageSize
                                    , sortColumn: gridDataRequest.SortColumn
                                    , sortOrder: gridDataRequest.SortOrder!
                                    , filterQuery: gridDataRequest.FilterQuery
                                    );
                //boardGames = await this.BoardGameServices
                //                .GetAll_Paged_Sorted_andFiltered(getDataRequest);

                _memoryCache.Set(cacheKey, boardGames, new TimeSpan(0, 0, 20));

                _logger.LogInformation(500151, "Data fetched from database");

                #endregion
            }

            //* Distributed caching

            List<BoardGame>? boardGames_distributed = null;

            if (_distributedCache.TryGetValue<List<BoardGame>>(cacheKey, out boardGames_distributed))
            {
                _logger.LogInformation(500151, "Data fetched from distributed cached");
            }
            else
            {
                #region Get data from database and (distributed) cache it

                //* Data is not cached, get it from DBMS and cache it

                boardGames_distributed = await this.BoardGameServices
                                .GetAll_Paged_Sorted_andFiltered(
                                    pageIndex: gridDataRequest.PageIndex
                                    , pageSize: gridDataRequest.PageSize
                                    , sortColumn: gridDataRequest.SortColumn
                                    , sortOrder: gridDataRequest.SortOrder!
                                    , filterQuery: gridDataRequest.FilterQuery
                                    );

                //boardGames = await this.BoardGameServices
                //                .GetAll_Paged_Sorted_andFiltered(getDataRequest);

                _distributedCache.Set(cacheKey, boardGames_distributed, new TimeSpan(0, 0, 40));

                _logger.LogInformation(500151, "Data fetched from database (distributed)");

                #endregion
            }


            _logger.LogInformation(500151
            , "Get end. boardGames.Count: {boardGames_Count}", boardGames!.Count);

            //https://localhost:40443/api/Core/BoardGames/GetAllDto?PageIndex=0&PageSize=10&SortOrder=ASC

            //throw new Exception("Test error from get end point");

            #region Create REST Response

            var restResponse = new RestResponse<List<BoardGame>>()
            {
                Data = boardGames,
                PageIndex = gridDataRequest.PageIndex,
                PageSize = gridDataRequest.PageSize,
                RecordCount = boardGames.Count,
                Links = new List<ResponseLink> {
                    new ResponseLink(
                        Url.Action(
                            null,
                            "BoardGames",
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
        [Route("Count")]         //* api/Core/BoardGames/Count
        public async Task<ActionResult<int>> Count()
        {
            try
            {
                int count = await this.BoardGameServices.GetAllRecordCount();
                    
                return Ok(count);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError
                                    , "Error getting stored data from the database");
            }
        }


        [HttpPut("{id:int}")]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> UpdateBoardGame(int id, BoardGame boardGame)
        {
            //* Validate
            if (boardGame is null)
                return BadRequest("invalid boardGame parameter: null");
            else if (id <= 0)
            {
                return BadRequest($"Invalid id parameter. It should have a value; but it has: {id}");
            }

            //* Proceed
            boardGame.BoardGameID = id;
            var pkid = await this.BoardGameServices.Update(boardGame);
            //TODO: return 201 if a new breakfast was created
            if (pkid > 0)
                return NoContent();
            else
                return NotFound();
        }

        [Authorize(Roles = RoleName.MODERATOR)]
        [HttpPut()]                        //Name = "UpdateBoardGame"
        [ResponseCache(NoStore = true)]
        [Route("UpdateDto")]         //* api/Core/BoardGames/UpdateBoardGame
        [SwaggerOperation(
            Summary = "Updates a board game (RoleName.MODERATOR).",
            Description = "Updates partially the entity. It will only change the provided fields")]
        public async Task<RestResponse<BoardGame?>> UpdateDto(
                                                    BoardGame_ChangeDTO boardGame)
        {
            _logger.LogInformation(500152, "Update start");

            BoardGame? storedBoardGame = await this.BoardGameServices
                                        .GetByID(boardGame.BoardGameID);

            if (storedBoardGame != null)
            {
                if (!string.IsNullOrEmpty(boardGame.Name))
                    storedBoardGame.Name = boardGame.Name;
                if (boardGame.Year.HasValue && boardGame.Year.Value > 0)
                    storedBoardGame.Year = boardGame.Year.Value;

                //storedBoardGame.LastModifiedDate = DateTime.Now;

                await this.BoardGameServices.Update(storedBoardGame);
            };

            return new RestResponse<BoardGame?>()
            {
                Data = storedBoardGame,
                Links = new List<ResponseLink>
                {
                    new ResponseLink(
                            Url.Action(
                                null,
                                "BoardGames",
                                boardGame,
                                Request.Scheme)!,
                            "self",
                            "POST"),
                }
            };
        }

        [Authorize(Roles = RoleName.ADMIN)]
        [HttpDelete("{id:int}")]
        [ResponseCache(NoStore = true)]
        [SwaggerOperation(
            Summary = "Deletes a board game (RoleName.ADMIN).",
            Description = "Deletes a board game from the database.")]
        public async Task<IActionResult> DeleteBoardGame(int id)
        {
            //* Validate
            if (id <= 0)
                return BadRequest($"Invalid BoardGameID value: {id}");

            //* Proceed
            await this.BoardGameServices.Delete(id);
            return NoContent();
        }

        //[HttpDelete()]//Name = "DeleteBoardGame"
        //[ResponseCache(NoStore = true)]
        //[Route("Delete")]         //* api/Core/BoardGames/UpdateBoardGame
        //public async Task<RestResponse<bool>> Delete(int id)
        //{
        //    bool isDeleted = await this.BoardGameServices.Delete(id);

        //    return new RestResponse<bool>()
        //    {
        //        Data = isDeleted,       //* Change to provide Deleted BoardGame
        //        Links = new List<ResponseLink>
        //        {
        //            new ResponseLink(
        //                    Url.Action(
        //                        null,
        //                        "BoardGames",
        //                        id,
        //                        Request.Scheme)!,
        //                    "self",
        //                    "DELETE"),
        //        }
        //    };
        //}
    }

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