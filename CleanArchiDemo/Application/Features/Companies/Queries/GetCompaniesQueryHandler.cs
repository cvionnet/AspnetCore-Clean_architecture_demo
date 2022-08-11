using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Threading;
using SerilogTimings;
using MediatR;
using Application.Contracts.Persistence;

namespace Application.Features.Companies;

public class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, GetCompaniesQueryResponse>
{
    private readonly IMapper _mapper;
    private readonly ILogger<GetCompaniesQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetCompaniesQueryHandler(IMapper mapper, ILogger<GetCompaniesQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetCompaniesQueryResponse> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        var response = new GetCompaniesQueryResponse();

        try
        {
            using (Operation.Time($"[{GetType().Name}][TIMING]Get All Companies"))
            {
                // Get the list of all companies (from Infrastructure layer)
                var allCompanies = await _unitOfWork.Companies.ListAllAsync();

                // Create a standard response (send back by the Controller in Api layer)
                if (allCompanies is null)
                {
                    //throw new NotFoundException(nameof(Companies), null);
                    response.Success = Responses.BaseResponse.StatusCode.NotFound;
                    response.Message = "No company found";
                    _logger.LogInformation($"{response.Message}");
                }
                else
                {
                    response.Success = Responses.BaseResponse.StatusCode.Ok;
                    response.Company = _mapper.Map<IReadOnlyList<CompanyVM>>(allCompanies);
                    _logger.LogInformation($"Return list of all companies", response);
                }
            }

            return response;
        }
        catch (Exception ex)
        {
            //throw new BadRequestException("Can't execute query on companies");
            response.Success = Responses.BaseResponse.StatusCode.BadRequest;
            response.Message = "Can't execute query";
            _logger.LogError(ex, $"{response.Message} on companies");

            return response;
        }
    }
}
