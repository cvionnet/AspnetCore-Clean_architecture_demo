using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features._ViewModels;
using Application.Features.Validators;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using SerilogTimings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Companies
{
    public class AddCompanyCommandHandler : IRequestHandler<AddCompanyCommand, AddCompanyCommandResponse>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<AddCompanyCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public AddCompanyCommandHandler(IMapper mapper, ILogger<AddCompanyCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<AddCompanyCommandResponse> Handle(AddCompanyCommand request, CancellationToken cancellationToken)
        {
            var response = new AddCompanyCommandResponse();

            try
            {
                using (Operation.Time($"[{GetType().Name}] Timing - Add new Company (name:{request.NewCompany.Name})"))
                {
                    // Call the Validator + Throw a Custom Exceptions if there is any error
                    var validator = new AddCompanyValidator(_unitOfWork.Companies);
                    var validationResult = await validator.ValidateAsync(request);
                    if (validationResult.Errors.Count > 0)
                    {
                        //throw new ValidationException(validationResult);
                        response.Success = Responses.BaseResponse.StatusCode.BadRequest;
var temp = String.Join(" || ", (new ValidationException(validationResult)).ValidationErrors);
                        response.Message = $"Validation errors - {String.Join(" || ", (new ValidationException(validationResult)).ValidationErrors)}";
                        _logger.LogInformation($"{response.Message}");
                    }
                    else
                    {
                        // Map the entity (AddCompanyDTO) from the controller to a Company (entity used to insert in DB)
                        var entity = _mapper.Map<Company>(request.NewCompany);
                        // Special case : AddCompanyDTO only have the ID of the country. Must create a Country object with this ID to insert in Company
                        entity.Country = new Country() { CountryID = request.NewCompany.CountryID };

                        // Insert the entity in DB
                        var newEntity = await _unitOfWork.Companies.AddAsync(entity);

                        response.Success = Responses.BaseResponse.StatusCode.Ok;
                        response.NewCompany = _mapper.Map<CompanyVM>(newEntity);

                        _logger.LogInformation($"Add new company (id:{newEntity.CompanyID}, name:{newEntity.Name})", response);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                //throw new BadRequestException($"Can't delete company {request.CompanyId}");
                response.Success = Responses.BaseResponse.StatusCode.BadRequest;
                response.Message = $"Can't add the new company (name:{request.NewCompany.Name})";
                _logger.LogError(ex, $"{response.Message}");

                return response;
            }
        }
    }
}
