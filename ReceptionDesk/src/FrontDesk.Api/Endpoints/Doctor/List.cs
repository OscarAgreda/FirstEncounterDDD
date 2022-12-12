using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using AutoMapper;
using BlazorShared.Models.Doctor;
using FrontDesk.Core.SyncedAggregates;
using Microsoft.AspNetCore.Mvc;
using FirstEncounterDDD.SharedKernel.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace FrontDesk.Api.DoctorEndpoints
{
        //when we define an endpoint we simply inherit from base async endpoint and specify the request type if any and the response type if any
        public class List : EndpointBaseAsync
          .WithRequest<ListDoctorRequest>
          .WithActionResult<ListDoctorResponse>
        {
                private readonly IReadRepository<Doctor> _repository;
                private readonly IMapper _mapper;

                //we can also do dependency injection through the Constructor just as you would with a controller
                public List(IReadRepository<Doctor> repository,
                  IMapper mapper)
                {
                        _repository = repository;
                        _mapper = mapper;
                }

                [HttpGet(ListDoctorRequest.Route)]
                [SwaggerOperation(
                    Summary = "List Doctors",
                    Description = "List Doctors",
                    OperationId = "doctors.List",
                    Tags = new[] { "DoctorEndpoints" })
                ]

                // <summary>
                //each endpoint has a single handle or handle async method and this is where
                //the actual work of the endpoint is done you can see in this example that we are
                // simply awaiting on the repositories list async method in order to get our list of
                // doctors once we have the list we map it to our
                // dto that we're going to actually return and pass that back as part of that
                // response type the response as we could see in Swagger,
                // includes the doctors as Json as well as a count property that includes the total
                // number of those doctors
                // </summary>
                public override async Task<ActionResult<ListDoctorResponse>> HandleAsync([FromQuery] ListDoctorRequest request,
                  CancellationToken cancellationToken)
                {
                        var response = new ListDoctorResponse(request.CorrelationId());

                        var doctors = await _repository.ListAsync();
                        if (doctors is null) return NotFound();

                        response.Doctors = _mapper.Map<List<DoctorDto>>(doctors);
                        response.Count = response.Doctors.Count;

                        return Ok(response);
                }
        }
}
