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
public class BoardGamesMechanicRepository : IBoardGamesMechanicRepository
{
            private readonly BGDbContext _db;

        public event Action? Inserted;
        public event Action? Deleted;

        public BoardGamesMechanicRepository(BGDbContext dbContext)
        {
            _db = dbContext;
        }
            public Task<BoardGamesMechanic?> GetByID(int pkID)
{
    return this.GetByID(pkID, false);
}
public async Task<BoardGamesMechanic?> GetByID(int pkID, bool doLoadLists)
{
    BoardGamesMechanic? boardGamesMechanic = null;

    if (doLoadLists)
    {
        boardGamesMechanic = await _db.BoardGamesMechanics
       .Include(boardGamesMechanic => boardGamesMechanic.BoardGame)
       .Include(boardGamesMechanic => boardGamesMechanic.Mechanic)
        .FirstOrDefaultAsync(boardGamesMechanic => boardGamesMechanic.BoardGamesMechanicID == pkID);
    }
    else
    {
        boardGamesMechanic = await _db.BoardGamesMechanics
        .FirstOrDefaultAsync(boardGamesMechanic => boardGamesMechanic.BoardGamesMechanicID == pkID);
    }

    return boardGamesMechanic;
}
public async Task<List<BoardGamesMechanic>> GetAll()
{
    List<BoardGamesMechanic> boardGamesMechanicList = await _db
        .BoardGamesMechanics
       .Include(boardGamesMechanic => boardGamesMechanic.BoardGame)
       .Include(boardGamesMechanic => boardGamesMechanic.Mechanic)
        //.Where(boardGamesMechanic => !boardGamesMechanic.IsDeleted)
        //.OrderBy(boardGamesMechanic => boardGamesMechanic.Name)
        .ToListAsync();

    return boardGamesMechanicList;
}
    public async Task<List<BoardGamesMechanic>> Search(string dataToSearch)
{
    //List<BoardGamesMechanic> boardGamesMechanicList = await _db.BoardGamesMechanics.FromSql(
    //    $"select * from vwBoardGamesMechanics where {dataToSearch}")
    //    .ToListAsync();

    List<BoardGamesMechanic> boardGamesMechanicList = await this.GetAll();   //* No field is searchable

    return boardGamesMechanicList;
}
public async Task<List<BoardGamesMechanic>> Search(Expression<Func<BoardGamesMechanic, bool>> wherePredicate)
{
    List<BoardGamesMechanic> boardGamesMechanicList = await _db
        .BoardGamesMechanics
       .Include(boardGamesMechanic => boardGamesMechanic.BoardGame)
       .Include(boardGamesMechanic => boardGamesMechanic.Mechanic)
        .Where(wherePredicate)
        .ToListAsync();

    return boardGamesMechanicList;
}
    public async Task<BoardGamesMechanic> Insert(BoardGamesMechanic model)
{
    await _db.BoardGamesMechanics.AddAsync(model);
    await _db.SaveChangesAsync();

    if (model.BoardGamesMechanicID <= 0) 
        throw new Exception("Could not create the boardGamesMechanic as expected");

    this.Inserted?.Invoke();

    return model;
}
    public async Task<int> Update(BoardGamesMechanic model)
{
    var dbBoardGamesMechanic = await _db
        .BoardGamesMechanics
            //.Include(x => x.ItemsGenres)
            //.Include(x => x.ItemsPlayers)
            .FirstOrDefaultAsync(x => x.BoardGamesMechanicID == model.BoardGamesMechanicID) 
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

    return dbBoardGamesMechanic.BoardGamesMechanicID;

    ////* Another way
    //DbSet<BoardGamesMechanic> dbSet = _db.Set<BoardGamesMechanic>();
    //dbSet.Attach(model);
    //_db.Entry(model).State = EntityState.Modified;
    //await _db.SaveChangesAsync();
    //return model.BoardGamesMechanicID;
}
    public async Task<bool> Delete(int pkID)
{
    //* We get a 'Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException'
    //var boardGamesMechanic = new BoardGamesMechanic() { BoardGamesMechanicID = pkID };
    //_db.BoardGamesMechanics.Attach(boardGamesMechanic);
    //_db.Remove(boardGamesMechanic);
    //int removedID = await _db.SaveChangesAsync();
    //return removedID > 0;

    BoardGamesMechanic? storedBoardGamesMechanic = await this.GetByID(pkID);

    if (storedBoardGamesMechanic == null)
        return false;
    {
        _db.BoardGamesMechanics.Remove(storedBoardGamesMechanic);
        await _db.SaveChangesAsync();

        this.Deleted?.Invoke();

        return true;
    };
}
}
}