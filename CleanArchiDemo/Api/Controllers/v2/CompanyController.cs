using Application.Features.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Api.Controllers.v2;

[ApiVersion("2.0")]
[ApiController]
// [Route("api/v{version:apiVersion}/[controller]")]
/// <summary>
/// Just a test to have 2 versions of the API  (methods are the same, but v1 do not need to be auth)
/// </summary>
public class CompanyController : ApiController<CompanyController>
{
    [Authorize]
    [HttpGet("all", Name = "GetAllCompanies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetCompaniesQueryResponse>> GetAllCompanies()
    {
        try
        {
            var query = new GetCompaniesQuery();
            var response = await Mediator.Send(query);

            if (response.Success == Application.Responses.BaseResponse.StatusCode.BadRequest)
                return BadRequest(response.Message);
            if (response.Success == Application.Responses.BaseResponse.StatusCode.NotFound)
                return NotFound(response.Message);

            return Ok(response);
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, $"Something went wrong inside GetAllCompanies action: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [Authorize]
    [HttpGet("{id}", Name = "GetCompanyById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetCompaniesQueryResponse>> GetCompanyById(int id)
    {
        try
        {
            var query = new GetCompanyByIdQuery() { CompanyId = id };
            var response = await Mediator.Send(query);

            if (response.Success == Application.Responses.BaseResponse.StatusCode.BadRequest)
                return BadRequest(response.Message);
            if (response.Success == Application.Responses.BaseResponse.StatusCode.NotFound)
                return NotFound(response.Message);

            return Ok(response);
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, $"Something went wrong inside GetCompanyById action: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [Authorize]
    [HttpPost(Name = "AddCompany")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AddCompanyCommandResponse>> AddCompany([FromBody] AddCompanyCommand command)
    {
        try
        {
            var response = await Mediator.Send(command);

            if (response.Success == Application.Responses.BaseResponse.StatusCode.BadRequest)
                return BadRequest(response.Message);
            if (response.Success == Application.Responses.BaseResponse.StatusCode.NotFound)
                return NotFound(response.Message);

            return Ok(response);
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, $"Something went wrong inside DeleteCompany action: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }

    [Authorize]
    [HttpPut(Name = "UpdateCompany")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UpdateCompanyCommandResponse>> UpdateCompany([FromBody] UpdateCompanyCommand command)
    {
        try
        {
            var response = await Mediator.Send(command);

            if (response.Success == Application.Responses.BaseResponse.StatusCode.BadRequest)
                return BadRequest(response.Message);
            if (response.Success == Application.Responses.BaseResponse.StatusCode.NotFound)
                return NotFound(response.Message);

            return Ok(response);
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, $"Something went wrong inside UpdateCompany action: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }


    [Authorize]
    [HttpDelete("{id}", Name = "DeleteCompany")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeleteCompanyCommandResponse>> DeleteCompany(int id)
    {
        try
        {
            var command = new DeleteCompanyCommand() { CompanyId = id };
            var response = await Mediator.Send(command);

            if (response.Success == Application.Responses.BaseResponse.StatusCode.BadRequest)
                return BadRequest(response.Message);
            if (response.Success == Application.Responses.BaseResponse.StatusCode.NotFound)
                return NotFound(response.Message);

            return Ok(response);
        }
        catch (Exception ex)
        {
            Logger.LogCritical(ex, $"Something went wrong inside DeleteCompany action: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }
}
