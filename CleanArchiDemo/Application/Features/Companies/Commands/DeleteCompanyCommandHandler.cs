using Application.Contracts.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SerilogTimings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Companies
{
    public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, DeleteCompanyCommandResponse>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<DeleteCompanyCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCompanyCommandHandler(IMapper mapper, ILogger<DeleteCompanyCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<DeleteCompanyCommandResponse> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            var response = new DeleteCompanyCommandResponse();

            try
            {
                using (Operation.Time($"[{GetType().Name}][TIMING]Delete Company {request.CompanyId}"))
                {
                    // Check if the element exists
                    var toDelete = await _unitOfWork.Companies.GetByIdAsync(request.CompanyId);

                    // Create a standard response (send back by the Controller in Api layer)
                    if (toDelete is null)
                    {
                        //throw new NotFoundException(nameof(Companies), request.CompanyId);
                        response.Success = Responses.BaseResponse.StatusCode.NotFound;
                        response.Message = "No company found";
                        _logger.LogInformation($"{response.Message}");
                    }
                    else
                    {
                        // Delete
                        int affectedRows = await _unitOfWork.Companies.DeleteAsync(toDelete);

                        response.Success = Responses.BaseResponse.StatusCode.Ok;
                        response.DeletedLinesCount = affectedRows;
                        _logger.LogInformation($"Delete {affectedRows} lines for company {request.CompanyId}", response);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                //throw new BadRequestException($"Can't delete company {request.CompanyId}");
                response.Success = Responses.BaseResponse.StatusCode.BadRequest;
                response.Message = $"Can't delete company {request.CompanyId}";
                _logger.LogError(ex, $"{response.Message}");

                return response;
            }
        }
    }
}
