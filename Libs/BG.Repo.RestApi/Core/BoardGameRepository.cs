using System.Linq.Expressions;
using System.Net.Http.Json;
using BG.Model.Core;
using Infrastructure.Data;

namespace BG.Repo.RestApi.Core
{
    public partial class BoardGameRepository : IBoardGameRepository
    {
        private readonly HttpClient _HttpClient;

        //* Ctor
        public BoardGameRepository(HttpClient httpClient)
        {
            _HttpClient = httpClient;
        }

        public async Task<BoardGame?> GetByID(int pkID)
        {
            return await this.GetByID(pkID, false);
        }
        public async Task<BoardGame?> GetByID(int pkID, bool doLoadLists)
        {
            try
            {
                if (pkID > 0)
                {
                    if (_HttpClient == null)
                        throw new Exception("_HttpClient is null");
                    else
                    {
                        HttpResponseMessage response = await _HttpClient.GetAsync($"Core/BoardGames/{pkID}");

                        //* response.EnsureSuccessStatusCode();   //throws exception if response.IsSuccessStatusCode = false

                        if (response.IsSuccessStatusCode)
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                            {
                                return default;
                            }

                            return await response.Content.ReadFromJsonAsync<BoardGame>();
                        }
                        else
                        {
                            string message = await response.Content.ReadAsStringAsync();
                            throw new Exception(message);
                        }
                    }
                }

                return default;

            }
            catch (Exception)
            {
                //* Log exception
                throw;
            }
        }
        public async Task<List<BoardGame>> GetAll()
        {
            try
            {
                if (_HttpClient == null)
                    throw new Exception("_HttpClient is null");
                else
                {
                    //Console.WriteLine("BoardGameRepository.GetAll: awaiting httpClient.GetAsync()");
                    HttpResponseMessage response = await _HttpClient.GetAsync($"Core/BoardGames");
                    //Console.WriteLine("BoardGameRepository.GetAll BoardGames: DONE with httpClient.GetAsync()");

                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        {
                            //* The API did not return a body. Ignore
                            //return Enumerable.Empty<BoardGame>();
                        }
                        else
                        {
                            List<BoardGame>? boardGames = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

                            if (boardGames != null)
                                return boardGames;
                        }
                    }
                    else
                    {
                        string message = await response.Content.ReadAsStringAsync();
                        throw new Exception(message);
                    }
                }

                return new List<BoardGame>();      //* return an empty list if data is not found
            }
            catch (Exception)
            {
                //* Log exception
                throw;
            }
        }
        public Task<List<BoardGame>> GetAll_Paged_Sorted_andFiltered(int pageIndex = 0, int pageSize = 20, string? sortColumn = null, string sortOrder = "ASC", string? filterQuery = null)
        {
            throw new NotImplementedException();
        }
        public async Task<int> GetAllRecordCount()
        {
            try
            {
                if (_HttpClient == null)
                    throw new Exception("_HttpClient is null");
                else
                {
                    HttpResponseMessage response = await _HttpClient
                                .GetAsync($"Core/BoardGames/Count");

                    //* response.EnsureSuccessStatusCode();   //throws exception if response.IsSuccessStatusCode = false

                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        {
                            return default;
                        }

                        return await response.Content.ReadFromJsonAsync<int>();
                    }
                    else
                    {
                        string message = await response.Content.ReadAsStringAsync();
                        throw new Exception(message);
                    }
                }

                //return 0;

            }
            catch (Exception)
            {
                //* Log exception
                throw;
            }
        }
        public async Task<List<BoardGame>> Search(string? dataToSearch)
        {
            try
            {
                if (_HttpClient == null)
                    throw new Exception("_HttpClient is null");
                else
                {
                    //* api/Mlb/BoardGames/Search?dataToSearch=Something

                    HttpResponseMessage response = await _HttpClient
                        .GetAsync($"Core/BoardGames/Search?dataToSearch={dataToSearch}");

                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        {
                            //* The API did not return a body. Ignore
                            //return Enumerable.Empty<BoardGame>();
                        }
                        else
                        {
                            List<BoardGame>? boardGames = await response.Content.ReadFromJsonAsync<List<BoardGame>>();

                            if (boardGames != null)
                                return boardGames;
                        }
                    }
                    else
                    {
                        string message = await response.Content.ReadAsStringAsync();
                        throw new Exception(message);
                    }
                }

                return [];      //* return an empty list if data is not found
            }
            catch (Exception)
            {
                //* Log exception
                throw;
            }
        }
        public async Task<BoardGame> Insert(BoardGame boardGame)
        {
            try
            {
                if (boardGame.BoardGameID <= 0)
                {
                    if (_HttpClient == null)
                        throw new Exception("_HttpClient is null");
                    else
                    {
                        HttpResponseMessage response = await _HttpClient
                                                .PostAsJsonAsync(
                                                    "Core/BoardGames", boardGame);

                        if (response.IsSuccessStatusCode)
                        {
                            this.Inserted?.Invoke();            //* Using null propagation (will get invoked if not null)

                            if (response.StatusCode == System.Net
                                                        .HttpStatusCode.NoContent)
                            {
                                return boardGame;
                            }

                            BoardGame? insertedBoardGame = await response.Content
                                                    .ReadFromJsonAsync<BoardGame>();

                            return insertedBoardGame ?? boardGame;
                        }
                        else
                        {
                            string message = await response.Content.ReadAsStringAsync();
                            throw new Exception($"Http status: {response.StatusCode}" +
                                                $", Message: {message}");
                        }
                    }
                }
                else
                {
                    string message = $"BoardGameServices.Insert.Ex: Provided boardGame should not have a pkid and it has: " +
                        $"{boardGame.BoardGameID}";
                    throw new Exception(message);
                }

            }
            catch (Exception)
            {
                //* Log exception
                throw;
            }
        }
        public async Task<int> Update(BoardGame boardGame)
        {
            try
            {
                if (boardGame.BoardGameID > 0)
                {
                    if (_HttpClient == null)
                        throw new Exception("_HttpClient is null");
                    else
                    {
                        HttpResponseMessage response = await _HttpClient.PutAsJsonAsync(
                                                        $"Core/BoardGames/{boardGame.BoardGameID}", boardGame);

                        if (response.IsSuccessStatusCode)
                        {
                            return boardGame.BoardGameID;
                        }
                        else
                        {
                            string message = await response.Content.ReadAsStringAsync();
                            throw new Exception(message);
                        }
                    }
                }
                else
                {
                    string message = $"BoardGameServices.Update.Ex: Provided boardGame should have a pkid value and it has: " +
                        $"{boardGame.BoardGameID}";
                    throw new Exception(message);
                }

            }
            catch (Exception)
            {
                //* Log exception
                throw;
            }
        }
        public async Task<bool> Delete(int pkID)
        {
            try
            {
                if (pkID > 0)
                {
                    if (_HttpClient == null)
                        throw new Exception("_HttpClient is null");
                    else
                    {
                        HttpResponseMessage response = await _HttpClient.DeleteAsync($"Core/BoardGames/{pkID}");

                        if (response.IsSuccessStatusCode)
                        {
                            //* return nothing but invoke any listening event handler

                            this.Deleted?.Invoke();            //* Using null propagation (will get invoked if not null)
                            return true;
                        }
                        else
                        {
                            string message = await response.Content.ReadAsStringAsync();
                            throw new Exception(message);
                        }
                    }
                }

                return false;
            }
            catch (Exception)
            {
                //* Log exception
                throw;
            }
        }
        public Task<List<BoardGame>> Search(Expression<Func<BoardGame, bool>> wherePredicate)
        {
            throw new NotImplementedException();
        }


        //* Event definition

        public event Action? Inserted;
        public event Action? Deleted;

    }
}