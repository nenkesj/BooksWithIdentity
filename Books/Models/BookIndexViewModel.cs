using HowTo_DBLibrary;
using System.Web.Mvc;

namespace Books.Models
{
    public class BookIndexViewModel
    {
        public Node? Node { get; set; }
        public Summary? Summary { get; set; }
        public int SummaryId { get; set; }
        public int? OtherChapterSummId { get; set; }
        public int UserChaptSumms { get; set; }
        public IEnumerable<Summary>? OtherSummaries { get; set; }
        public IEnumerable<Node>? ChapterSummSiblings { get; set; }
        public int SentencesNoOf { get; set; }
        [AllowHtml]
        public List<string>? Sentences { get; set; }
        public List<int>? SentenceInParagraph { get; set; }
        public List<bool>? SelectedSentences { get; set; }
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
        public bool HasSummary { get; set; }
        public bool HasOtherSummaries { get; set; }
        public bool HasOtherChapterSummSiblings { get; set; }
        public bool HasChildren { get; set; }
        public int NoOfChildren { get; set; }
        public bool HasParent { get; set; }
        public bool HasNoFigPara { get; set; }
        public bool HasNoTabPara { get; set; }
        public bool ShowingDetails { get; set; }
        public bool ShowingSummary { get; set; }
        [AllowHtml]
        public string? SearchKey { get; set; }
        public string? KeyText { get; set; }
        public string? Category { get; set; }
        public int NoOfKeys { get; set; }
        public List<string>? Keys { get; set; }
        public IEnumerable<Node>? Siblings { get; set; }
        public string? CurrentUser { get; set; }
        public bool Owner { get; set; }

    }
}