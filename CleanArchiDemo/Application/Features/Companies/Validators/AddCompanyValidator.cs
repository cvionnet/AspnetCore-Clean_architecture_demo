using Application.Contracts.Persistence;
using Application.Features.Companies;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Validators
{
    public class AddCompanyValidator : AbstractValidator<AddCompanyCommand>
    {
        private readonly ICompanyRepository _companyRepository;

        public AddCompanyValidator(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;

            RuleFor(p => p.NewCompany.Name)
                .NotEmpty().WithMessage("{PropertyName} is required")
                .NotNull()
                .MinimumLength(2).WithMessage("{PropertyName} must have at least 2 characters")
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters");

            RuleFor(p => p.NewCompany.Email)
                .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible)
                .When(p => p.NewCompany.Email != "" || p.NewCompany.Email is not null);

            RuleFor(p => p.NewCompany.BillingAddress)
                .MinimumLength(5).WithMessage("{PropertyName} must have at least 5 characters")
                .MaximumLength(150).WithMessage("{PropertyName} must not exceed 150 characters")
                .When(p => p.NewCompany.BillingAddress != "" || p.NewCompany.BillingAddress is not null);

            RuleFor(p => p.NewCompany.Postcode)
                .MinimumLength(2).WithMessage("{PropertyName} must have at least 2 characters")
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters")
                .When(p => p.NewCompany.Postcode != "" || p.NewCompany.Postcode is not null);

            RuleFor(p => p.NewCompany.City)
                .MinimumLength(2).WithMessage("{PropertyName} must have at least 2 characters")
                .MaximumLength(50).WithMessage("{PropertyName} must not exceed 50 characters")
                .When(p => p.NewCompany.City != "" || p.NewCompany.City is not null);

            // Business rules
            RuleFor(e => e)
                .MustAsync(CompanyNameUnique)
                .WithMessage("A company with this name already exists");
        }

        private async Task<bool> CompanyNameUnique(AddCompanyCommand arg1, CancellationToken arg2)
        {
            return await _companyRepository.isNameUnique(arg1.NewCompany.Name);
        }
    }
}
