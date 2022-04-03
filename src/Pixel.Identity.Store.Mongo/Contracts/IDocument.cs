namespace Pixel.Identity.Store.Mongo.Contracts
{   
    public interface IDocument<TKey> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Identifier for the document
        /// </summary>
        TKey Id { get; set; }

        /// <summary>
        /// Version of the document
        /// </summary>
        int Version { get; set; }
    }
}
