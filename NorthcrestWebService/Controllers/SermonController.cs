
using Microsoft.AspNetCore.Mvc;
using NorthcrestWebService.App_BusinessLogic.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DTAS_MMWebServices_Core.Controllers
{
    //[Route("api/[controller]/[action]")]

    [ApiController, RequireHttps]
    [Route("[controller]/[action]")]
    public class SermonController : ControllerBase
    {
        private readonly ISermonProcessor _sermonProcessor;

        public SermonController(ISermonProcessor sermonProcessor)
        {
            _sermonProcessor = sermonProcessor;
        }

        [HttpGet, RequireHttps]
        public async Task<IActionResult> GetSermons()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IList<Sermon> sermonList =
                        await _sermonProcessor.GetSermonsAsync();

                    return Ok(sermonList);
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
    }


}
