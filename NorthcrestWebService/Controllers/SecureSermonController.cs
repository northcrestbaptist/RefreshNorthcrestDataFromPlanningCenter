
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web.Resource;
using NorthcrestWebService.App_BusinessLogic.Interfaces;
using NorthcrestWebService.Models.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DTAS_MMWebServices_Core.Controllers
{
    [ApiController, RequireHttps, Authorize]
    [Route("[controller]/[action]")]
    public class SecureSermonController : ControllerBase
    {
        // The web API will only accept tokens 1) for users, and 2) having the "access_as_user" scope for this API
        //static readonly string[] scopeRequiredByApi = new string[] { "access_as_user" };

        private readonly ISermonProcessor _sermonProcessor;
        private readonly ILogger<SecureSermonController> _log;
        public SecureSermonController(ISermonProcessor sermonProcessor, ILogger<SecureSermonController> log)
        {
            _sermonProcessor = sermonProcessor;
            _log = log; 
        }

        [HttpGet, RequireHttps]//, Authorize(Roles = "deacon,mediaTeam")]
        [RequiredScopeOrAppPermission(
            RequiredScopesConfigurationKey = "AzureAD:Scopes:Read"
        )]
        public async Task<IActionResult> GetSermons()
        {
            //_log.LogInformation("User Claims:{claims}", User.Claims);
            //_log.LogInformation("User Claims:{claims}", User.Claims.Where(c => c.Type == ClaimTypes.Role).ToList());
            //_log.LogInformation("mediaTeam? {answer}", User.IsInRole("mediaTeam"));
            //_log.LogInformation("deacon? {answer}", User.IsInRole("deacon"));
            //_log.LogInformation("staff? {answer}", User.IsInRole("staff"));

            try
            {
                if (ModelState.IsValid)
                {
                    IList<IClientSermon> clientSermonList =
                        await _sermonProcessor.GetSermonsAsync();

                    return Ok(clientSermonList);
                }
                else
                {
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                // TODO Add error logging.
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet, RequireHttps]
        [RequiredScopeOrAppPermission(
            RequiredScopesConfigurationKey = "AzureAD:Scopes:Read"       )]
        public async Task<IActionResult> GetFile([FromQuery] int fileId)
        {
            try
            {
                byte[] file = await _sermonProcessor.GetFileAsync(fileId);
                Stream stream = new MemoryStream(file);
                return new FileStreamResult(stream, "application/pdf");
            }
            catch (Exception ex)
            {
                // TODO Add error logging.
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
