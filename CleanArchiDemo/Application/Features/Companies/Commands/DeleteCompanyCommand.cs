using MediatR;

namespace Application.Features.Companies
{
    public class DeleteCompanyCommand : IRequest<DeleteCompanyCommandResponse>
    {
        public int CompanyId { get; set; }
    }
}
