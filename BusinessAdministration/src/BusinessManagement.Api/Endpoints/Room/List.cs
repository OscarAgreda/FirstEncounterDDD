using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using BlazorShared.Models.Room;
using BusinessManagement.Core.Aggregates;
using BusinessManagement.Core.Interfaces;
using BusinessManagement.Core.Specifications;
using Microsoft.AspNetCore.Mvc;
using FirstEncounterDDD.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace BusinessManagement.Api.RoomEndpoints
{
  public class List : EndpointBaseAsync
    .WithRequest<ListRoomRequest>
    .WithActionResult<ListRoomResponse>
  {
    private readonly IRepository<Room> _repository;
    private readonly IMapper _mapper;

    public List(IRepository<Room> repository,
      IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [HttpGet("api/rooms")]
    [SwaggerOperation(
        Summary = "List Rooms",
        Description = "List Rooms",
        OperationId = "rooms.List",
        Tags = new[] { "RoomEndpoints" })
    ]
    public override async Task<ActionResult<ListRoomResponse>> HandleAsync([FromQuery] ListRoomRequest request, CancellationToken cancellationToken)
    {
      var response = new ListRoomResponse(request.CorrelationId);

      var roomSpec = new RoomSpecification();
      var rooms = await _repository.ListAsync(roomSpec);
      if (rooms is null) return NotFound();

      response.Rooms = _mapper.Map<List<RoomDto>>(rooms);
      response.Count = response.Rooms.Count;

      return Ok(response);
    }
  }
}
