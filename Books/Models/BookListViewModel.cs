using HowTo_DBLibrary;

namespace Books.Models
{
    public class BookListViewModel
    {
        public IQueryable<Node>? Nodes;
        public string? SearchText;
    }
}
