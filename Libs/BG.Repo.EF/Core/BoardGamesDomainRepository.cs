using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BG.Model.Core;

namespace BG.Repo.EF.Core
{
public class BoardGamesDomainRepository : IBoardGamesDomainRepository
{
            private readonly BGDbContext _db;

        public event Action? Inserted;
        public event Action? Deleted;

        public BoardGamesDomainRepository(BGDbContext dbContext)
        {
            _db = dbContext;
        }
            public Task<BoardGamesDomain?> GetByID(int pkID)
{
    return this.GetByID(pkID, false);
}
public async Task<BoardGamesDomain?> GetByID(int pkID, bool doLoadLists)
{
    BoardGamesDomain? boardGamesDomain = null;

    if (doLoadLists)
    {
        boardGamesDomain = await _db.BoardGamesDomains
       .Include(boardGamesDomain => boardGamesDomain.BoardGame)
       .Include(boardGamesDomain => boardGamesDomain.Domain)
        .FirstOrDefaultAsync(boardGamesDomain => boardGamesDomain.BoardGamesDomainID == pkID);
    }
    else
    {
        boardGamesDomain = await _db.BoardGamesDomains
        .FirstOrDefaultAsync(boardGamesDomain => boardGamesDomain.BoardGamesDomainID == pkID);
    }

    return boardGamesDomain;
}
public async Task<List<BoardGamesDomain>> GetAll()
{
    List<BoardGamesDomain> boardGamesDomainList = await _db
        .BoardGamesDomains
       .Include(boardGamesDomain => boardGamesDomain.BoardGame)
       .Include(boardGamesDomain => boardGamesDomain.Domain)
        //.Where(boardGamesDomain => !boardGamesDomain.IsDeleted)
        //.OrderBy(boardGamesDomain => boardGamesDomain.Name)
        .ToListAsync();

    return boardGamesDomainList;
}
    public async Task<List<BoardGamesDomain>> Search(string dataToSearch)
{
    //List<BoardGamesDomain> boardGamesDomainList = await _db.BoardGamesDomains.FromSql(
    //    $"select * from vwBoardGamesDomains where {dataToSearch}")
    //    .ToListAsync();

    List<BoardGamesDomain> boardGamesDomainList = await this.GetAll();   //* No field is searchable

    return boardGamesDomainList;
}
public async Task<List<BoardGamesDomain>> Search(Expression<Func<BoardGamesDomain, bool>> wherePredicate)
{
    List<BoardGamesDomain> boardGamesDomainList = await _db
        .BoardGamesDomains
       .Include(boardGamesDomain => boardGamesDomain.BoardGame)
       .Include(boardGamesDomain => boardGamesDomain.Domain)
        .Where(wherePredicate)
        .ToListAsync();

    return boardGamesDomainList;
}
    public async Task<BoardGamesDomain> Insert(BoardGamesDomain model)
{
    await _db.BoardGamesDomains.AddAsync(model);
    await _db.SaveChangesAsync();

    if (model.BoardGamesDomainID <= 0) 
        throw new Exception("Could not create the boardGamesDomain as expected");

    this.Inserted?.Invoke();

    return model;
}
    public async Task<int> Update(BoardGamesDomain model)
{
    var dbBoardGamesDomain = await _db
        .BoardGamesDomains
            //.Include(x => x.ItemsGenres)
            //.Include(x => x.ItemsPlayers)
            .FirstOrDefaultAsync(x => x.BoardGamesDomainID == model.BoardGamesDomainID) 
            ?? throw new Exception("Item not found");
    
    //if (model.ItemsGenres != null)
    //{
    //    dbItem.ItemsGenres = model.ItemsGenres;
    //}
    //if (model.ItemsPlayers != null)
    //{
    //    dbItem.ItemsPlayers = model.ItemsPlayers;
    //}
    await _db.SaveChangesAsync();

    return dbBoardGamesDomain.BoardGamesDomainID;

    ////* Another way
    //DbSet<BoardGamesDomain> dbSet = _db.Set<BoardGamesDomain>();
    //dbSet.Attach(model);
    //_db.Entry(model).State = EntityState.Modified;
    //await _db.SaveChangesAsync();
    //return model.BoardGamesDomainID;
}
    public async Task<bool> Delete(int pkID)
{
    //* We get a 'Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException'
    //var boardGamesDomain = new BoardGamesDomain() { BoardGamesDomainID = pkID };
    //_db.BoardGamesDomains.Attach(boardGamesDomain);
    //_db.Remove(boardGamesDomain);
    //int removedID = await _db.SaveChangesAsync();
    //return removedID > 0;

    BoardGamesDomain? storedBoardGamesDomain = await this.GetByID(pkID);

    if (storedBoardGamesDomain == null)
        return false;
    {
        _db.BoardGamesDomains.Remove(storedBoardGamesDomain);
        await _db.SaveChangesAsync();

        this.Deleted?.Invoke();

        return true;
    };
}
}
}