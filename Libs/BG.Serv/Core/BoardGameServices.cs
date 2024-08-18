using BG.Model.Core;
using PrjBase.Data;
using System.Drawing.Printing;
using System.Linq.Expressions;

namespace BG.Serv.Core
{
    public partial class BoardGameServices : IBoardGameServices
    {
        private readonly ICoreData _coreData;
        private IBoardGameRepository Repository { get { return _coreData.BoardGameRepository; } }

        //* Ctor
        public BoardGameServices(ICoreData coreData)
        {
            //Console.WriteLine("BoardGameServices.ctor");
            _coreData = coreData;
        }

        public async Task<BoardGame?> GetByID(int pkID)
        {
            return await this.GetByID(pkID, false);
        }
        public async Task<BoardGame?> GetByID(int pkID, bool doLoadLists)
        {
            //try
            //{
            BoardGame? boardGame = await this.Repository.GetByID(pkID, doLoadLists);

            return boardGame;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public async Task<List<BoardGame>> GetAll()
        {
            //try
            //{
            List<BoardGame> boardGames = await Repository.GetAll();

            return boardGames;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        public async Task<List<BoardGame>> GetAll_Paged_Sorted_andFiltered(
            int pageIndex = 0, int pageSize = 20
            , string? sortColumn = null, string sortOrder = "ASC"
            , string? filterQuery = null)
        {
            //try
            //{
            List<BoardGame> boardGames = await Repository
                .GetAll_Paged_Sorted_andFiltered(
                    pageIndex, pageSize, sortColumn, sortOrder, filterQuery);

            return boardGames;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        //public async Task<List<BoardGame>> GetAll_Paged_Sorted_andFiltered(
        //                        GetDataRequest<BoardGame> getDataRequest)
        //{
        //    //try
        //    //{
        //    List<BoardGame> boardGames = await Repository
        //        .GetAll_Paged_Sorted_andFiltered(getDataRequest);

        //    return boardGames;

        //    //}
        //    //catch (Exception)
        //    //{
        //    //    throw;
        //    //}
        //}
        public async Task<int> GetAllRecordCount()
        {
            //try
            //{
            int count = await Repository.GetAllRecordCount();

            return count;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        public async Task<List<BoardGame>> Search(string dataToSearch)
        {
            //try
            //{
            List<BoardGame> boardGames = await Repository.Search(dataToSearch);

            return boardGames;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }

        public async Task<BoardGame> Insert(BoardGame boardGame)
        {
            //try
            //{
            BoardGame? createdBoardGame = await this.Repository.Insert(boardGame);
            boardGame.BoardGameID = createdBoardGame.BoardGameID;

            this.Inserted?.Invoke();            //* Using null propagation (will get invoked if not null)

            return boardGame;

            //}
            //catch (Exception)
            //{
            //    throw;
            //}
        }
        /// <summary>
        /// If the model is not stored at the database, the return value will be -1; > 0 otherwise.
        /// </summary>
        /// <param name="boardGame"></param>
        /// <returns></returns>
        public async Task<int> Update(BoardGame boardGame)
        {
            //try
            //{
            int pkid = await this.Repository.Update(boardGame);

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
        public async Task<List<BoardGame>> Search(Expression<Func<BoardGame, bool>> wherePredicate)   //* parameter used in EF
        {
            return await this.Repository.Search(wherePredicate);
        }

        



        //* Event definition

        public event Action? Inserted;
        public event Action? Deleted;
    }
}