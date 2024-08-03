using System.Linq.Expressions;
using System.Net.Http.Json;
using BG.Model.Core;
using Infrastructure.Data;

namespace BG.Repo.RestApi.Core
{
    public partial class MechanicRepository : IMechanicRepository
    {
        private readonly HttpClient _HttpClient;

        //* Ctor
        public MechanicRepository(HttpClient httpClient )
        {
            _HttpClient = httpClient;
        }

        public async Task<Mechanic?> GetByID(int pkID)
        {
            return await this.GetByID(pkID, false);
        }
        public async Task<Mechanic?> GetByID(int pkID, bool doLoadLists)
        {
            try
            {
                if (pkID > 0)
                {
                    if (_HttpClient == null)
                        throw new Exception("_HttpClient is null");
                    else
                    {
                        HttpResponseMessage response = await _HttpClient.GetAsync($"Core/Mechanics/{pkID}");

                        //* response.EnsureSuccessStatusCode();   //throws exception if response.IsSuccessStatusCode = false

                        if (response.IsSuccessStatusCode)
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                            {
                                return default;
                            }

                            return await response.Content.ReadFromJsonAsync<Mechanic>();
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
        public async Task<List<Mechanic>> GetAll()
        {
            try
            {
                if (_HttpClient == null)
                    throw new Exception("_HttpClient is null");
                else
                {
                    //Console.WriteLine("MechanicRepository.GetAll: awaiting httpClient.GetAsync()");
                    HttpResponseMessage response = await _HttpClient.GetAsync($"Core/Mechanics");
                    //Console.WriteLine("MechanicRepository.GetAll Mechanics: DONE with httpClient.GetAsync()");

                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        {
                            //* The API did not return a body. Ignore
                            //return Enumerable.Empty<Mechanic>();
                        }
                        else
                        {
                            List<Mechanic>? mechanics = await response.Content.ReadFromJsonAsync<List<Mechanic>>();

                            if (mechanics != null)
                                return mechanics;
                        }
                    }
                    else
                    {
                        string message = await response.Content.ReadAsStringAsync();
                        throw new Exception(message);
                    }
                }
                
                return new List<Mechanic>();      //* return an empty list if data is not found
            }
            catch (Exception)
            {
                //* Log exception
                throw;
            }
        }
public async Task<List<Mechanic>> Search(string? dataToSearch)
{
    try
    {
        if (_HttpClient == null)
            throw new Exception("_HttpClient is null");
        else
        {
            //* api/Mlb/Mechanics/Search?dataToSearch=Something

            HttpResponseMessage response = await _HttpClient
                .GetAsync($"Core/Mechanics/Search?dataToSearch={dataToSearch}");

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    //* The API did not return a body. Ignore
                    //return Enumerable.Empty<Mechanic>();
                }
                else
                {
                    List<Mechanic>? mechanics = await response.Content.ReadFromJsonAsync<List<Mechanic>>();

                    if (mechanics != null)
                        return mechanics;
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
public async Task<Mechanic> Insert(Mechanic mechanic)
{
    try
    {
        if (mechanic.MechanicID <= 0)
        {
            if (_HttpClient == null)
                throw new Exception("_HttpClient is null");
            else
            {
                HttpResponseMessage response = await _HttpClient.PostAsJsonAsync(
                                                        "Core/Mechanics", mechanic);

                if (response.IsSuccessStatusCode)
                {
                    this.Inserted?.Invoke();            //* Using null propagation (will get invoked if not null)

                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return mechanic;
                    }

                    Mechanic? insertedMechanic = await response.Content.ReadFromJsonAsync<Mechanic>();

                    return insertedMechanic ?? mechanic;
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
            string message = $"MechanicServices.Insert.Ex: Provided mechanic should not have a pkid and it has: " +
                $"{mechanic.MechanicID}";
            throw new Exception(message);
        }

    }
    catch (Exception)
    {
        //* Log exception
        throw;
    }
}
public async Task<int> Update(Mechanic mechanic)
{
    try
    {
        if (mechanic.MechanicID > 0)
        {
            if (_HttpClient == null)
                throw new Exception("_HttpClient is null");
            else
            {
                HttpResponseMessage response = await _HttpClient.PutAsJsonAsync(
                                                $"Core/Mechanics/{mechanic.MechanicID}", mechanic);

                if (response.IsSuccessStatusCode)
                {
                    return mechanic.MechanicID;
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
            string message = $"MechanicServices.Update.Ex: Provided mechanic should have a pkid value and it has: " +
                $"{mechanic.MechanicID}";
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
                HttpResponseMessage response = await _HttpClient.DeleteAsync($"Core/Mechanics/{pkID}");

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
public Task<List<Mechanic>> Search(Expression<Func<Mechanic, bool>> wherePredicate)
{
    throw new NotImplementedException();
}

//* Event definition

public event Action? Inserted;
public event Action? Deleted;

}
}