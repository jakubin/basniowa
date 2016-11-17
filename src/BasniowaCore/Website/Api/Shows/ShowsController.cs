using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Logic;
using Logic.Commands;
using Website.Models;
using Logic.Common;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using System.Net;
using Common.Startup;
using Website.Infrastructure;
using Logic.Services;
using Common.Cqrs;
using System.Threading.Tasks;

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
        /// Gets or sets the shows provider.
        /// </summary>
        [InjectService]
        public IShowsProvider ShowsProvider { get; set; }

        [InjectService]
        public IUniqueIdService IdService { get; set; }

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
            var details = ShowsProvider.GetAllShows();
            var result = details.Select(Mapper.Map<ShowWithDetailsModel>).ToArray();
            return new ObjectResult(result);
        }

        [HttpGet]
        [Route("{showId}")]
        public IActionResult GetById(long showId)
        {
            try
            {
                var show = ShowsProvider.GetShowById(showId);
                return new ObjectResult(show);
            }
            catch (EntityNotFoundException<ShowWithDetails>)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("create")]
        [Authorize()]
        public async Task<IActionResult> Post([FromBody]AddShowModel commandModel)
        {
            var command = new AddShowCommand();
            Mapper.Map(commandModel, command);

            command.Id = await IdService.GenerateId();
            command.UserName = User.Identity.Name;

            await CommandPublisher.PublishCommand(command);

            var result = new IdResult
            {
                Id = command.Id
            };
            
            return new ObjectResult(result);
        }

        /// <summary>
        /// Configures the mapper for entities owned by this controller.
        /// </summary>
        /// <param name="cfg">The mapper configuration builder.</param>
        [MapperStartup]
        public static void ConfigureMapper(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ShowWithDetails, ShowWithDetailsModel>();

            cfg.CreateMap<AddShowModel, AddShowCommand>();
        }
    }
}
