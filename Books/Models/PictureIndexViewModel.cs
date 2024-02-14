using HowTo_DBLibrary;

namespace Books.Models
{
    public class PictureIndexViewModel
    {
        public Node Node { get; set; }
        public bool HasPicture { get; set; }
        public int NoOfPictures { get; set; }
        public IEnumerable<Picture> Pictures { get; set; }
        public int PicturePointer { get; set; }
        public bool Caption { get; set; }
    }
}