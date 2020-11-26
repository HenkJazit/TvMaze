using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using TvMaze.Data;
using TvMaze.Data.Models;

namespace TvMaze.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class TvShowsController : ControllerBase
    {
        private readonly ITvShowRepository _tvShowRepository;

        public TvShowsController(ITvShowRepository tvShowRepository)
        {
            _tvShowRepository = tvShowRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TvShow>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<TvShow>> Get(int start, int rows)
        {
            if (start < 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid input",
                    Detail = $"input {start} for {nameof(start)} is invalid, it must be 0 or greater",
                    Instance = Request.Path,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (rows <= 0 || rows > 100)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid input",
                    Detail = $"input {rows} for {nameof(rows)} is invalid, it must be greater than zero and not greater than 100",
                    Instance = Request.Path,
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var tvShows = _tvShowRepository.GetTvShowsPaged(start, rows);

            if (!(tvShows?.Any() ?? false))
                return NotFound();

            return Ok(tvShows);
        }
    }
}
