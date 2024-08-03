using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BG.Model.Core;
using Infrastructure.Extensions;

namespace BG.Repo.EF.Core
{
    public class BoardGameRepository : IBoardGameRepository
    {
        private readonly BGDbContext _db;

        private static Expression<Func<BoardGame, Object>>? 
            ConvertToKeySelectorExpression(string? fromColumnName)
        {
            if (fromColumnName == null)
                return null;

            Expression<Func<BoardGame, Object>>? keySelector = null;

            switch (fromColumnName)
            {
                case "Name": keySelector = boardGame => boardGame.Name!; break;
                case "Year": keySelector = boardGame => boardGame.Year; break;
                case "RatingAverage": keySelector = boardGame => boardGame.RatingAverage; break;
                case "BGGRank": keySelector = boardGame => boardGame.BGGRank; break;
                case "ComplexityAverage": keySelector = boardGame => boardGame.ComplexityAverage; break;
                case "OwnedUsers": keySelector = boardGame => boardGame.OwnedUsers; break;
            }

            return keySelector;
        }

        public event Action? Inserted;
        public event Action? Deleted;

        public BoardGameRepository(BGDbContext dbContext)
        {
            _db = dbContext;
        }
        public Task<BoardGame?> GetByID(int pkID)
        {
            return this.GetByID(pkID, false);
        }
        public async Task<BoardGame?> GetByID(int pkID, bool doLoadLists)
        {
            BoardGame? boardGame = null;

            if (doLoadLists)
            {
                boardGame = await _db.BoardGames
               .Include(boardGame => boardGame.BoardGamesDomains)
               .Include(boardGame => boardGame.BoardGamesMechanics)
                .FirstOrDefaultAsync(boardGame => boardGame.BoardGameID == pkID);
            }
            else
            {
                boardGame = await _db.BoardGames
                .FirstOrDefaultAsync(boardGame => boardGame.BoardGameID == pkID);
            }

            return boardGame;
        }
        public async Task<List<BoardGame>> GetAll()
        {
            //DbSet<BoardGame> query = _db.BoardGames;
            //var data = await query.ToArrayAsync();
            //return data.ToList();

            List<BoardGame> boardGameList = await _db
                .BoardGames
                .Include(boardGame => boardGame.BoardGamesDomains)
                .Include(boardGame => boardGame.BoardGamesMechanics)
                //.Where(boardGame => !boardGame.IsDeleted)
                //.OrderBy(boardGame => boardGame.Name)
                .ToListAsync();

            return boardGameList;
        }
        public Task<List<BoardGame>> GetAll_Paged_Sorted_andFiltered(
            int pageIndex = 0
            , int pageSize = 20
            , string? sortColumn = null
            , string sortOrder = "ASC"
            , string? filterQuery = null)
        {

            IQueryable<BoardGame> query = _db.BoardGames.AsQueryable();

            if (filterQuery is not null)
            {
                if (filterQuery.CheckHasData())
                    query = query.Where(boardGame =>
                                        boardGame.Name != null 
                                            && boardGame.Name.Contains(filterQuery));
            }

            var sortKeySelector = ConvertToKeySelectorExpression(sortColumn);

            //* The order to configure IQueryable is important
            //* If OrderBy is configured last, it will not work as expected

            if (sortKeySelector is not null)
            {
                if (sortOrder == "ASC")
                    query = query.OrderBy(sortKeySelector);
                else
                    query = query.OrderByDescending(sortKeySelector);
            }

            query = query
                    //.OrderBy(keySelector)
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .Include(boardGame => boardGame.BoardGamesDomains)
                    //.ThenInclude(boardGamesDomain => boardGamesDomain.Domain)
                    .Include(boardGame => boardGame.BoardGamesMechanics)
            ;

            return query.ToListAsync();
        }
        public async Task<int> GetAllRecordCount()
        {
            return await _db.BoardGames.CountAsync();
        }
        public async Task<List<BoardGame>> Search(string dataToSearch)
        {
            //List<BoardGame> boardGameList = await _db.BoardGames.FromSql(
            //    $"select * from vwBoardGames where {dataToSearch}")
            //    .ToListAsync();

            List<BoardGame> boardGameList = await this.Search(boardGame =>
              (boardGame.Name != null && boardGame.Name.Contains(dataToSearch))
            );

            return boardGameList;
        }
        public async Task<List<BoardGame>> Search(Expression<Func<BoardGame, bool>> wherePredicate)
        {
            List<BoardGame> boardGameList = await _db
                .BoardGames
               .Include(boardGame => boardGame.BoardGamesDomains)
               .Include(boardGame => boardGame.BoardGamesMechanics)
                .Where(wherePredicate)
                .ToListAsync();

            return boardGameList;
        }
        public async Task<BoardGame> Insert(BoardGame model)
        {
            await _db.BoardGames.AddAsync(model);
            await _db.SaveChangesAsync();

            if (model.BoardGameID <= 0)
                throw new Exception("Could not create the boardGame as expected");

            this.Inserted?.Invoke();

            return model;
        }
        public async Task<int> Update_Obsolete(BoardGame model)
        {
            var dbBoardGame = await _db
                .BoardGames
                    //.Include(x => x.ItemsGenres)
                    //.Include(x => x.ItemsPlayers)
                    .FirstOrDefaultAsync(x => x.BoardGameID == model.BoardGameID)
                    ?? throw new Exception("Item not found");

            dbBoardGame.Name = model.Name;
            dbBoardGame.Year = model.Year;
            dbBoardGame.MinPlayers = model.MinPlayers;
            dbBoardGame.MaxPlayers = model.MaxPlayers;
            dbBoardGame.PlayTime = model.PlayTime;
            dbBoardGame.MinAge = model.MinAge;
            dbBoardGame.UsersRated = model.UsersRated;
            dbBoardGame.RatingAverage = model.RatingAverage;
            dbBoardGame.BGGRank = model.BGGRank;
            dbBoardGame.ComplexityAverage = model.ComplexityAverage;
            dbBoardGame.OwnedUsers = model.OwnedUsers;
            //if (model.ItemsGenres != null)
            //{
            //    dbItem.ItemsGenres = model.ItemsGenres;
            //}
            //if (model.ItemsPlayers != null)
            //{
            //    dbItem.ItemsPlayers = model.ItemsPlayers;
            //}
            await _db.SaveChangesAsync();

            return dbBoardGame.BoardGameID;

            ////* Another way
            //DbSet<BoardGame> dbSet = _db.Set<BoardGame>();
            //dbSet.Attach(model);
            //_db.Entry(model).State = EntityState.Modified;
            //await _db.SaveChangesAsync();
            //return model.BoardGameID;
        }
        public async Task<int> Update(BoardGame model)
        { 
            //* model should be ready to update.
            //* No field assigments should happen in this method

            _db.BoardGames.Update(model);
            await _db.SaveChangesAsync();

            return model.BoardGameID;

            ////* Another way
            //DbSet<BoardGame> dbSet = _db.Set<BoardGame>();
            //dbSet.Attach(model);
            //_db.Entry(model).State = EntityState.Modified;
            //await _db.SaveChangesAsync();
            //return model.BoardGameID;
        }
        //public async Task<bool> Update(BoardGameDTO boardGame)
        //{
        //}
        public async Task<bool> Delete(int pkID)
        {
            //* We get a 'Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException'
            //* If the pkID does not exist in the DB
            //var boardGame = new BoardGame() { BoardGameID = pkID };
            //_db.BoardGames.Attach(boardGame);
            //_db.Remove(boardGame);
            //int removedID = await _db.SaveChangesAsync();
            //return removedID > 0;

            //* Another way
            BoardGame? storedBoardGame = await this.GetByID(pkID);

            if (storedBoardGame == null)
                return false;
            {
                _db.BoardGames.Remove(storedBoardGame);
                await _db.SaveChangesAsync();

                this.Deleted?.Invoke();

                return true;
            };
        }

    }
}