using System.Linq.Expressions;
using System.Net.Http.Json;
using BG.Model.Core;
using Infrastructure.Data;

namespace BG.Repo.RestApi.Core
{
    public partial class BoardGamesDomainRepository : IBoardGamesDomainRepository
    {
        private readonly HttpClient _HttpClient;

        //* Ctor
        public BoardGamesDomainRepository(HttpClient httpClient )
        {
            _HttpClient = httpClient;
        }

        public async Task<BoardGamesDomain?> GetByID(int pkID)
        {
            return await this.GetByID(pkID, false);
        }
        public async Task<BoardGamesDomain?> GetByID(int pkID, bool doLoadLists)
        {
            try
            {
                if (pkID > 0)
                {
                    if (_HttpClient == null)
                        throw new Exception("_HttpClient is null");
                    else
                    {
                        HttpResponseMessage response = await _HttpClient.GetAsync($"Core/BoardGamesDomains/{pkID}");

                        //* response.EnsureSuccessStatusCode();   //throws exception if response.IsSuccessStatusCode = false

                        if (response.IsSuccessStatusCode)
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                            {
                                return default;
                            }

                            return await response.Content.ReadFromJsonAsync<BoardGamesDomain>();
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
        public async Task<List<BoardGamesDomain>> GetAll()
        {
            try
            {
                if (_HttpClient == null)
                    throw new Exception("_HttpClient is null");
                else
                {
                    //Console.WriteLine("BoardGamesDomainRepository.GetAll: awaiting httpClient.GetAsync()");
                    HttpResponseMessage response = await _HttpClient.GetAsync($"Core/BoardGamesDomains");
                    //Console.WriteLine("BoardGamesDomainRepository.GetAll BoardGamesDomains: DONE with httpClient.GetAsync()");

                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        {
                            //* The API did not return a body. Ignore
                            //return Enumerable.Empty<BoardGamesDomain>();
                        }
                        else
                        {
                            List<BoardGamesDomain>? boardGamesDomains = await response.Content.ReadFromJsonAsync<List<BoardGamesDomain>>();

                            if (boardGamesDomains != null)
                                return boardGamesDomains;
                        }
                    }
                    else
                    {
                        string message = await response.Content.ReadAsStringAsync();
                        throw new Exception(message);
                    }
                }
                
                return new List<BoardGamesDomain>();      //* return an empty list if data is not found
            }
            catch (Exception)
            {
                //* Log exception
                throw;
            }
        }
public async Task<List<BoardGamesDomain>> Search(string? dataToSearch)
{
    try
    {
        if (_HttpClient == null)
            throw new Exception("_HttpClient is null");
        else
        {
            //* api/Mlb/BoardGamesDomains/Search?dataToSearch=Something

            HttpResponseMessage response = await _HttpClient
                .GetAsync($"Core/BoardGamesDomains/Search?dataToSearch={dataToSearch}");

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    //* The API did not return a body. Ignore
                    //return Enumerable.Empty<BoardGamesDomain>();
                }
                else
                {
                    List<BoardGamesDomain>? boardGamesDomains = await response.Content.ReadFromJsonAsync<List<BoardGamesDomain>>();

                    if (boardGamesDomains != null)
                        return boardGamesDomains;
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
public async Task<BoardGamesDomain> Insert(BoardGamesDomain boardGamesDomain)
{
    try
    {
        if (boardGamesDomain.BoardGamesDomainID <= 0)
        {
            if (_HttpClient == null)
                throw new Exception("_HttpClient is null");
            else
            {
                HttpResponseMessage response = await _HttpClient.PostAsJsonAsync(
                                                        "Core/BoardGamesDomains", boardGamesDomain);

                if (response.IsSuccessStatusCode)
                {
                    this.Inserted?.Invoke();            //* Using null propagation (will get invoked if not null)

                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return boardGamesDomain;
                    }

                    BoardGamesDomain? insertedBoardGamesDomain = await response.Content.ReadFromJsonAsync<BoardGamesDomain>();

                    return insertedBoardGamesDomain ?? boardGamesDomain;
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
            string message = $"BoardGamesDomainServices.Insert.Ex: Provided boardGamesDomain should not have a pkid and it has: " +
                $"{boardGamesDomain.BoardGamesDomainID}";
            throw new Exception(message);
        }

    }
    catch (Exception)
    {
        //* Log exception
        throw;
    }
}
public async Task<int> Update(BoardGamesDomain boardGamesDomain)
{
    try
    {
        if (boardGamesDomain.BoardGamesDomainID > 0)
        {
            if (_HttpClient == null)
                throw new Exception("_HttpClient is null");
            else
            {
                HttpResponseMessage response = await _HttpClient.PutAsJsonAsync(
                                                $"Core/BoardGamesDomains/{boardGamesDomain.BoardGamesDomainID}", boardGamesDomain);

                if (response.IsSuccessStatusCode)
                {
                    return boardGamesDomain.BoardGamesDomainID;
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
            string message = $"BoardGamesDomainServices.Update.Ex: Provided boardGamesDomain should have a pkid value and it has: " +
                $"{boardGamesDomain.BoardGamesDomainID}";
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
                HttpResponseMessage response = await _HttpClient.DeleteAsync($"Core/BoardGamesDomains/{pkID}");

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
public Task<List<BoardGamesDomain>> Search(Expression<Func<BoardGamesDomain, bool>> wherePredicate)
{
    throw new NotImplementedException();
}

//* Event definition

public event Action? Inserted;
public event Action? Deleted;

}
}