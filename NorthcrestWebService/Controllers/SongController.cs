using Microsoft.AspNetCore.Mvc;
using NorthcrestWebService.App_BusinessLogic.Interfaces;
using NorthcrestWebService.Models;
using NorthcrestWebService.Models.Interfaces;
using RefreshNorthcrestDataFromPlanningCenter.Domain;
using System.Drawing.Text;
using System.IO.Compression;

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
        public async Task<IActionResult> GetPlanWithSongs(int planId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IClientPlan_ForSongs clientPlanWithSongs =
                        await _songProcessor.GetPlanWithSongsAsync(planId);

                    return Ok(clientPlanWithSongs);
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
        public async Task<IActionResult> GetPlansWithoutSongs()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IList<IClientPlan_ForSongs> clientPlan_ForSongsList =
                        await _songProcessor.GetPlansWithoutSongsAsync();

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
        public async Task<IActionResult> GetPlansForSongs()
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IList<IClientPlan_ForSongs> clientPlan_ForSongsList =
                        await _songProcessor.GetPlansWithoutSongsAsync();
                    
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

        [HttpGet, RequireHttps]
        public async Task<IActionResult> GetMp3([FromQuery] string filePath)
        {
            try
            {
                byte[] song = await _songProcessor.GetMp3Async(filePath);
                Stream stream = new MemoryStream(song);
                return new FileStreamResult(stream, "audio/mpeg");
            }
            catch (Exception ex)
            {
                // TODO Add error logging.
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet, RequireHttps]
        public async Task<IActionResult> GetSongsZipFile([FromHeader] string[] songFileNameList)
        {
            string songDirectory = @"C:\ExternalDatabaseFiles\SongFiles\";
            try
            {
                using (var outStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var file in songFileNameList)
                        {
                            var fileInArchive = archive.CreateEntry(file, CompressionLevel.Optimal);
                            using (var entryStream = fileInArchive.Open())
                            {
                                using (var fileCompressionStream =
                                    new MemoryStream(System.IO.File.ReadAllBytes(songDirectory + file)))
                                {
                                    await fileCompressionStream.CopyToAsync(entryStream);
                                }
                            }
                        }
                    }

                    outStream.Position = 0;

                    return File(outStream.ToArray(), "application/zip", "songFiles.zip");
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
