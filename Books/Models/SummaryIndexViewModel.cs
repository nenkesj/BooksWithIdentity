using HowTo_DBLibrary;
using System.Web.Mvc;

namespace Books.Models
{
    public class SummaryIndexViewModel
    {
        public Node? Node { get; set; }
        public Summary? Summary { get; set; }
        [AllowHtml]
        public List<string>? Paragraphs { set; get; }
        public int NoOfParagraphs { get; set; }
        public string? Paragraph { get; set; }
        public bool DisplayPictures { get; set; }
        public bool HasPicture { get; set; }
        public int NoOfPictures { get; set; }
        public int PicturePointer { get; set; }
        public IEnumerable<Picture>? Pictures { get; set; }
        public string? PictureTitle { get; set; }
        //public System.Web.HttpPostedFileWrapper PictureFile { get; set; }
        public string? PictureFile { get; set; }

        public bool PictureFixed { get; set; }
        public bool HasNoFigPara { get; set; }
        public bool HasNoTabPara { get; set; }
        public bool ShowingDetails { get; set; }
        public bool ShowingSummary { get; set; }
        [AllowHtml]
        public bool Owner { get; set; }

    }
}