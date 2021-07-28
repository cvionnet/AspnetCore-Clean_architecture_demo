using Application.Features._DTOs;
using MediatR;

namespace Application.Features.Companies
{
    public class UpdateCompanyCommand : IRequest<UpdateCompanyCommandResponse>
    {
        public UpdateCompanyDTO UpdateCompany { get; set; }
    }
}
