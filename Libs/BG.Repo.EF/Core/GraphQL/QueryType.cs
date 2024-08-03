using BG.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG.Repo.EF.Core.GraphQL
{
    public class QueryType
    {
        [Serial]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<BoardGame> GetBoardGames(
            [Service] BGDbContext dbContext)
            => dbContext.BoardGames;

        [Serial]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Domain> GetDomains(
            [Service] BGDbContext dbContext)
            => dbContext.Domains;

        [Serial]
        [UsePaging]
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Mechanic> GetMechanics(
            [Service] BGDbContext dbContext)
            => dbContext.Mechanics;
    }
}
