using Food_Market_BE.Modules.FavoriteModule.Models;

namespace Food_Market_BE.Modules.FavoriteModule.Helpers
{
    public static class FavoriteHelper
    {
        public static bool ExistsInFavorites(IEnumerable<Favorite> favorites, string productId)
        {
            return favorites.Any(x => x.ProductId == productId);
        }

        public static List<Favorite> SortNewestFirst(IEnumerable<Favorite> favorites)
        {
            return favorites
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
        }
    }
}
