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
public class MechanicRepository : IMechanicRepository
{
            private readonly BGDbContext _db;

        public event Action? Inserted;
        public event Action? Deleted;

        public MechanicRepository(BGDbContext dbContext)
        {
            _db = dbContext;
        }
            public Task<Mechanic?> GetByID(int pkID)
{
    return this.GetByID(pkID, false);
}
public async Task<Mechanic?> GetByID(int pkID, bool doLoadLists)
{
    Mechanic? mechanic = null;

    if (doLoadLists)
    {
        mechanic = await _db.Mechanics
       .Include(mechanic => mechanic.BoardGamesMechanics)
        .FirstOrDefaultAsync(mechanic => mechanic.MechanicID == pkID);
    }
    else
    {
        mechanic = await _db.Mechanics
        .FirstOrDefaultAsync(mechanic => mechanic.MechanicID == pkID);
    }

    return mechanic;
}
public async Task<List<Mechanic>> GetAll()
{
    List<Mechanic> mechanicList = await _db
        .Mechanics
       .Include(mechanic => mechanic.BoardGamesMechanics)
        //.Where(mechanic => !mechanic.IsDeleted)
        //.OrderBy(mechanic => mechanic.Name)
        .ToListAsync();

    return mechanicList;
}
    public async Task<List<Mechanic>> Search(string dataToSearch)
{
    //List<Mechanic> mechanicList = await _db.Mechanics.FromSql(
    //    $"select * from vwMechanics where {dataToSearch}")
    //    .ToListAsync();

    List<Mechanic> mechanicList = await this.GetAll();   //* No field is searchable

    return mechanicList;
}
public async Task<List<Mechanic>> Search(Expression<Func<Mechanic, bool>> wherePredicate)
{
    List<Mechanic> mechanicList = await _db
        .Mechanics
       .Include(mechanic => mechanic.BoardGamesMechanics)
        .Where(wherePredicate)
        .ToListAsync();

    return mechanicList;
}
    public async Task<Mechanic> Insert(Mechanic model)
{
    await _db.Mechanics.AddAsync(model);
    await _db.SaveChangesAsync();

    if (model.MechanicID <= 0) 
        throw new Exception("Could not create the mechanic as expected");

    this.Inserted?.Invoke();

    return model;
}
    public async Task<int> Update(Mechanic model)
{
    var dbMechanic = await _db
        .Mechanics
            //.Include(x => x.ItemsGenres)
            //.Include(x => x.ItemsPlayers)
            .FirstOrDefaultAsync(x => x.MechanicID == model.MechanicID) 
            ?? throw new Exception("Item not found");
    
dbMechanic.MechanicDesc = model.MechanicDesc;
    //if (model.ItemsGenres != null)
    //{
    //    dbItem.ItemsGenres = model.ItemsGenres;
    //}
    //if (model.ItemsPlayers != null)
    //{
    //    dbItem.ItemsPlayers = model.ItemsPlayers;
    //}
    await _db.SaveChangesAsync();

    return dbMechanic.MechanicID;

    ////* Another way
    //DbSet<Mechanic> dbSet = _db.Set<Mechanic>();
    //dbSet.Attach(model);
    //_db.Entry(model).State = EntityState.Modified;
    //await _db.SaveChangesAsync();
    //return model.MechanicID;
}
    public async Task<bool> Delete(int pkID)
{
    //* We get a 'Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException'
    //var mechanic = new Mechanic() { MechanicID = pkID };
    //_db.Mechanics.Attach(mechanic);
    //_db.Remove(mechanic);
    //int removedID = await _db.SaveChangesAsync();
    //return removedID > 0;

    Mechanic? storedMechanic = await this.GetByID(pkID);

    if (storedMechanic == null)
        return false;
    {
        _db.Mechanics.Remove(storedMechanic);
        await _db.SaveChangesAsync();

        this.Deleted?.Invoke();

        return true;
    };
}
}
}