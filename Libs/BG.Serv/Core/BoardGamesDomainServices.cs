using BG.Model.Core;
using System.Linq.Expressions;

namespace BG.Serv.Core
{
    public partial class BoardGamesDomainServices : IBoardGamesDomainServices
    {
        private readonly ICoreData _coreData;
        private IBoardGamesDomainRepository Repository { get { return _coreData.BoardGamesDomainRepository; } }

        //* Ctor
        public BoardGamesDomainServices(ICoreData coreData)
        {
            //Console.WriteLine("BoardGamesDomainServices.ctor");
            _coreData = coreData;
        }

        public async Task<BoardGamesDomain?> GetByID(int pkID)
        {
            return await this.GetByID(pkID, false);
        }
        public async Task<BoardGamesDomain?> GetByID(int pkID, bool doLoadLists)
        {
            //try
            //{
                BoardGamesDomain? boardGamesDomain = await this.Repository.GetByID(pkID, doLoadLists);

                return boardGamesDomain;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public async Task<List<BoardGamesDomain>> GetAll()
        {
            //try
            //{
                List<BoardGamesDomain> boardGamesDomains = await Repository.GetAll();

                return boardGamesDomains;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        public async Task<List<BoardGamesDomain>> Search(string dataToSearch)
        {
            //try
            //{
            List<BoardGamesDomain> boardGamesDomains = await Repository.Search(dataToSearch);

            return boardGamesDomains;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public async Task<BoardGamesDomain> Insert(BoardGamesDomain boardGamesDomain)
        {
            //try
            //{
            BoardGamesDomain? createdBoardGamesDomain = await this.Repository.Insert(boardGamesDomain);
            boardGamesDomain.BoardGamesDomainID = createdBoardGamesDomain.BoardGamesDomainID;

            this.Inserted?.Invoke();            //* Using null propagation (will get invoked if not null)

            return boardGamesDomain;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        /// <summary>
        /// If the model is not stored at the database, the return value will be -1; > 0 otherwise.
        /// </summary>
        /// <param name="boardGamesDomain"></param>
        /// <returns></returns>
        public async Task<int> Update(BoardGamesDomain boardGamesDomain)
        {
            //try
            //{
            int pkid  = await this.Repository.Update(boardGamesDomain);

            return pkid;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        public async Task<bool> Delete(int pkID)
        {
            //try
            //{
            bool isDeleted = await this.Repository.Delete(pkID);

            this.Deleted?.Invoke();            //* Using null propagation (will get invoked if not null)

            return isDeleted;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        public async Task<List<BoardGamesDomain>> Search(Expression<Func<BoardGamesDomain, bool>> wherePredicate)   //* parameter used in EF
        {
            return await this.Repository.Search(wherePredicate);
        }

        //* Event definition

        public event Action? Inserted;
        public event Action? Deleted;
    }
}