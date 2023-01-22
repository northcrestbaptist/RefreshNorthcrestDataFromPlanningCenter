using Microsoft.AspNetCore.Mvc;
using NorthcrestWebService.App_BusinessLogic.Interfaces;
using NorthcrestWebService.Models.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Domain;

namespace NorthcrestWebService.Controllers
{
    [ApiController, RequireHttps]
    [Route("[controller]/[action]")]
    public class SongController : ControllerBase
    {
        private readonly ISongProcessor _songProcessor;
        private readonly ILogger<SongController> _logger;

        public SongController(
            ISongProcessor songProcessor,
            ILogger<SongController> logger)
        {
            _songProcessor = songProcessor;
            _logger = logger;
        }

        [HttpGet, RequireHttps]
        public async Task<IActionResult> GetGeneralSongs()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IList<IClientGeneralSong> clientGeneralSongList =
                        await _songProcessor.GetGeneralSongsAsync();

                    return Ok(clientGeneralSongList);
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
        public async Task<IActionResult> GetPlansForSongs()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IList<IClientPlan_ForSongs> clientPlan_ForSongsList =
                        await _songProcessor.GetPlansWithSongsAsync();
                    //_logger.LogInformation("Plans For Songs Resulsts:");
                    //foreach(IClientPlan_ForSongs plan in clientPlan_ForSongsList)
                    //{
                    //    _logger.LogInformation("Plan date: {date}", plan.PlanDateTime.ToString());
                    //    _logger.LogInformation("Number of songs in plan: {number}", plan.PlanSongs.Count.ToString());
                    //    foreach(PlanSong song in plan.PlanSongs)
                    //    {
                    //        _logger.LogInformation("Song Name: {name}", song.SongName);
                    //    }
                    //}
                    
                    return Ok(clientPlan_ForSongsList);
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
        public async Task<IActionResult> GetPdf([FromQuery] int fileId)
        {
            try
            {
                byte[] file = await _songProcessor.GetPdfAsync(fileId);
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
