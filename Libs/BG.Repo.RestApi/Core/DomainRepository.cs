using System.Linq.Expressions;
using System.Net.Http.Json;
using BG.Model.Core;
using Infrastructure.Data;

namespace BG.Repo.RestApi.Core
{
    public partial class DomainRepository : IDomainRepository
    {
        private readonly HttpClient _HttpClient;

        //* Ctor
        public DomainRepository(HttpClient httpClient )
        {
            _HttpClient = httpClient;
        }

        public async Task<Domain?> GetByID(int pkID)
        {
            return await this.GetByID(pkID, false);
        }
        public async Task<Domain?> GetByID(int pkID, bool doLoadLists)
        {
            try
            {
                if (pkID > 0)
                {
                    if (_HttpClient == null)
                        throw new Exception("_HttpClient is null");
                    else
                    {
                        HttpResponseMessage response = await _HttpClient.GetAsync($"Core/Domains/{pkID}");

                        //* response.EnsureSuccessStatusCode();   //throws exception if response.IsSuccessStatusCode = false

                        if (response.IsSuccessStatusCode)
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                            {
                                return default;
                            }

                            return await response.Content.ReadFromJsonAsync<Domain>();
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
        public async Task<List<Domain>> GetAll()
        {
            try
            {
                if (_HttpClient == null)
                    throw new Exception("_HttpClient is null");
                else
                {
                    //Console.WriteLine("DomainRepository.GetAll: awaiting httpClient.GetAsync()");
                    HttpResponseMessage response = await _HttpClient.GetAsync($"Core/Domains");
                    //Console.WriteLine("DomainRepository.GetAll Domains: DONE with httpClient.GetAsync()");

                    if (response.IsSuccessStatusCode)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                        {
                            //* The API did not return a body. Ignore
                            //return Enumerable.Empty<Domain>();
                        }
                        else
                        {
                            List<Domain>? domains = await response.Content.ReadFromJsonAsync<List<Domain>>();

                            if (domains != null)
                                return domains;
                        }
                    }
                    else
                    {
                        string message = await response.Content.ReadAsStringAsync();
                        throw new Exception(message);
                    }
                }
                
                return new List<Domain>();      //* return an empty list if data is not found
            }
            catch (Exception)
            {
                //* Log exception
                throw;
            }
        }
public async Task<List<Domain>> Search(string? dataToSearch)
{
    try
    {
        if (_HttpClient == null)
            throw new Exception("_HttpClient is null");
        else
        {
            //* api/Mlb/Domains/Search?dataToSearch=Something

            HttpResponseMessage response = await _HttpClient
                .GetAsync($"Core/Domains/Search?dataToSearch={dataToSearch}");

            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    //* The API did not return a body. Ignore
                    //return Enumerable.Empty<Domain>();
                }
                else
                {
                    List<Domain>? domains = await response.Content.ReadFromJsonAsync<List<Domain>>();

                    if (domains != null)
                        return domains;
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
public async Task<Domain> Insert(Domain domain)
{
    try
    {
        if (domain.DomainID <= 0)
        {
            if (_HttpClient == null)
                throw new Exception("_HttpClient is null");
            else
            {
                HttpResponseMessage response = await _HttpClient.PostAsJsonAsync(
                                                        "Core/Domains", domain);

                if (response.IsSuccessStatusCode)
                {
                    this.Inserted?.Invoke();            //* Using null propagation (will get invoked if not null)

                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return domain;
                    }

                    Domain? insertedDomain = await response.Content.ReadFromJsonAsync<Domain>();

                    return insertedDomain ?? domain;
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
            string message = $"DomainServices.Insert.Ex: Provided domain should not have a pkid and it has: " +
                $"{domain.DomainID}";
            throw new Exception(message);
        }

    }
    catch (Exception)
    {
        //* Log exception
        throw;
    }
}
public async Task<int> Update(Domain domain)
{
    try
    {
        if (domain.DomainID > 0)
        {
            if (_HttpClient == null)
                throw new Exception("_HttpClient is null");
            else
            {
                HttpResponseMessage response = await _HttpClient.PutAsJsonAsync(
                                                $"Core/Domains/{domain.DomainID}", domain);

                if (response.IsSuccessStatusCode)
                {
                    return domain.DomainID;
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
            string message = $"DomainServices.Update.Ex: Provided domain should have a pkid value and it has: " +
                $"{domain.DomainID}";
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
                HttpResponseMessage response = await _HttpClient.DeleteAsync($"Core/Domains/{pkID}");

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
public Task<List<Domain>> Search(Expression<Func<Domain, bool>> wherePredicate)
{
    throw new NotImplementedException();
}

//* Event definition

public event Action? Inserted;
public event Action? Deleted;

}
}