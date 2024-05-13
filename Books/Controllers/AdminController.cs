using Books.Infrastructure;
using Books.Models;
using HowTo_DBLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Books.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly HowToDBContext _context;
        public UserManager<IdentityUser> UserManager { get; set; }

        public AdminController(HowToDBContext context, UserManager<IdentityUser> userMgr)
        {
            _context = context;
            UserManager = userMgr;
        }

        // GET: Admin
        public IActionResult Index() => View(_context.Nodes.Where(n => n.ParentNodeId == 0).OrderBy(n => n.Heading));
        public async Task<IActionResult> Index1()
        {
            var howToDBContext = _context.Nodes.Include(n => n.Tree).Include(n => n.Type);
            return View(await howToDBContext.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Nodes == null)
            {
                return NotFound();
            }

            var node = await _context.Nodes
                .Include(n => n.Tree)
                .Include(n => n.Type)
                .FirstOrDefaultAsync(m => m.NodeId == id);
            if (node == null)
            {
                return NotFound();
            }

            return View(node);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            ViewData["TreeId"] = new SelectList(_context.Trees, "TreeId", "Heading");
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Label");
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NodeId,TreeId,TypeId,ParentNodeId,TreeLevel,Heading,NodeText")] Node node)
        {
            if (ModelState.IsValid)
            {
                _context.Add(node);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TreeId"] = new SelectList(_context.Trees, "TreeId", "Heading", node.TreeId);
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Label", node.TypeId);
            return View(node);
        }

        // GET: Summaries/Create
        public async Task<IActionResult> CreateSummary(int id)
        {
            int linesNoOf, sentencesNoOf, paragraphsNoOf, paragraphPtr, newNoOfParagraphs;
            List<string> lines, sentences, para, paragrphs, newParagraphs;
            List<int> sentenceInParagraph;
            List<bool> selectedSentences;
            Paragraphs paragraphs;
            Summary summary;
            IEnumerable<Summary> summaries;
            IEnumerable<Picture> pictures;

            lines = new List<string>();
            sentences = new List<string>();
            sentenceInParagraph = new List<int>();
            selectedSentences = new List<bool>();
            paragrphs = new List<string>();
            para = new List<string>();
            paragraphs = new Paragraphs();

            bool haspict = false;
            bool hasnofigpara = true;
            bool hasnotabpara = true;

            IdentityUser CurrentUser = await UserManager.GetUserAsync(User);

            //ViewBag.Save = false;
            Node node = _context.Nodes.FirstOrDefault(n => n.NodeId == id);
            paragraphs.TheText = node.NodeText;
            paragraphs.NoOfChars = paragraphs.TheText.Length;

            paragraphs.Paragrphs(out paragraphsNoOf, ref paragrphs, out sentencesNoOf, ref sentences, ref sentenceInParagraph, out linesNoOf, ref lines, 0, false, true, true, false, true, false, true);

            paragraphs.TheText = paragraphs.TheAlteredText;
            paragraphs.NoOfChars = paragraphs.TheAlteredText.Length;

            paragraphs.ListsAndTables(paragraphsNoOf, paragrphs, out newNoOfParagraphs, out newParagraphs, out hasnofigpara, out hasnotabpara);

            for (int i = 0; i < sentencesNoOf; i++)
            {
                selectedSentences.Add(false);
            }

            para.Add("");

            summaries = _context.Summaries.Where(s => s.NodeId == id && s.Owner == CurrentUser.NormalizedEmail);
            if (summaries.Count() > 0)
            {
                summary = summaries.ToArray()[0];
            }
            else
            {
                summary = new Summary();
                summary.NodeId = id;
                summary.Owner = CurrentUser.NormalizedEmail;
            }

            summary.Summary1 = "<h4>Summary: " + node.Heading + "</h4>\r\n";

            pictures = _context.Pictures.Where(pic => pic.NodeId == id);
            if (pictures.Count() > 0)
            {
                haspict = true;
            }

            BookIndexViewModel model = new BookIndexViewModel
            {
                Node = node,
                Summary = summary,
                SentencesNoOf = sentencesNoOf,
                Sentences = sentences,
                SentenceInParagraph = sentenceInParagraph,
                SelectedSentences = selectedSentences,
                Paragraphs = para,
                NoOfParagraphs = 1,
                Paragraph = "",
                DisplayPictures = false,
                HasPicture = haspict,
                NoOfPictures = pictures.Count(),
                PicturePointer = 0,
                Pictures = pictures,
                PictureTitle = "",
                PictureFile = "",
                PictureFixed = false,
                HasSummary = false,
                HasChildren = false,
                NoOfChildren = 0,
                HasParent = false,
                HasNoFigPara = false,
                HasNoTabPara = false,
                ShowingDetails = false,
                ShowingSummary = true,
                SearchKey = "",
                NoOfKeys = 0,
                Keys = null,
                Siblings = null
            };

            return View(model);

        }

        // POST: Summaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSummary(BookIndexViewModel summ, bool save = false)
        {
            int linesNoOf, sentencesNoOf, paragraphsNoOf, paragraphPtr, newNoOfParagraphs, pictPtr;
            string FigNo, FigNoAlt;
            List<string> lines, sentences, paragrphs, newParagraphs;
            Paragraphs paragraphs;
            List<int> sentenceInParagraph;
            List<bool> selectedSentences;
            IEnumerable<Node> nodes;
            IEnumerable<Picture> pictures;
            List<Picture> picts;

            bool haspict = false;
            bool hasnofigpara = true;
            bool hasnotabpara = true;

            sentenceInParagraph = new List<int>();
            selectedSentences = new List<bool>();
            paragrphs = new List<string>();
            sentences = new List<string>();
            lines = new List<string>();
            newParagraphs = new List<string>();
            picts = new List<Picture>();
            paragraphs = new Paragraphs();

            paragraphPtr = 0;
            paragraphs.TheText = summ.Summary.Summary1;

            for (int i = 0; i < summ.SentencesNoOf; i++)
            {
                if (summ.SentenceInParagraph[i] != paragraphPtr)
                {
                    if (paragraphs.TheText.Length > 2)
                    {
                        if (paragraphs.TheText.Substring(paragraphs.TheText.Length - 2) != "\r\n") { paragraphs.TheText += "\r\n"; }
                    }
                    paragraphPtr = summ.SentenceInParagraph[i];
                }
                if (summ.SelectedSentences[i])
                {
                    if (paragraphs.TheText.Length > 2)
                    {
                        if (paragraphs.TheText.Substring(paragraphs.TheText.Length - 2) == "\r\n")
                        {
                            paragraphs.TheText += summ.Sentences[i];
                        }
                        else
                        {
                            paragraphs.TheText += " " + summ.Sentences[i];
                        }
                    }
                }
            }

            paragraphs.NoOfChars = paragraphs.TheText.Length;

            paragraphs.Paragrphs(out paragraphsNoOf, ref paragrphs, out sentencesNoOf, ref sentences, ref sentenceInParagraph, out linesNoOf, ref lines, 0, false, true, true, false, true, false, false);

            summ.Summary.Summary1 = paragraphs.TheAlteredText;

            paragrphs = new List<string>();
            sentences = new List<string>();
            lines = new List<string>();

            paragraphs.Paragrphs(out paragraphsNoOf, ref paragrphs, out sentencesNoOf, ref sentences, ref sentenceInParagraph, out linesNoOf, ref lines, 0, false, true, true, false, true, false, true);

            paragraphs.ListsAndTables(paragraphsNoOf, paragrphs, out newNoOfParagraphs, out newParagraphs, out hasnofigpara, out hasnotabpara);

            nodes = _context.Nodes.Where(n => n.NodeId == summ.Summary.NodeId);
            pictures = _context.Pictures.Where(pic => pic.NodeId == summ.Summary.NodeId);
            if (pictures.Count() > 0)
            {
                haspict = true;
            }

            //ViewBag.Save = save;

            //BookIndexViewModel model = new BookIndexViewModel
            //{
            summ.Node = nodes.FirstOrDefault(n => n.NodeId == summ.Summary.NodeId);
            //Summary = summ.Summary,
            //SentencesNoOf = summ.SentencesNoOf,
            //Sentences = summ.Sentences,
            //SentenceInParagraph = summ.SentenceInParagraph,
            //SelectedSentences = summ.SelectedSentences,
            summ.Paragraphs = newParagraphs;
            summ.NoOfParagraphs = newNoOfParagraphs;
            summ.Paragraph = "";
            summ.DisplayPictures = true;
            summ.HasPicture = haspict;
            summ.NoOfPictures = pictures.Count();
            summ.PicturePointer = 0;
            summ.Pictures = pictures;
            summ.PictureTitle = "";
            summ.PictureFile = "";
            summ.PictureFixed = false;
            summ.HasSummary = false;
            summ.HasChildren = false;
            summ.NoOfChildren = 0;
            summ.HasParent = false;
            summ.HasNoFigPara = false;
            summ.HasNoTabPara = false;
            summ.ShowingDetails = false;
            summ.ShowingSummary = true;
            summ.SearchKey = "";
            summ.NoOfKeys = 0;
            summ.Keys = null;
            summ.Siblings = null;
            //};
            if (ModelState.IsValid)
            {
                if (save)
                {
                    _context.Add(summ.Summary);
                    await _context.SaveChangesAsync();
                    TempData["message"] = string.Format("Summary: {0} ... has been created for Node: {1}", summ.Summary.SummaryId, summ.Summary.NodeId);
                    return RedirectToAction("Details", "Books", new { id = summ.Summary.NodeId });
                }
                else
                {
                    return View(summ);
                }
            }
            else
            {
                // there is something wrong with the data values
                return View(summ);
            }
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Nodes == null)
            {
                return NotFound();
            }
            var node = await _context.Nodes.Include(n => n.Tree)
                .Include(n => n.Type).FirstOrDefaultAsync(m => m.NodeId == id); ;
            if (node == null)
            {
                return NotFound();
            }
            ViewData["TreeId"] = new SelectList(_context.Trees, "TreeId", "Heading", node.TreeId);
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Label", node.TypeId);
            TempData["oldtext"] = node.NodeText;
            TempData["message"] = null;
            ViewBag.SelectedNodeHeading = node.Heading;
            return View(node);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NodeId,TreeId,TypeId,ParentNodeId,TreeLevel,Heading,NodeText,Owner")] Node node, int ParentNodeID = -1, int TreeLevel = -1, bool save = true)
        {
            int LinesNoOf, SentencesNoOf, ParagraphsNoOf;
            List<string> lines, sentences, paragrphs, newParagraphs;
            Paragraphs paragraphs;
            List<int> SentenceInParagraph;

            SentenceInParagraph = new List<int>();
            paragrphs = new List<string>();
            sentences = new List<string>();
            lines = new List<string>();
            newParagraphs = new List<string>();
            paragraphs = new Paragraphs();
            if (id != node.NodeId)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(node.Heading))
            {
                ModelState.AddModelError("Heading", "Please enter a Heading");
            }
            if (ParentNodeID != -1)
            {
                node.TreeId = 2;
                node.TypeId = 2;
                node.ParentNodeId = ParentNodeID;
                node.TreeLevel = (short)TreeLevel;
            }
            ViewBag.SelectedNodeHeading = node.Heading;

            paragraphs.TheText = node.NodeText;
            paragraphs.NoOfChars = paragraphs.TheText.Length;
            paragraphs.Paragrphs(out ParagraphsNoOf, ref paragrphs, out SentencesNoOf, ref sentences, ref SentenceInParagraph, out LinesNoOf, ref lines, 0, false, true, true, false, true, false, false);
            node.NodeText = paragraphs.TheAlteredText;

            if (node.Heading.Length > 50) { node.Heading = node.Heading.Substring(0, 50); };

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(node);
                    await _context.SaveChangesAsync();
                    TempData["message"] = string.Format("Node: {0}, {1} ... has been edited", node.NodeId, node.Heading);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NodeExists(node.NodeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), "Books", new { id = node.NodeId });
            }
            ViewData["TreeId"] = new SelectList(_context.Trees, "TreeId", "Heading", node.TreeId);
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Label", node.TypeId);
            return View(node);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Nodes == null)
            {
                return NotFound();
            }

            var node = await _context.Nodes
                .Include(n => n.Tree)
                .Include(n => n.Type)
                .FirstOrDefaultAsync(m => m.NodeId == id);
            if (node == null)
            {
                return NotFound();
            }

            return View(node);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Nodes == null)
            {
                return Problem("Entity set 'HowToDBContext.Nodes'  is null.");
            }
            var node = await _context.Nodes.FindAsync(id);
            if (node != null)
            {
                _context.Nodes.Remove(node);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NodeExists(int id)
        {
            return _context.Nodes.Any(e => e.NodeId == id);
        }
        private bool SummaryExists(int id)
        {
            return _context.Summaries.Any(e => e.SummaryId == id);
        }
        public ViewResult UnDo(int id)
        {
            Node node = _context.Nodes
                .FirstOrDefault(n => n.NodeId == id);

            ViewBag.SelectedNodeHeading = node.Heading;

            node.NodeText = (string)TempData["oldtext"];

            AdminEditViewModel model = new AdminEditViewModel
            {
                Node = node,
                PrevNodeID = id
            };

            return View("Edit", model);
        }
        // GET: Admin/New/5
        public async Task<IActionResult> New(int? id)
        {
            if (id == null || _context.Nodes == null)
            {
                return NotFound();
            }
            Node currNode = await _context.Nodes.Include(n => n.Tree)
                .Include(n => n.Type).FirstOrDefaultAsync(m => m.NodeId == id);
            if (currNode == null)
            {
                return NotFound();
            }
            ViewData["TreeId"] = new SelectList(_context.Trees, "TreeId", "Heading", currNode.TreeId);
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Label", currNode.TypeId);
            TempData["message"] = null;
            Node node = new Node();
            node.Heading = "Enter Heading Here";
            node.NodeText = "Enter Text Here";
            node.TreeId = 2;
            node.TypeId = 2;
            node.ParentNodeId = currNode.ParentNodeId;
            node.TreeLevel = currNode.TreeLevel;
            node.Owner = currNode.Owner;
            ViewBag.ReturnNode = id;
            return View(node);
        }

        // POST: Admin/New/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New([Bind("NodeId,TreeId,TypeId,ParentNodeId,TreeLevel,Heading,NodeText,Owner")] Node node)
        {
            int LinesNoOf, SentencesNoOf, ParagraphsNoOf;
            List<string> lines, sentences, paragrphs, newParagraphs;
            Paragraphs paragraphs;
            List<int> SentenceInParagraph;

            SentenceInParagraph = new List<int>();
            paragrphs = new List<string>();
            sentences = new List<string>();
            lines = new List<string>();
            newParagraphs = new List<string>();
            paragraphs = new Paragraphs();

            if (ModelState.IsValid)
            {
                if (node.Heading == "Enter Heading Here" && node.NodeText.IndexOf((Char)10) > 1)
                {
                    node.Heading = node.NodeText.Substring(0, node.NodeText.IndexOf((Char)10) - 1);
                    node.NodeText = node.NodeText.Substring(node.NodeText.IndexOf((char)10) + 1);
                    paragraphs.TheText = node.NodeText;
                    paragraphs.NoOfChars = paragraphs.TheText.Length;
                    paragraphs.Paragrphs(out ParagraphsNoOf, ref paragrphs, out SentencesNoOf, ref sentences, ref SentenceInParagraph, out LinesNoOf, ref lines, 0, false, true, true, false, true, false, false);
                    node.NodeText = paragraphs.TheAlteredText;
                }
                if (node.Heading.Length > 50)
                {
                    node.NodeText = node.Heading + (Char)13 + (Char)10 + node.NodeText;
                    node.Heading = node.Heading.Substring(0, 50);
                };
                _context.Add(node);
                await _context.SaveChangesAsync();
                ViewBag.SelectedNodeHeading = node.Heading;
                return RedirectToAction("Details", "Books", new { id = node.NodeId });
            }
            ViewData["TreeId"] = new SelectList(_context.Trees, "TreeId", "Heading", node.TreeId);
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Label", node.TypeId);
            return View(node);
        }
        // GET: Admin/NewChild/5
        public async Task<IActionResult> NewChild(int? id)
        {
            if (id == null || _context.Nodes == null)
            {
                return NotFound();
            }
            Node currNode = await _context.Nodes.Include(n => n.Tree)
                .Include(n => n.Type).FirstOrDefaultAsync(m => m.NodeId == id);
            if (currNode == null)
            {
                return NotFound();
            }
            ViewData["TreeId"] = new SelectList(_context.Trees, "TreeId", "Heading", currNode.TreeId);
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Label", currNode.TypeId);
            TempData["message"] = null;
            Node node = new Node();
            node.Heading = "Enter Heading Here";
            node.NodeText = "Enter Text Here";
            node.TreeId = 2;
            node.TypeId = 2;
            node.ParentNodeId = currNode.NodeId;
            node.TreeLevel = (short)(currNode.TreeLevel + 1);
            node.Owner = currNode.Owner;
            ViewBag.ReturnNode = id;
            return View(node);
        }

        // POST: Admin/NewChild/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewChild([Bind("NodeId,TreeId,TypeId,ParentNodeId,TreeLevel,Heading,NodeText,Owner")] Node node)
        {
            int LinesNoOf, SentencesNoOf, ParagraphsNoOf;
            List<string> lines, sentences, paragrphs, newParagraphs;
            Paragraphs paragraphs;
            List<int> SentenceInParagraph;

            SentenceInParagraph = new List<int>();
            paragrphs = new List<string>();
            sentences = new List<string>();
            lines = new List<string>();
            newParagraphs = new List<string>();
            paragraphs = new Paragraphs();

            if (ModelState.IsValid)
            {
                if (node.Heading == "Enter Heading Here" && node.NodeText.IndexOf((Char)10) > 1)
                {
                    node.Heading = node.NodeText.Substring(0, node.NodeText.IndexOf((Char)10) - 1);
                    node.NodeText = node.NodeText.Substring(node.NodeText.IndexOf((char)10) + 1);
                    paragraphs.TheText = node.NodeText;
                    paragraphs.NoOfChars = paragraphs.TheText.Length;
                    paragraphs.Paragrphs(out ParagraphsNoOf, ref paragrphs, out SentencesNoOf, ref sentences, ref SentenceInParagraph, out LinesNoOf, ref lines, 0, false, true, true, false, true, false, false);
                    node.NodeText = paragraphs.TheAlteredText;
                }
                if (node.Heading.Length > 50)
                {
                    node.NodeText = node.Heading + (Char)13 + (Char)10 + node.NodeText;
                    node.Heading = node.Heading.Substring(0, 50);
                };
                _context.Add(node);
                await _context.SaveChangesAsync();
                ViewBag.SelectedNodeHeading = node.Heading;
                return RedirectToAction("Details", "Books", new { id = node.NodeId });
            }
            ViewData["TreeId"] = new SelectList(_context.Trees, "TreeId", "Heading", node.TreeId);
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Label", node.TypeId);
            return View(node);
        }
        // GET: Admin/NewBook/5
        public async Task<IActionResult> NewBook()
        {
            TempData["message"] = null;
            Node node = new Node();
            node.Heading = "Enter Heading Here";
            node.NodeText = "Enter Text Here";
            node.TreeId = 2;
            node.TypeId = 2;
            node.ParentNodeId = 0;
            node.TreeLevel = 1;
            IdentityUser CurrentUser = await UserManager.GetUserAsync(User);
            node.Owner = CurrentUser.NormalizedEmail;
            return View(node);
        }
        // POST: Admin/NewBook/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewBook([Bind("NodeId,TreeId,TypeId,ParentNodeId,TreeLevel,Heading,NodeText,Owner")] Node node)
        {
            if (ModelState.IsValid)
            {
                _context.Add(node);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Books", new { id = node.NodeId });
            }
            ViewData["TreeId"] = new SelectList(_context.Trees, "TreeId", "Heading", node.TreeId);
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Label", node.TypeId);
            return View(node);
        }
        // GET: Summaries/EditSummary/5
        public async Task<IActionResult> EditSummary(int? id)
        {
            if (id == null || _context.Summaries == null)
            {
                return NotFound();
            }

            var summary = await _context.Summaries.FindAsync(id);
            if (summary == null)
            {
                return NotFound();
            }
            //ViewData["NodeId"] = new SelectList(_context.Nodes, "NodeId", "Heading", summary.NodeId);
            TempData["message"] = null;
            return View(summary);
        }
        // POST: Admin/EditSummary/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSummary(int id, [Bind("SummaryId, NodeId, Summary1")] Summary summary)
        {
            int LinesNoOf, SentencesNoOf, ParagraphsNoOf;
            List<string> lines, sentences, paragrphs, newParagraphs;
            Paragraphs paragraphs;
            List<int> SentenceInParagraph;

            SentenceInParagraph = new List<int>();
            paragrphs = new List<string>();
            sentences = new List<string>();
            lines = new List<string>();
            newParagraphs = new List<string>();
            paragraphs = new Paragraphs();
            if (id != summary.SummaryId)
            {
                return NotFound();
            }

            paragraphs.TheText = summary.Summary1;
            paragraphs.NoOfChars = paragraphs.TheText.Length;
            paragraphs.Paragrphs(out ParagraphsNoOf, ref paragrphs, out SentencesNoOf, ref sentences, ref SentenceInParagraph, out LinesNoOf, ref lines, 0, false, true, true, false, true, false, false);
            summary.Summary1 = paragraphs.TheAlteredText;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(summary);
                    await _context.SaveChangesAsync();
                    TempData["message"] = string.Format("Summary: {0}... has been edited", summary.SummaryId);
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction(nameof(Details), "Books", new { id = summary.NodeId });
            }
            return View(summary);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToActionResult NewPicture(int id, IFormFile picturefile, string picturetitle = "")
        {
            string book = "";
            string chapter = "";
            string path = "";

            Node node = _context.Nodes.FirstOrDefault(n => n.NodeId == id);
            TempData["message"] = string.Format("Picture: {0} for Node: {1}, {2} ... NOT created SOMETHING WENT WRONG", picturetitle, node.NodeId, node.Heading);
            do
            {
                if (node.TreeLevel == 2)
                {
                    chapter = "1";
                    if (node.Heading.Length > 8)
                    {
                        if (node.Heading.Substring(0, 7).ToLower() == "chapter")
                        {
                            chapter = node.Heading.Substring(8);
                            if (chapter.Contains("."))
                            {
                                chapter = chapter.Replace(".", " ");
                            }
                            if (chapter.IndexOf(" ") != -1)
                            {
                                chapter = chapter.Substring(0, chapter.IndexOf(" "));
                            }
                        }
                    }
                    if (node.Heading.Length > 5)
                    {
                        if (node.Heading.Substring(0, 5).ToLower() == "book ")
                        {
                            chapter = node.Heading.Substring(5);
                            if (chapter.Contains("."))
                            {
                                chapter = chapter.Replace(".", " ");
                            }
                            if (chapter.IndexOf(" ") != -1)
                            {
                                chapter = chapter.Substring(0, chapter.IndexOf(" "));
                            }
                        }
                    }
                }
                if (node.ParentNodeId != 0)
                {
                    node = _context.Nodes.FirstOrDefault(n => n.NodeId == node.ParentNodeId);
                }
            }
            while (node.TreeLevel > 1);

            // Image directory name doesnt match heading in the following cases

            switch (node.Heading)
            {
                case "802.11 Wireless Networks":
                    book = "802.11";
                    break;
                case "ADO NET 4 Entity Framework":
                    book = "ADO NET Entity Framework";
                    break;
                case "ASP NET 2 Developing Web Applications ":
                    book = "Developing Web Applications";
                    break;
                case "ASP NET 4 MVC 5 Programming (Microsoft)":
                    book = "MVC Programming (Microsoft)";
                    break;
                case "ASP NET 4 MVC 5 (Pro)":
                    book = "MVC 5 (Pro)";
                    break;
                case "ASP NET 4 Web API 2 for MVC":
                    book = "Web API 2 for MVC";
                    break;
                case "ASP NET 4 Web Forms ":
                    book = "Web Forms";
                    break;
                case "ASP NET Core MVC 2 ":
                    book = "ASP NET Core";
                    break;
                case "ASP NET Core 2 MVC ":
                    book = "ASP NET Core";
                    break;
                case "ASP NET and AJAX":
                    book = "Ajax";
                    break;
                case "Azure Introduction":
                    book = "Azure Intro";
                    break;
                case "Bootstrap 3 ":
                    book = "Bootstrap";
                    break;
                case "Euclid's Elements":
                    book = "Euclids Elements";
                    break;
                case "Excel 2002 Visual Basic for Application Step by St":
                    book = "Excel VBA";
                    break;
                case "IIS 7.0 (Internet Information Services)":
                    book = "IIS 7.0";
                    break;
                case "JavaScript The Definitive Guide":
                    book = "JavaScript";
                    break;
                case "LINQ in .NET 4":
                    book = "LINQ in NET 4";
                    break;
                case "Ninject for Dependency Injection":
                    book = "Ninject";
                    break;
                case "PayPal APIs":
                    book = "PayPal";
                    break;
                case "ReactJS Pro":
                    book = "ReactJSPro";
                    break;
                case "Redux - Taming the State in React":
                    book = "Redux";
                    break;
                case "TCP/IP Illustrated Volume 1":
                    book = "TCPIP";
                    break;
                case "Visual C# 2013 Step by Step":
                    book = "CSharp 2013 Step by Step";
                    break;
                case "Windows Communication Foundation 4":
                    book = "WCF 4";
                    break;
                case "Wireshark 101":
                    book = "Wireshark";
                    break;
                default:
                    book = node.Heading;
                    break;
            }
            node = _context.Nodes.FirstOrDefault(n => n.NodeId == id);
            if (book != "")
            {
                path = "images\\" + book + "\\";
                if (book != "" && chapter != "")
                {
                    if (book == "Euclids Elements")
                    {
                        path = "images\\" + book + "\\Book " + chapter + "\\";
                    }
                    else
                    {
                        path = "images\\" + book + "\\Chapter " + chapter + "\\";
                    }
                }
                Picture dbEntry = new Picture();
                dbEntry.NodeId = id;
                dbEntry.TypeId = 14;
                dbEntry.Picture1 = path + picturefile.FileName;
                dbEntry.Title = picturetitle;
                dbEntry.PictureSize = 0;
                dbEntry.DisplayAt = 0;
                dbEntry.DisplayStopAt = 0;
                dbEntry.InfoId = 0;
                _context.Pictures.Add(dbEntry);
                _context.SaveChanges();
                TempData["message"] = string.Format("Picture: {0} for Node: {1}, {2} ... has been created", picturetitle, node.NodeId, node.Heading);
            }
            else
            {
                TempData["message"] = string.Format("Picture: {0} for Node: {1}, {2} ... NOT created ONLY WORKS IN INTERNET EXPLORER, CHROME OR FIREFOX", picturetitle, node.NodeId, node.Heading);
            }

            ViewBag.SelectedNodeHeading = node.Heading;

            return RedirectToAction("Details", "Books", new { id = node.NodeId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public RedirectToActionResult NewKey(int id, string keytext = "", string category = "None")
        {
            Key k = new Key();
            k.NodeId = id;
            k.KeyText = keytext;
            k.Category = category;
            k.TreeId = 2;
            k.TypeId = 7;
            _context.Add(k);
            _context.SaveChanges();
            Node node = _context.Nodes.Where(n => n.NodeId == id).FirstOrDefault();
            TempData["message"] = string.Format("Key: {0} for Node: {1}, {2} ... has been created", keytext, id, node.Heading);

            ViewBag.SelectedNodeHeading = node.Heading;

            return RedirectToAction("Details", "Books", new { id = node.NodeId });
        }
    }
}
