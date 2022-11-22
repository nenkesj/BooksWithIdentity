using System.Collections.Generic;
using Books.Infrastructure;
using HowTo_DBLibrary;

namespace Books.Models
{
    public class AdminEditViewModel
    {
        public Node Node { get; set; }
        public int? PrevNodeID { get; set;}
        //public string Display { get; set; }
        //public bool Format { get; set; }
    }
}