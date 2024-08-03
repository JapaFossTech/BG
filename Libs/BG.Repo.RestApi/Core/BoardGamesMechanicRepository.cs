using System.Linq.Expressions;
using System.Net.Http.Json;
using BG.Model.Core;
using Infrastructure.Data;

namespace BG.Repo.RestApi.Core
{
    public partial class BoardGamesMechanicRepository : IBoardGamesMechanicRepository
    {
        private readonly HttpClient _HttpClient;

        //* Ctor
        public BoardGamesMechanicRepository(HttpClient httpClient )
        {
            _HttpClient = httpClient;
        }

        public async Task<BoardGamesMechanic?> GetByID(int pkID)
        {
            return await this.GetByID(pkID, false);
        }
        public async Task<BoardGamesMechanic?> GetByID(int pkID, bool doLoadLists)
        {
            try
            {
                if (pkID > 0)
                {
                    if (_HttpClient == null)
                        throw new Exception("_HttpClient is null");
                    else
                    {
                        HttpResponseMessage response = await _HttpClient.GetAsync($"Core/BoardGamesMechanics/{pkID}");

                        //* response.EnsureSuccessStatusCode();   //throws exception if response.IsSuccessStatusCode = false

                        if (response.IsSuccessStatusCode)
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                            {
                                return default;
                            }

                            return await response.Content.ReadFromJsonAsync<BoardGamesMechanic>();
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
        public async Task<List<BoardGamesMechanic>> GetAll()
        {
            try
            {
                if (_HttpClient == null)
                    throw new Exception("_HttpClient is null");
                else
                {
                    //Console.WriteLine("BoardGamesMechanicRepository.GetAll: awaiting httpClient.GetAsync()");
                    HttpResponseMessage response = await _HttpClient.GetAsync($"Core/BoardGamesMechanics");
                    //Console.WriteLine("BoardGamesMechanicRepository.GetAll BoardGamesMechanics: DONE with httpClient.GetAsync()");

                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        {
                            //* The API did not return a body. Ignore
                            //return Enumerable.Empty<BoardGamesMechanic>();
                        }
                        else
                        {
                            List<BoardGamesMechanic>? boardGamesMechanics = await response.Content.ReadFromJsonAsync<List<BoardGamesMechanic>>();

                            if (boardGamesMechanics != null)
                                return boardGamesMechanics;
                        }
                    }
                    else
                    {
                        string message = await response.Content.ReadAsStringAsync();
                        throw new Exception(message);
                    }
                }
                
                return new List<BoardGamesMechanic>();      //* return an empty list if data is not found
            }
            catch (Exception)
            {
                //* Log exception
                throw;
            }
        }
public async Task<List<BoardGamesMechanic>> Search(string? dataToSearch)
{
    try
    {
        if (_HttpClient == null)
            throw new Exception("_HttpClient is null");
        else
        {
            //* api/Mlb/BoardGamesMechanics/Search?dataToSearch=Something

            HttpResponseMessage response = await _HttpClient
                .GetAsync($"Core/BoardGamesMechanics/Search?dataToSearch={dataToSearch}");

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    //* The API did not return a body. Ignore
                    //return Enumerable.Empty<BoardGamesMechanic>();
                }
                else
                {
                    List<BoardGamesMechanic>? boardGamesMechanics = await response.Content.ReadFromJsonAsync<List<BoardGamesMechanic>>();

                    if (boardGamesMechanics != null)
                        return boardGamesMechanics;
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
public async Task<BoardGamesMechanic> Insert(BoardGamesMechanic boardGamesMechanic)
{
    try
    {
        if (boardGamesMechanic.BoardGamesMechanicID <= 0)
        {
            if (_HttpClient == null)
                throw new Exception("_HttpClient is null");
            else
            {
                HttpResponseMessage response = await _HttpClient.PostAsJsonAsync(
                                                        "Core/BoardGamesMechanics", boardGamesMechanic);

                if (response.IsSuccessStatusCode)
                {
                    this.Inserted?.Invoke();            //* Using null propagation (will get invoked if not null)

                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return boardGamesMechanic;
                    }

                    BoardGamesMechanic? insertedBoardGamesMechanic = await response.Content.ReadFromJsonAsync<BoardGamesMechanic>();

                    return insertedBoardGamesMechanic ?? boardGamesMechanic;
                }
                else
                {
                    string message = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Http status: {response.StatusCode}, Message: {message}");
                }
            }
        }
        else
        {
            string message = $"BoardGamesMechanicServices.Insert.Ex: Provided boardGamesMechanic should not have a pkid and it has: " +
                $"{boardGamesMechanic.BoardGamesMechanicID}";
            throw new Exception(message);
        }

    }
    catch (Exception)
    {
        //* Log exception
        throw;
    }
}
public async Task<int> Update(BoardGamesMechanic boardGamesMechanic)
{
    try
    {
        if (boardGamesMechanic.BoardGamesMechanicID > 0)
        {
            if (_HttpClient == null)
                throw new Exception("_HttpClient is null");
            else
            {
                HttpResponseMessage response = await _HttpClient.PutAsJsonAsync(
                                                $"Core/BoardGamesMechanics/{boardGamesMechanic.BoardGamesMechanicID}", boardGamesMechanic);

                if (response.IsSuccessStatusCode)
                {
                    return boardGamesMechanic.BoardGamesMechanicID;
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
            string message = $"BoardGamesMechanicServices.Update.Ex: Provided boardGamesMechanic should have a pkid value and it has: " +
                $"{boardGamesMechanic.BoardGamesMechanicID}";
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
                HttpResponseMessage response = await _HttpClient.DeleteAsync($"Core/BoardGamesMechanics/{pkID}");

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
public Task<List<BoardGamesMechanic>> Search(Expression<Func<BoardGamesMechanic, bool>> wherePredicate)
{
    throw new NotImplementedException();
}

//* Event definition

public event Action? Inserted;
public event Action? Deleted;

}
}