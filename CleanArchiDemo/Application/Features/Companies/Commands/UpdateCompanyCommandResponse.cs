using Application.Features._ViewModels;
using Application.Responses;

namespace Application.Features.Companies
{
    public class UpdateCompanyCommandResponse : BaseResponse
    {
        public UpdateCompanyCommandResponse() : base()
        { }

        public CompanyVM UpdateCompany { get; set; }
    }
}
