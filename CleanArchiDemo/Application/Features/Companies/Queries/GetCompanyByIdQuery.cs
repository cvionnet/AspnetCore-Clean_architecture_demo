using MediatR;

namespace Application.Features.Companies
{
    public class GetCompanyByIdQuery : IRequest<GetCompanyByIdQueryResponse>
    {
        public int CompanyId { get; set; }
    }
}
