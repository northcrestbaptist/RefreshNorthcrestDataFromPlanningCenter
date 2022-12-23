
using Microsoft.AspNetCore.Mvc;
using NorthcrestWebService.App_BusinessLogic.Interfaces;
using NorthcrestWebService.Models.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DTAS_MMWebServices_Core.Controllers
{
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
        public async Task<IActionResult> GetFile([FromQuery]int fileId)
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
