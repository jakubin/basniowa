using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Common.Cqrs;
using Common.Startup;
using Logic.Common;
using Logic.Services;
using Logic.Shows;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Infrastructure;
using Website.Models;

namespace Website.Api.Shows
{
    /// <summary>
    /// API controller for managing shows.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/[controller]")]
    [Produces(ContentTypes.ApplicationJson)]
    public class ShowsController : Controller
    {
        /// <summary>
        /// Gets or sets the shows reader.
        /// </summary>
        [InjectService]
        public IShowsReader ShowsReader { get; set; }

        /// <summary>
        /// Gets or sets the identifier service.
        /// </summary>
        [InjectService]
        public IUniqueIdService IdService { get; set; }

        /// <summary>
        /// Gets or sets the command publisher.
        /// </summary>
        [InjectService]
        public ICommandPublisher CommandPublisher { get; set; }

        /// <summary>
        /// Gets all shows.
        /// </summary>
        /// <returns>List of all shows with details.</returns>
        /// <response code="200">Returns all shows main information.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ShowHeaderModel[]), (int)HttpStatusCode.OK)]
        public IActionResult GetAll()
        {
            var details = ShowsReader.GetAllShows();
            var result = details.Select(Mapper.Map<ShowHeader>).ToArray();
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets the show by identifier.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns>Show with specific details</returns>
        /// <response code="200">Returns show details.</response>
        /// <response code="404">When show of specified ID doesn't exist.</response>
        [HttpGet]
        [Route("{showId}")]
        [ProducesResponseType(typeof(ShowWithDetailsModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetById(long showId)
        {
            try
            {
                var show = ShowsReader.GetShowById(showId);
                return new ObjectResult(show);
            }
            catch (EntityNotFoundException<ShowWithDetails>)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Creates a new show.
        /// </summary>
        /// <param name="commandModel">Contains information about the show to create.</param>
        /// <returns>ID of created show.</returns>
        /// <response code="201">Returns show details.</response>
        /// <response code="400">When the provided information are invalid.</response>
        [HttpPost]
        [Route("create")]
        [Authorize]
        [ProducesResponseType(typeof(IdResult), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody]AddShowModel commandModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var command = new AddShowCommand();
            Mapper.Map(commandModel, command);

            command.Id = await IdService.GenerateId();
            command.UserName = User.Identity.Name;

            await CommandPublisher.PublishCommand(command);

            var result = new IdResult
            {
                Id = command.Id
            };
            
            return new ObjectResult(result) { StatusCode = (int)HttpStatusCode.Created };
        }

        /// <summary>
        /// Configures the mapper for entities owned by this controller.
        /// </summary>
        /// <param name="cfg">The mapper configuration builder.</param>
        [MapperStartup]
        public static void ConfigureMapper(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ShowWithDetails, ShowWithDetailsModel>();
            cfg.CreateMap<ShowHeader, ShowHeaderModel>();

            cfg.CreateMap<AddShowModel, AddShowCommand>();
        }
    }
}
