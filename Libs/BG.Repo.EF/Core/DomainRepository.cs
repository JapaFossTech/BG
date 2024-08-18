using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using BG.Model.Core;
using Infrastructure.Extensions;

namespace BG.Repo.EF.Core;

public class DomainRepository : IDomainRepository
{
    private static Expression<Func<Domain, Object>>?
                                      GetOrderByKeySelector(string? fromColumnName)
    {
        if (fromColumnName == null)
            return null;

        Expression<Func<Domain, Object>>? keySelector = null;

        switch (fromColumnName)
        {

            case "DomainDesc": keySelector = domain => domain.DomainDesc!; break;
        }

        return keySelector;
    }
    private readonly BGDbContext _db;

    public event Action? Inserted;
    public event Action? Deleted;

    public DomainRepository(BGDbContext dbContext)
    {
        _db = dbContext;
    }

    public Task<Domain?> GetByID(int pkID)
    {
        return this.GetByID(pkID, false);
    }
    public async Task<Domain?> GetByID(int pkID, bool doLoadLists)
    {
        Domain? domain = null;

        if (doLoadLists)
        {
            domain = await _db.Domains
       .Include(domain => domain.BoardGamesDomains)
            .FirstOrDefaultAsync(domain => domain.DomainID == pkID);
        }
        else
        {
            domain = await _db.Domains
            .FirstOrDefaultAsync(domain => domain.DomainID == pkID);
        }

        return domain;
    }
    public async Task<List<Domain>> GetAll()
    {
        List<Domain> domainList = await _db
            .Domains
       .Include(domain => domain.BoardGamesDomains)
            //.Where(domain => !domain.IsDeleted)
            //.OrderBy(domain => domain.Name)
            .ToListAsync();

        return domainList;
    }
    public Task<List<Domain>> GetAll_Paged_Sorted_andFiltered(
        int pageIndex = 0
        , int pageSize = 20
        , string? sortColumn = null
        , string sortOrder = "ASC"
        , string? filterQuery = null)
    {

        IQueryable<Domain> query = _db.Domains.AsQueryable();

        if (filterQuery is not null)
        {
            if (filterQuery.CheckHasData())
            {
                query = query.Where(domain =>

      domain.DomainDesc != null
                                        && domain.DomainDesc.Contains(filterQuery));

            }
        }

        var sortKeySelector = GetOrderByKeySelector(sortColumn);

        //* The order to configure IQueryable is important
        //* If OrderBy is configured last, it will not work as expected

        if (sortKeySelector is not null)
        {
            if (sortOrder == "ASC")
                query = query.OrderBy(sortKeySelector);
            else
                query = query.OrderByDescending(sortKeySelector);
        }

        //* Configure Skip and Take

        query = query
                //.OrderBy(keySelector)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
       .Include(domain => domain.BoardGamesDomains)
        ;

        return query.ToListAsync();
    }
    public async Task<int> GetAllRecordCount()
    {
        return await _db.Domains.CountAsync();
    }

    public async Task<List<Domain>> Search(string dataToSearch)
    {
        //List<Domain> domainList = await _db.Domains.FromSql(
        //    $"select * from vwDomains where {dataToSearch}")
        //    .ToListAsync();

        List<Domain> domainList = await this.Search(domain =>
              (domain.DomainDesc != null && domain.DomainDesc.Contains(dataToSearch))
        );

        return domainList;
    }
    public async Task<List<Domain>> Search(Expression<Func<Domain, bool>> wherePredicate)
    {
        List<Domain> domainList = await _db
            .Domains
           .Include(domain => domain.BoardGamesDomains)
            .Where(wherePredicate)
            .ToListAsync();

        return domainList;
    }
    public async Task<Domain> Insert(Domain model)
    {
        await _db.Domains.AddAsync(model);
        await _db.SaveChangesAsync();

        if (model.DomainID <= 0)
            throw new Exception("Could not create the domain as expected");

        this.Inserted?.Invoke();

        return model;
    }
    public async Task<int> Update(Domain model)
    {
        var dbDomain = await _db
            .Domains
                //.Include(x => x.ItemsGenres)
                //.Include(x => x.ItemsPlayers)
                .FirstOrDefaultAsync(x => x.DomainID == model.DomainID)
                ?? throw new Exception("Item not found");

        dbDomain.DomainDesc = model.DomainDesc;
        //if (model.ItemsGenres != null)
        //{
        //    dbItem.ItemsGenres = model.ItemsGenres;
        //}
        //if (model.ItemsPlayers != null)
        //{
        //    dbItem.ItemsPlayers = model.ItemsPlayers;
        //}
        await _db.SaveChangesAsync();

        return dbDomain.DomainID;

        ////* Another way
        //DbSet<Domain> dbSet = _db.Set<Domain>();
        //dbSet.Attach(model);
        //_db.Entry(model).State = EntityState.Modified;
        //await _db.SaveChangesAsync();
        //return model.DomainID;
    }
    public async Task<bool> Delete(int pkID)
    {
        //* We get a 'Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException'
        //var domain = new Domain() { DomainID = pkID };
        //_db.Domains.Attach(domain);
        //_db.Remove(domain);
        //int removedID = await _db.SaveChangesAsync();
        //return removedID > 0;

        Domain? storedDomain = await this.GetByID(pkID);

        if (storedDomain == null)
            return false;
        {
            _db.Domains.Remove(storedDomain);
            await _db.SaveChangesAsync();

            this.Deleted?.Invoke();

            return true;
        };
    }
}
