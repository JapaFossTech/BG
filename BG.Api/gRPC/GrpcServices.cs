using BG.Repo.EF;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PrjBase.SecurityBase;

namespace BG.Api.gRPC;
public class GrpcServices: Grpc.GrpcBase
{
    private readonly BGDbContext _context;

    public GrpcServices(BGDbContext context)
    {
        _context = context;
    }

    public override async Task<BoardGameResponse> GetBoardGame(
        BoardGameRequest request,
        ServerCallContext scc)
    {
        var bg = await _context.BoardGames
            .Where(bg => bg.BoardGameID == request.Id)
            .FirstOrDefaultAsync();
        var response = new BoardGameResponse();
        if (bg != null)
        {
            response.Id = bg.BoardGameID;
            response.Name = bg.Name;
            response.Year = bg.Year;
        }
        return response;
    }

    [Authorize(Roles = RoleName.MODERATOR)]
    public override async Task<BoardGameResponse> UpdateBoardGame(
        UpdateBoardGameRequest request,
        ServerCallContext scc)
    {
        var bg = await _context.BoardGames
            .Where(bg => bg.BoardGameID == request.Id)
            .FirstOrDefaultAsync();
        var response = new BoardGameResponse();
        if (bg != null)
        {
            bg.Name = request.Name;
            _context.BoardGames.Update(bg);
            await _context.SaveChangesAsync();
            response.Id = bg.BoardGameID;
            response.Name = bg.Name;
            response.Year = bg.Year;
        }
        return response;
    }
}