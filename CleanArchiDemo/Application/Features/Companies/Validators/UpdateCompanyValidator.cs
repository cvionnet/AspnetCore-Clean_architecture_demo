using Application.Contracts.Persistence;
using Application.Features.Companies;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Validators
{
    public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyCommand>
    {
        private readonly ICompanyRepository _companyRepository;

        public UpdateCompanyValidator(ICompanyRepository companyRepository, string pActualCompanyName)
        {
            _companyRepository = companyRepository;

            RuleFor(p => p.UpdateCompany.CompanyID)
                .NotEmpty().WithMessage("'{PropertyName}' is required")
                .NotNull()
                .WithName("CompanyID");

            RuleFor(p => p.UpdateCompany.Name)
                .NotEmpty().WithMessage("'{PropertyName}' is required")
                .NotNull()
                .MinimumLength(2).WithMessage("'{PropertyName}' must have at least 2 characters")
                .MaximumLength(50).WithMessage("'{PropertyName}' must not exceed 50 characters")
                .WithName("Name");

            RuleFor(p => p.UpdateCompany.Email)
                .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible)
                .WithMessage("'{PropertyValue}' is not a valid email address")
                .WithName("Email")
                .When(p => p.UpdateCompany.Email != "" || p.UpdateCompany.Email is not null);

            RuleFor(p => p.UpdateCompany.BillingAddress)
                .MinimumLength(5).WithMessage("'{PropertyName}' must have at least 5 characters")
                .MaximumLength(150).WithMessage("'{PropertyName}' must not exceed 150 characters")
                .WithName("BillingAddress")
                .When(p => p.UpdateCompany.BillingAddress != "" || p.UpdateCompany.BillingAddress is not null);

            RuleFor(p => p.UpdateCompany.Postcode)
                .MinimumLength(2).WithMessage("'{PropertyName}' must have at least 2 characters")
                .MaximumLength(50).WithMessage("'{PropertyName}' must not exceed 50 characters")
                .WithName("Postcode")
                .When(p => p.UpdateCompany.Postcode != "" || p.UpdateCompany.Postcode is not null);

            RuleFor(p => p.UpdateCompany.City)
                .MinimumLength(2).WithMessage("'{PropertyName}' must have at least 2 characters")
                .MaximumLength(50).WithMessage("'{PropertyName}' must not exceed 50 characters")
                .WithName("City")
                .When(p => p.UpdateCompany.City != "" || p.UpdateCompany.City is not null);

            // Business rules
            // Name must be unique  (only check if the new name is different from the actual Name in DB)
            RuleFor(e => e)
                .MustAsync(CompanyNameUnique)
                .WithMessage($"A company with this name already exists")
                .When(p => p.UpdateCompany.Name != pActualCompanyName);

            // CountryID must exists
            RuleFor(e => e)
                .MustAsync(CountryIDExists)
                .WithMessage($"The country ID does not exists")
                .When(p => p.UpdateCompany.CountryID != 0);
        }

        /// <summary>
        /// Check if the name already exists
        /// </summary>
        /// <returns>True if the value is not present</returns>
        private async Task<bool> CompanyNameUnique(UpdateCompanyCommand arg1, CancellationToken arg2)
        {
            return await _companyRepository.isNameUnique(arg1.UpdateCompany.Name);
        }

        /// <summary>
        /// Check if the country ID exists
        /// </summary>
        /// <returns>True if the value is present</returns>
        private async Task<bool> CountryIDExists(UpdateCompanyCommand arg1, CancellationToken arg2)
        {
            return ! await _companyRepository.isCountryIdExists(arg1.UpdateCompany.CountryID);
        }
    }
}
