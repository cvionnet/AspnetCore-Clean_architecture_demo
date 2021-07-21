using Application.Features._ViewModels;
using Application.Responses;
using System.Collections.Generic;

namespace Application.Features.Companies
{
    public class GetCompaniesQueryResponse : BaseResponse
    {
        public GetCompaniesQueryResponse() : base() { }

        public IReadOnlyList<CompanyVM> Company { get; set; }
    }
}
