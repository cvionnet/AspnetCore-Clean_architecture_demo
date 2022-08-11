using Application.Responses;

namespace Application.Features.Companies;

public class GetCompaniesQueryResponse : BaseResponse
{
    public GetCompaniesQueryResponse() : base()
    { }

    public IReadOnlyList<CompanyVM> Company { get; set; }
}
