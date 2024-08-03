using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using PrjBase.SecurityBase;

namespace BG.Repo.EF.Core.gRPC;
//public class GrpcServices : Grpc.GrpcBase
//{
//    private readonly BGDbContext _context;

//    public GrpcService(BGDbContext context)
//    {
//        _context = context;
//    }

//    public override async Task<BoardGameResponse> GetBoardGame(
//        BoardGameRequest request,
//        ServerCallContext scc)
//    {
//        var bg = await _context.BoardGames
//            .Where(bg => bg.Id == request.Id)
//            .FirstOrDefaultAsync();
//        var response = new BoardGameResponse();
//        if (bg != null)
//        {
//            response.Id = bg.Id;
//            response.Name = bg.Name;
//            response.Year = bg.Year;
//        }
//        return response;
//    }

//    [Authorize(Roles = RoleName.MODERATOR)]
//    public override async Task<BoardGameResponse> UpdateBoardGame(
//        UpdateBoardGameRequest request,
//        ServerCallContext scc)
//    {
//        var bg = await _context.BoardGames
//            .Where(bg => bg.Id == request.Id)
//            .FirstOrDefaultAsync();
//        var response = new BoardGameResponse();
//        if (bg != null)
//        {
//            bg.Name = request.Name;
//            _context.BoardGames.Update(bg);
//            await _context.SaveChangesAsync();
//            response.Id = bg.Id;
//            response.Name = bg.Name;
//            response.Year = bg.Year;
//        }
//        return response;
//    }
//}