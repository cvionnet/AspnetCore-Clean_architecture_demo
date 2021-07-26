using Application.Features._ViewModels;
using Application.Responses;
using System.Collections.Generic;

namespace Application.Features.Companies
{
    public class GetCompanyByIdQueryResponse : BaseResponse
    {
        public GetCompanyByIdQueryResponse() : base() { }

        public CompanyVM Company { get; set; }
    }
}
