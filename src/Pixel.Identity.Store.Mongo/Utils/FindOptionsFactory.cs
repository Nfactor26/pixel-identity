using MongoDB.Driver;

namespace Pixel.Identity.Store.Mongo.Utils
{
    public static class FindOptionsFactory
    {
        public static FindOptions<TItem> LimitTo<TItem>(int limit)
        {
            return new FindOptions<TItem>()
            {
                Limit = limit,
            };
        }
    }
}
