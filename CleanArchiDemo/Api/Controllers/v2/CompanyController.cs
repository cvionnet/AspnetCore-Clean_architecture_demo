using Application.Features.Companies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers.v2
{
    [ApiVersion("2.0")]
    [ApiController]
    // [Route("api/v{version:apiVersion}/[controller]")]
    /// <summary>
    /// Just a test to have 2 versions of the API  (methods are the same)
    /// </summary>
    public class CompanyController : ApiController<CompanyController>
    {
        [HttpGet("all", Name = "GetAllCompanies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetCompaniesQueryResponse>> GetAllCompanies()
        {
            try
            {
                GetCompaniesQuery query = new GetCompaniesQuery();
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
    }
}
