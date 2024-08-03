using BG.Model.Core;

namespace BG.Repo.RestApi.Core
{
    public class CoreData : ICoreData
    {
        private readonly HttpClient _HttpClient;

//* BoardGameRepository
private IBoardGameRepository? _BoardGameRepository;
public IBoardGameRepository BoardGameRepository
{
    get
    {
        _BoardGameRepository ??= new BoardGameRepository(_HttpClient);
        return _BoardGameRepository;
    }
}

//* DomainRepository
private IDomainRepository? _DomainRepository;
public IDomainRepository DomainRepository
{
    get
    {
        _DomainRepository ??= new DomainRepository(_HttpClient);
        return _DomainRepository;
    }
}

//* MechanicRepository
private IMechanicRepository? _MechanicRepository;
public IMechanicRepository MechanicRepository
{
    get
    {
        _MechanicRepository ??= new MechanicRepository(_HttpClient);
        return _MechanicRepository;
    }
}

//* BoardGamesDomainRepository
private IBoardGamesDomainRepository? _BoardGamesDomainRepository;
public IBoardGamesDomainRepository BoardGamesDomainRepository
{
    get
    {
        _BoardGamesDomainRepository ??= new BoardGamesDomainRepository(_HttpClient);
        return _BoardGamesDomainRepository;
    }
}

//* BoardGamesMechanicRepository
private IBoardGamesMechanicRepository? _BoardGamesMechanicRepository;
public IBoardGamesMechanicRepository BoardGamesMechanicRepository
{
    get
    {
        _BoardGamesMechanicRepository ??= new BoardGamesMechanicRepository(_HttpClient);
        return _BoardGamesMechanicRepository;
    }
}

        //* Ctor
        public CoreData(HttpClient httpClient)
        {
            _HttpClient = httpClient;
        }
    }
}        
