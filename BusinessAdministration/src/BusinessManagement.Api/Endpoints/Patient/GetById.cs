using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using BlazorShared.Models.Patient;
using BusinessManagement.Core.Aggregates;
using BusinessManagement.Core.Specifications;
using Microsoft.AspNetCore.Mvc;
using FirstEncounterDDD.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace BusinessManagement.Api.PatientEndpoints
{
  public class GetById : EndpointBaseAsync
    .WithRequest<GetByIdPatientRequest>
    .WithActionResult<GetByIdPatientResponse>
  {
    private readonly IRepository<Client> _repository;
    private readonly IMapper _mapper;

    public GetById(IRepository<Client> repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [HttpGet("api/patients/{PatientId}")]
    [SwaggerOperation(
        Summary = "Get a Patient by Id with ClientId (via querystring)",
        Description = "Gets a Patient by Id",
        OperationId = "patients.GetById",
        Tags = new[] { "PatientEndpoints" })
    ]
    public override async Task<ActionResult<GetByIdPatientResponse>> HandleAsync([FromRoute] GetByIdPatientRequest request, 
      CancellationToken cancellationToken)
    {
      var response = new GetByIdPatientResponse(request.CorrelationId);

      var spec = new ClientByIdIncludePatientsSpec(request.ClientId);
      var client = await _repository.GetBySpecAsync(spec);
      if (client == null) return NotFound();

      var patient = client.Patients.FirstOrDefault(p => p.Id == request.PatientId);
      if (patient == null) return NotFound();

      response.Patient = _mapper.Map<PatientDto>(patient);

      return Ok(response);
    }
  }
}
