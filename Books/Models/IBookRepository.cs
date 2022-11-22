using HowTo_DBLibrary;

namespace Books.Models
{
    public interface IBookRepository
    {
        IQueryable<Node> Nodes { get; }
        IQueryable<Summary> Summaries { get; }
        IQueryable<Picture> Pictures { get; }
        IQueryable<Key> Keys { get; }
    }
}
