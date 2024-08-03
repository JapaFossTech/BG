using BG.Model.Core;
using System.Linq.Expressions;

namespace BG.Serv.Core
{
    public partial class BoardGamesMechanicServices : IBoardGamesMechanicServices
    {
        private readonly ICoreData _coreData;
        private IBoardGamesMechanicRepository Repository { get { return _coreData.BoardGamesMechanicRepository; } }

        //* Ctor
        public BoardGamesMechanicServices(ICoreData coreData)
        {
            //Console.WriteLine("BoardGamesMechanicServices.ctor");
            _coreData = coreData;
        }

        public async Task<BoardGamesMechanic?> GetByID(int pkID)
        {
            return await this.GetByID(pkID, false);
        }
        public async Task<BoardGamesMechanic?> GetByID(int pkID, bool doLoadLists)
        {
            //try
            //{
                BoardGamesMechanic? boardGamesMechanic = await this.Repository.GetByID(pkID, doLoadLists);

                return boardGamesMechanic;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public async Task<List<BoardGamesMechanic>> GetAll()
        {
            //try
            //{
                List<BoardGamesMechanic> boardGamesMechanics = await Repository.GetAll();

                return boardGamesMechanics;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        public async Task<List<BoardGamesMechanic>> Search(string dataToSearch)
        {
            //try
            //{
            List<BoardGamesMechanic> boardGamesMechanics = await Repository.Search(dataToSearch);

            return boardGamesMechanics;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public async Task<BoardGamesMechanic> Insert(BoardGamesMechanic boardGamesMechanic)
        {
            //try
            //{
            BoardGamesMechanic? createdBoardGamesMechanic = await this.Repository.Insert(boardGamesMechanic);
            boardGamesMechanic.BoardGamesMechanicID = createdBoardGamesMechanic.BoardGamesMechanicID;

            this.Inserted?.Invoke();            //* Using null propagation (will get invoked if not null)

            return boardGamesMechanic;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        /// <summary>
        /// If the model is not stored at the database, the return value will be -1; > 0 otherwise.
        /// </summary>
        /// <param name="boardGamesMechanic"></param>
        /// <returns></returns>
        public async Task<int> Update(BoardGamesMechanic boardGamesMechanic)
        {
            //try
            //{
            int pkid  = await this.Repository.Update(boardGamesMechanic);

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
        public async Task<List<BoardGamesMechanic>> Search(Expression<Func<BoardGamesMechanic, bool>> wherePredicate)   //* parameter used in EF
        {
            return await this.Repository.Search(wherePredicate);
        }

        //* Event definition

        public event Action? Inserted;
        public event Action? Deleted;
    }
}