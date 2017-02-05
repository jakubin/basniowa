using System.Linq;
using System.Net;
using AutoMapper;
using Common.Startup;
using Logic.Common;
using Logic.Shows;
using Microsoft.AspNetCore.Mvc;
using Website.Infrastructure;
using Website.Infrastructure.ErrorHandling;

namespace Website.Api.Shows
{
    /// <summary>
    /// API controller for managing shows.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/shows")]
    [Produces(ContentTypes.ApplicationJson)]
    [ApiExplorerSettings(GroupName = "Shows")]
    public class ShowsQueryController : Controller
    {
        private const string GroupName = "Shows";

        /// <summary>
        /// Gets or sets the shows reader.
        /// </summary>
        [InjectService]
        public IShowsReader ShowsReader { get; set; }

        /// <summary>
        /// Gets or sets the mapper.
        /// </summary>
        [InjectService]
        public IMapper Mapper { get; set; }

        /// <summary>
        /// Gets all shows.
        /// </summary>
        /// <returns>List of all shows with details.</returns>
        /// <response code="200">Returns all shows main information.</response>
        [HttpGet]
        [ApiExplorerSettings(GroupName = GroupName)]
        public ShowHeaderModel[] GetAll()
        {
            var details = ShowsReader.GetAllShows();
            var result = details.Select(Mapper.Map<ShowHeaderModel>).ToArray();
            return result;
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
        [ApiExplorerSettings(GroupName = GroupName)]
        public ShowWithDetailsModel GetById(long showId)
        {
            try
            {
                var show = ShowsReader.GetShowById(showId);
                return Mapper.Map<ShowWithDetailsModel>(show);
            }
            catch (EntityNotFoundException<DataAccess.Shows.Show>)
            {
                throw new HttpErrorException(NotFound());
            }
        }

        /// <summary>
        /// Configures the mapper for entities owned by this controller.
        /// </summary>
        /// <param name="cfg">The mapper configuration builder.</param>
        [MapperStartup]
        public static void ConfigureMapper(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ShowHeader, ShowHeaderModel>();
            cfg.CreateMap<ShowWithDetails, ShowWithDetailsModel>();
        }
    }
}
