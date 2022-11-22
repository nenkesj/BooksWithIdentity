using Microsoft.EntityFrameworkCore.Migrations;
using HowTo_DBLibrary;

namespace Books.Models
{
    public class EFBookRepository : IBookRepository
    {
        private HowToDBContext context;
        public EFBookRepository(HowToDBContext ctx)
        {
            context = ctx;
        }
        public IQueryable<Node> Nodes => context.Nodes;
        public IQueryable<Summary> Summaries => context.Summaries;
        public IQueryable<Picture> Pictures => context.Pictures;
        public IQueryable<Key> Keys => context.Keys;
    }
}
