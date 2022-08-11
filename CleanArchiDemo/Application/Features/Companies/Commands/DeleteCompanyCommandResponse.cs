using Application.Responses;

namespace Application.Features.Companies;

public class DeleteCompanyCommandResponse : BaseResponse
{
    public DeleteCompanyCommandResponse() : base()
    { }

    public int DeletedLinesCount { get; set; }
}
