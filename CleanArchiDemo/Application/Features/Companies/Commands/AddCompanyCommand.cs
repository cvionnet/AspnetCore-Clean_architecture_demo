using Application.Features._DTOs;
using MediatR;

namespace Application.Features.Companies;

public class AddCompanyCommand : IRequest<AddCompanyCommandResponse>
{
    public AddCompanyDTO NewCompany { get; set; }
}
