using BG.Model.Core;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
//using HotChocolate.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using PrjBase.SecurityBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BG.Repo.EF.Core.GraphQL;
public class Mutation
{
    [Serial]
    [Authorize(Roles = new[] { RoleName.MODERATOR })]
    public async Task<BoardGame?> UpdateBoardGame(
            [Service] BGDbContext context, BoardGame_ChangeDTO argModel)
    {
        BoardGame? storedBoardgame = await context.BoardGames
            .Where(boardGame => boardGame.BoardGameID == argModel.BoardGameID)
            .FirstOrDefaultAsync();

        if (storedBoardgame != null)
        {
            if (!string.IsNullOrEmpty(argModel.Name))
                storedBoardgame.Name = argModel.Name;
            if (argModel.Year.HasValue && argModel.Year.Value > 0)
                storedBoardgame.Year = argModel.Year.Value;
            //storedBoardgame.LastModifiedDate = DateTime.Now;
            context.BoardGames.Update(storedBoardgame);
            await context.SaveChangesAsync();
        }
        return storedBoardgame;
    }

    [Serial]
    [Authorize(Roles = new[] { RoleName.ADMIN })]
    public async Task DeleteBoardGame(
        [Service] BGDbContext context, int id)
    {
        BoardGame? boardgame = await context.BoardGames
            .Where(boardGame => boardGame.BoardGameID == id)
            .FirstOrDefaultAsync();
        if (boardgame != null)
        {
            context.BoardGames.Remove(boardgame);
            await context.SaveChangesAsync();
        }
    }

    //[Serial]
    //[Authorize(Roles = new[] { RoleName.MODERATOR })]
    //public async Task<Domain?> UpdateDomain(
    //    [Service] BGDbContext context, DomainDTO model)
    //{
    //    var domain = await context.Domains
    //        .Where(d => d.Id == model.Id)
    //        .FirstOrDefaultAsync();
    //    if (domain != null)
    //    {
    //        if (!string.IsNullOrEmpty(model.Name))
    //            domain.Name = model.Name;
    //        domain.LastModifiedDate = DateTime.Now;
    //        context.Domains.Update(domain);
    //        await context.SaveChangesAsync();
    //    }
    //    return domain;
    //}

    //[Serial]
    //[Authorize(Roles = new[] { RoleName.ADMIN })]
    //public async Task DeleteDomain(
    //    [Service] BGDbContext context, int id)
    //{
    //    var domain = await context.Domains
    //        .Where(d => d.Id == id)
    //        .FirstOrDefaultAsync();
    //    if (domain != null)
    //    {
    //        context.Domains.Remove(domain);
    //        await context.SaveChangesAsync();
    //    }
    //}

    //[Serial]
    //[Authorize(Roles = new[] { RoleName.MODERATOR })]
    //public async Task<Mechanic?> UpdateMechanic(
    //    [Service] BGDbContext context, MechanicDTO model)
    //{
    //    var mechanic = await context.Mechanics
    //        .Where(m => m.Id == model.Id)
    //        .FirstOrDefaultAsync();
    //    if (mechanic != null)
    //    {
    //        if (!string.IsNullOrEmpty(model.Name))
    //            mechanic.Name = model.Name;
    //        mechanic.LastModifiedDate = DateTime.Now;

    //        context.Mechanics.Update(mechanic);
    //        await context.SaveChangesAsync();
    //    }
    //    return mechanic;
    //}

    //[Serial]
    //[Authorize(Roles = new[] { RoleName.ADMIN })]
    //public async Task DeleteMechanic(
    //    [Service] BGDbContext context, int id)
    //{
    //    var mechanic = await context.Mechanics
    //        .Where(m => m.Id == id)
    //        .FirstOrDefaultAsync();
    //    if (mechanic != null)
    //    {
    //        context.Mechanics.Remove(mechanic);
    //        await context.SaveChangesAsync();
    //    }
    //}
}
