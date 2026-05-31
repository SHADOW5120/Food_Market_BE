using Food_Market_BE.Modules.StoreModule.DTOs;
using Food_Market_BE.Modules.StoreModule.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Food_Market_BE.Modules.StoreModule.Helpers
{
    public class StoreSearchHelper
    {
        public static FilterDefinition<Store> BuildFilter(GetStoreQueryDto query)
        {
            var builder = Builders<Store>.Filter;
            var filters = new List<FilterDefinition<Store>>();

            // get valid stores (not deleted)
            filters.Add(builder.Eq(x => x.IsDeleted, false));

            // serch by name (regex)
            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                filters.Add(
                    builder.Regex(
                        x => x.Name,
                        new BsonRegularExpression(query.Search, "i")
                    )
                );
            }

            // rating min
            if (query.MinRating.HasValue)
            {
                filters.Add(builder.Gte(x => x.Rating, query.MinRating.Value));
            }

            // rating max
            if (query.MaxRating.HasValue)
            {
                filters.Add(builder.Lte(x => x.Rating, query.MaxRating.Value));
            }

            return filters.Count > 0
                ? builder.And(filters)
                : builder.Empty;
        }

        public static SortDefinition<Store> BuildSort(GetStoreQueryDto query)
        {
            return query.Sort?.ToLower() switch
            {
                "rating_asc" =>
                    Builders<Store>.Sort.Ascending(x => x.Rating),

                "rating_desc" =>
                    Builders<Store>.Sort.Descending(x => x.Rating),

                "newest" =>
                    Builders<Store>.Sort.Descending(x => x.CreatedAt),

                "oldest" =>
                    Builders<Store>.Sort.Ascending(x => x.CreatedAt),

                "name_asc" =>
                    Builders<Store>.Sort.Ascending(x => x.Name),

                "name_desc" =>
                    Builders<Store>.Sort.Descending(x => x.Name),

                _ =>
                    Builders<Store>.Sort.Descending(x => x.CreatedAt)
            };
        }

        public static (int Skip, int Limit) BuildPaging(GetStoreQueryDto query)
        {
            var page = query.Page <= 0 ? 1 : query.Page;
            var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

            return (
                (page - 1) * pageSize,
                pageSize
            );
        }
    }
}
