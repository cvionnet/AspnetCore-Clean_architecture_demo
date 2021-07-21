using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using SerilogTimings;
using Application.Features._ViewModels;
using MediatR;
using Application.Contracts.Persistence;
using Application.Exceptions;
using System.Collections.Generic;

namespace Application.Features.Companies
{
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
                using (Operation.Time($"[{GetType().Name}] Get All Companies - Time operation", GetType().Name))
                {
                    // Get the list of all companies (from Infrastructure layer)
                    var allCompanies = await _unitOfWork.Companies.ListAllAsync();

                    // Create a standard response (send back by the Controller in Api layer)
                    if (allCompanies is null)
                    {
                        response.Success = Responses.BaseResponse.StatusCode.NotFound;
                        response.Message = "No company found";
                        _logger.LogInformation($"{System.Reflection.MethodBase.GetCurrentMethod().Name} - {response.Message}");
                    }
                    else
                    {
                        response.Success = Responses.BaseResponse.StatusCode.Ok;
                        response.Company = _mapper.Map<IReadOnlyList<CompanyVM>>(allCompanies);
                    }
                }

                _logger.LogInformation($"{GetType().Name} - Return list of all companies", response);

                return response;
            }
            catch (Exception ex)
            {
                response.Success = Responses.BaseResponse.StatusCode.BadRequest;
                response.Message = "Can't execute query on companies";
                _logger.LogError(ex, $"{System.Reflection.MethodBase.GetCurrentMethod().Name} - {response.Message}");

                return response;
            }
        }
    }
}
