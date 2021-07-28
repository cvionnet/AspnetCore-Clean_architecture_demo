using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features._ViewModels;
using Application.Features.Validators;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SerilogTimings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Companies
{
    public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, UpdateCompanyCommandResponse>
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateCompanyCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCompanyCommandHandler(IMapper mapper, ILogger<UpdateCompanyCommandHandler> logger, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateCompanyCommandResponse> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            var response = new UpdateCompanyCommandResponse();

            try
            {
                using (Operation.Time($"[{GetType().Name}][TIMING]Update Company (name:{request.UpdateCompany.Name})"))
                {
                    // Check if the entity to update exists
                    var entityBeforeUpdate = await _unitOfWork.Companies.GetByIdAsync(request.UpdateCompany.CompanyID);
                    if (entityBeforeUpdate is null)
                    {
                        //throw new NotFoundException(nameof(Companies), request.CompanyId);
                        response.Success = Responses.BaseResponse.StatusCode.NotFound;
                        response.Message = "No company found";
                        _logger.LogInformation($"{response.Message} (company id:{request.UpdateCompany.CompanyID})");
                    }
                    else
                    {
                        // Call the Validator
                        var validator = new UpdateCompanyValidator(_unitOfWork.Companies, entityBeforeUpdate.Name);
                        var validationResult = await validator.ValidateAsync(request);
                        if (validationResult.Errors.Count > 0)
                        {
                            //throw new ValidationException(validationResult);
                            response.Success = Responses.BaseResponse.StatusCode.BadRequest;
                            response.Message = $"Validation errors ({validationResult.Errors.Count}):\n{String.Join("\n", (new ValidationException(validationResult)).ValidationErrors)}";
                            _logger.LogInformation($"{response.Message}");
                        }
                        else
                        {
                            // Map the entity (UpdateCompanyDTO) from the controller to a Company (entity used to update in DB)
                            var entity = _mapper.Map<Company>(request.UpdateCompany);

                            // Complete the entity
                            // CountryID : UpdateCompanyDTO only have the ID of the country. Must create a Country object with this ID to update in Company
                            entity.Country = new Country() { CountryID = request.UpdateCompany.CountryID };
                            // If values are null, take values from the entity in DB
                            if (entity.Name is null) entity.Name = entityBeforeUpdate.Name;
                            if (entity.Email is null) entity.Email = entityBeforeUpdate.Email;
                            if (entity.BillingAddress is null) entity.BillingAddress = entityBeforeUpdate.BillingAddress;
                            if (entity.Postcode is null) entity.Postcode = entityBeforeUpdate.Postcode;
                            if (entity.City is null) entity.City = entityBeforeUpdate.City;

                            // Update the entity in DB + traceability
                            _logger.LogInformation($"Update company - Values before ({JsonConvert.SerializeObject(entityBeforeUpdate)})", response);
                            var updateEntity = await _unitOfWork.Companies.UpdateAsync(entity);

                            response.Success = Responses.BaseResponse.StatusCode.Ok;
                            response.UpdateCompany = _mapper.Map<CompanyVM>(updateEntity);

                            _logger.LogInformation($"Update company (id:{updateEntity.CompanyID}, name:{updateEntity.Name})", response);
                        }
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                //throw new BadRequestException($"Can't delete company {request.CompanyId}");
                response.Success = Responses.BaseResponse.StatusCode.BadRequest;
                response.Message = $"Can't update the company";
                _logger.LogError(ex, $"{response.Message} (id:{request.UpdateCompany.CompanyID}, name:{request.UpdateCompany.Name})");

                return response;
            }
        }
    }
}
