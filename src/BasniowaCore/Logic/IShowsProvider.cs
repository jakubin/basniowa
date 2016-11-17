using System.Collections.Generic;

namespace Logic
{
    public interface IShowsProvider
    {
        long AddShow(ShowWithDetails showWithDetails);
        IList<ShowWithDetails> GetAllShows();

        ShowWithDetails GetShowById(long showId);
    }
}