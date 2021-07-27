using Application.Features._ViewModels;
using Application.Responses;

namespace Application.Features.Companies
{
    public class AddCompanyCommandResponse : BaseResponse
    {
        public AddCompanyCommandResponse() : base()
        { }

        public CompanyVM NewCompany { get; set; }
    }
}
