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
    public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, GetCompanyByIdQueryResponse>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<GetCompanyByIdQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public GetCompanyByIdQueryHandler(IMapper mapper, ILogger<GetCompanyByIdQueryHandler> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<GetCompanyByIdQueryResponse> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new GetCompanyByIdQueryResponse();

            try
            {
                using (Operation.Time($"[{GetType().Name}][TIMING]Get Company {request.CompanyId}"))
                {
                    // Get a specific company (from Infrastructure layer)
                    var company = await _unitOfWork.Companies.GetByIdAsync(request.CompanyId);

                    // Create a standard response (send back by the Controller in Api layer)
                    if (company is null)
                    {
                        //throw new NotFoundException(nameof(Companies), request.CompanyId);
                        response.Success = Responses.BaseResponse.StatusCode.NotFound;
                        response.Message = "No company found";
                        _logger.LogInformation($"{response.Message} (company id:{request.CompanyId})");
                    }
                    else
                    {
                        response.Success = Responses.BaseResponse.StatusCode.Ok;
                        response.Company = _mapper.Map<CompanyVM>(company);
                        _logger.LogInformation($"Return company {request.CompanyId}", response);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                //throw new BadRequestException($"Can't execute query on company {request.CompanyId}");
                response.Success = Responses.BaseResponse.StatusCode.BadRequest;
                response.Message = $"Can't execute query";
                _logger.LogError(ex, $"{response.Message} (company id:{request.CompanyId}");

                return response;
            }
        }
    }
}
