using BG.Model.Core;

namespace BG.Repo.EF.Core
{
    public class CoreData : ICoreData
    {
        private readonly BGDbContext _DBContext;

        //* BoardGameRepository
        private IBoardGameRepository? _BoardGameRepository;
        public IBoardGameRepository BoardGameRepository
        {
            get
            {
                _BoardGameRepository ??= new BoardGameRepository(_DBContext);
                return _BoardGameRepository;
            }
        }

        //* DomainRepository
        private IDomainRepository? _DomainRepository;
        public IDomainRepository DomainRepository
        {
            get
            {
                _DomainRepository ??= new DomainRepository(_DBContext);
                return _DomainRepository;
            }
        }

        //* MechanicRepository
        private IMechanicRepository? _MechanicRepository;
        public IMechanicRepository MechanicRepository
        {
            get
            {
                _MechanicRepository ??= new MechanicRepository(_DBContext);
                return _MechanicRepository;
            }
        }

        //* BoardGamesDomainRepository
        private IBoardGamesDomainRepository? _BoardGamesDomainRepository;
        public IBoardGamesDomainRepository BoardGamesDomainRepository
        {
            get
            {
                _BoardGamesDomainRepository ??= new BoardGamesDomainRepository(_DBContext);
                return _BoardGamesDomainRepository;
            }
        }

        //* BoardGamesMechanicRepository
        private IBoardGamesMechanicRepository? _BoardGamesMechanicRepository;
        public IBoardGamesMechanicRepository BoardGamesMechanicRepository
        {
            get
            {
                _BoardGamesMechanicRepository ??= new BoardGamesMechanicRepository(_DBContext);
                return _BoardGamesMechanicRepository;
            }
        }

        //* Ctor
        public CoreData() : this(new BGDbContext()) { }
        public CoreData(BGDbContext dbContext)
        {
            _DBContext = dbContext;
        }
    }
}   
