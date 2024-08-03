using BG.Model.Core;

//* Put this on its own .cs file at BG.Serv project

namespace BG.Serv.Core
{
    public class CoreServices : ICoreServices
    {
        private readonly ICoreData _CoreData;

        //* BoardGameServices

        private IBoardGameServices? _BoardGameServices;
        public IBoardGameServices BoardGame
        {
            get
            {
                _BoardGameServices ??= new BoardGameServices(_CoreData);
                return _BoardGameServices;
            }
        }

        //* DomainServices

        private IDomainServices? _DomainServices;
        public IDomainServices Domain
        {
            get
            {
                _DomainServices ??= new DomainServices(_CoreData);
                return _DomainServices;
            }
        }

        //* MechanicServices

        private IMechanicServices? _MechanicServices;
        public IMechanicServices Mechanic
        {
            get
            {
                _MechanicServices ??= new MechanicServices(_CoreData);
                return _MechanicServices;
            }
        }

        //* BoardGamesDomainServices

        private IBoardGamesDomainServices? _BoardGamesDomainServices;
        public IBoardGamesDomainServices BoardGamesDomain
        {
            get
            {
                _BoardGamesDomainServices ??= new BoardGamesDomainServices(_CoreData);
                return _BoardGamesDomainServices;
            }
        }

        //* BoardGamesMechanicServices

        private IBoardGamesMechanicServices? _BoardGamesMechanicServices;
        public IBoardGamesMechanicServices BoardGamesMechanic
        {
            get
            {
                _BoardGamesMechanicServices ??= new BoardGamesMechanicServices(_CoreData);
                return _BoardGamesMechanicServices;
            }
        }


        //* Ctor
        public CoreServices(ICoreData coreData)
        {
            _CoreData = coreData;
        }
    }
}
