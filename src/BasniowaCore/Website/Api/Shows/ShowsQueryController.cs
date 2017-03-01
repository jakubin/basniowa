using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Common.Startup;
using Logic.Shows;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Website.Infrastructure;

namespace Website.Api.Shows
{
    /// <summary>
    /// API controller for managing shows.
    /// </summary>
    /// <seealso cref="Controller" />
    [Route("api/shows")]
    [Produces(ContentTypes.ApplicationJson)]
    public class ShowsQueryController : Controller
    {
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
        /// Gets or sets the show pictures URL proviver.
        /// </summary>
        [InjectService]
        public ShowPictureUrlProvider ShowPictures { get; set; }

        /// <summary>
        /// Gets or sets the content type provider.
        /// </summary>
        [InjectService]
        public IContentTypeProvider ContentTypeProvider { get; set; }

        /// <summary>
        /// Gets all shows.
        /// </summary>
        /// <returns>List of all shows with details.</returns>
        /// <response code="200">Returns all shows main information.</response>
        [HttpGet]
        public IList<ShowHeaderModel> GetAll()
        {
            var details = ShowsReader.GetAllShows();
            var result = new List<ShowHeaderModel>();
            foreach (var showHeader in details)
            {
                var model = Mapper.Map<ShowHeaderModel>(showHeader);
                model.MainShowPictureUrl = ShowPictures.UrlProvider.GetDownloadUrl(showHeader.MainShowPicturePath);
                model.MainShowPictureThumbUrl =
                    ShowPictures.UrlProvider.GetDownloadUrl(showHeader.MainShowPictureThumbPath);

                result.Add(model);
            }

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
        public ShowWithDetailsModel GetById(long showId)
        {
            var show = ShowsReader.GetShowById(showId);
            return Mapper.Map<ShowWithDetailsModel>(show);
        }

        /// <summary>
        /// Gets the show by identifier.
        /// </summary>
        /// <param name="showId">The show identifier.</param>
        /// <returns>Show with specific details</returns>
        /// <response code="200">Returns show details.</response>
        /// <response code="404">When show of specified ID doesn't exist.</response>
        [HttpGet]
        [Route("{showId}/pictures")]
        public async Task<IList<ShowPictureDetailsModel>> GetShowPictures(long showId)
        {
            var pictures = await ShowsReader.GetShowPictures(showId);
            var result = pictures
                .Select(x =>
                {
                    var model = Mapper.Map<ShowPictureDetailsModel>(x);
                    model.ImageUrl = ShowPictures.UrlProvider.GetDownloadUrl(x.ImagePath);
                    model.ThumbUrl = ShowPictures.UrlProvider.GetDownloadUrl(x.ThumbPath);
                    return model;
                })
                .ToList();

            return result;
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
            cfg.CreateMap<ShowPictureDetails, ShowPictureDetailsModel>();
        }
    }
}
