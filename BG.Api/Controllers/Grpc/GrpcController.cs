using BG.Api.gRPC;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static BG.Api.gRPC.Grpc;

namespace BG.Api.Controllers.Grpc;

//[Route("api/[controller]")]
[Route("[controller]/[action]")]
[ApiController]
public class GrpcController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<BoardGameResponse> GetBoardGame(int id)
    {
        using GrpcChannel? channel = GrpcChannel.ForAddress("https://localhost:40443");
        GrpcClient? client = new gRPC.Grpc.GrpcClient(channel);

        BoardGameResponse? response = await client.GetBoardGameAsync(
                                            new BoardGameRequest { Id = id });

        return response;
    }

    [HttpPost]
    public async Task<BoardGameResponse> UpdateBoardGame(
        string token,
        int id,
        string name)
    {
        var headers = new Metadata();
        headers.Add("Authorization", $"Bearer {token}");

        using GrpcChannel? channel = GrpcChannel.ForAddress("https://localhost:40443");
        GrpcClient? client = new gRPC.Grpc.GrpcClient(channel);

        BoardGameResponse? response = await client.UpdateBoardGameAsync(
                            new UpdateBoardGameRequest
                            {
                                Id = id,
                                Name = name
                            },
                            headers);
        return response;
    }
}
