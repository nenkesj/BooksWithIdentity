﻿using System.Collections.Generic;
using Books.Infrastructure;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using HowTo_DBLibrary;

namespace Books.Models
{
    public class SummaryEditViewModel
    {
        public Summary Summary { get; set; }
        public int SentencesNoOf { get; set; }
        [AllowHtml]
        public List<string> Sentences { get; set; }
        public List<int> SentenceInParagraph { get; set; }
        public List<bool> SelectedSentences { get; set; }
        [AllowHtml]
        public List<string> Paragraphs { set; get; }
        public int ParagraphsNoOf { set; get; }
    }
}
