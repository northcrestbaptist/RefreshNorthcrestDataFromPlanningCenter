using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using NorthcrestWebService.App_BusinessLogic.Interfaces;
using NorthcrestWebService.Models.Interfaces;
using System.IO.Compression;

namespace NorthcrestWebService.Controllers
{
    [ApiController, RequireHttps, Authorize]
    [Route("[controller]/[action]")]
    public class SecureSongController : ControllerBase
    {
        private readonly ISongProcessor _songProcessor;
        private readonly ILogger<SongController> _logger;
        // The web API will only accept tokens 1) for users, and 2) having the "access_as_user" scope for this API
        //static readonly string[] scopeRequiredByApi = new string[] { "access_as_user" };

        public SecureSongController(
            ISongProcessor songProcessor,
            ILogger<SongController> logger)
        {
            _songProcessor = songProcessor;
            _logger = logger;
        }

        [HttpGet, RequireHttps]
        [RequiredScopeOrAppPermission(
            RequiredScopesConfigurationKey = "AzureAD:Scopes:Read"
        )]
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
        [RequiredScopeOrAppPermission(
            RequiredScopesConfigurationKey = "AzureAD:Scopes:Read"
        )]
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
        [RequiredScopeOrAppPermission(
            RequiredScopesConfigurationKey = "AzureAD:Scopes:Read"
        )]
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
        [RequiredScopeOrAppPermission(
            RequiredScopesConfigurationKey = "AzureAD:Scopes:Read"
        )]
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
        [RequiredScopeOrAppPermission(
            RequiredScopesConfigurationKey = "AzureAD:Scopes:Read"
        )]
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
        [RequiredScopeOrAppPermission(
            RequiredScopesConfigurationKey = "AzureAD:Scopes:Read"
        )]
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
        [RequiredScopeOrAppPermission(
            RequiredScopesConfigurationKey = "AzureAD:Scopes:Read"
        )]
        public async Task<IActionResult> GetSongsZipFile([FromHeader] string[] songPathList)
        {
            string songDirectory = @"C:\ExternalDatabaseFiles\SongFiles\";
            try
            {
                using (var outStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var file in songPathList)
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
                _logger.LogError("AN ERROR OCCURRED: {error}", ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }
    }
}
