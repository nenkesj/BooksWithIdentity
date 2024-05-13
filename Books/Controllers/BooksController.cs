
using Books.Infrastructure;
using Books.Models;
using HowTo_DBLibrary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Books.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly HowToDBContext _context;
        public UserManager<IdentityUser> UserManager { get; set; }
        public BooksController(HowToDBContext context, UserManager<IdentityUser> userMgr)
        {
            _context = context;
            UserManager = userMgr;
        }

        // GET: Books

        public IActionResult Index() => View(_context.Nodes.Where(n => n.ParentNodeId == 0).OrderBy(n => n.Heading));
        public async Task<IActionResult> Index1()
        {
            var howToDBContext = _context.Nodes.Include(n => n.Tree).Include(n => n.Type);
            return View(await howToDBContext.ToListAsync());
        }
        public ViewResult Index2(int? id, string Display = "Details", bool picturefixed = false, int pictureptr = 0, string searchkey = "")
        {
            bool haspict = false;
            bool hassumm = false;
            bool haschild = false;
            bool hasparent = true;
            bool showingdetails = true;
            bool showingsummary = false;
            bool hasnofigpara = true;
            bool hasnotabpara = true;
            bool NodeIDOK = false;
            int noofchildren = 0;
            int newNoOfParagraphs = 0;


            int LinesNoOf, SentencesNoOf, ParagraphsNoOf, noofkeys;
            List<string> lines, sentences, paragrphs, newParagraphs, keyvalues;
            Paragraphs paragraphs;
            List<int> SentenceInParagraph;
            List<Node> MatchingNodes;
            IEnumerable<Node> nodes;
            IEnumerable<Summary> summaries;
            IEnumerable<Key> keys;
            IEnumerable<Node> children;
            IEnumerable<Node> siblings;
            IEnumerable<Picture> pictures;

            MatchingNodes = new List<Node>();
            SentenceInParagraph = new List<int>();
            paragrphs = new List<string>();
            sentences = new List<string>();
            lines = new List<string>();
            newParagraphs = new List<string>();
            keyvalues = new List<string>();
            paragraphs = new Paragraphs();

            nodes = _context.Nodes.Where(n => n.NodeId == id);
            summaries = _context.Summaries.Where(n => n.NodeId == id);
            pictures = _context.Pictures.Where(pic => pic.NodeId == id);
            children = _context.Nodes.Where(n => n.ParentNodeId == id);
            siblings = _context.Nodes.Where(n => n.ParentNodeId == nodes.Single().ParentNodeId);

            if (searchkey == "")
            {
                ViewData["SearchKey"] = searchkey;
                ViewData["SearchMsg"] = "";
                keys = _context.Keys.Where(k => k.TreeId == 2 && k.NodeId == id);
            }
            else
            {
                keys = _context.Keys.Where(k => k.TreeId == 2 && k.KeyText == searchkey);
                noofkeys = keys.Count();

                if (keys.Count() > 0)
                {
                    MatchingNodes.Clear();
                    foreach (Key k in keys)
                    {
                        Node n = _context.Nodes.Single(node => node.NodeId == k.NodeId);
                        MatchingNodes.Add(n);
                        if (k.NodeId == id)
                        {
                            NodeIDOK = true;
                        }
                    }
                    if (!NodeIDOK)
                    {
                        ViewData["SearchReturn"] = id;
                        id = MatchingNodes.Single().NodeId;
                    }
                    children = MatchingNodes;
                    int noofchild = children.Count();
                    int noofmatchingnodes = MatchingNodes.Count();
                    ViewData["SearchMsg"] = "Searched on - ";
                    ViewData["SearchKey"] = searchkey;
                }
                else
                {
                    ViewData["SearchMsg"] = "No Hits on - ";
                    ViewData["SearchKey"] = searchkey;
                    children = _context.Nodes.Where(n => n.ParentNodeId == id);
                }
                nodes = _context.Nodes.Where(n => n.NodeId == id);
                summaries = _context.Summaries.Where(n => n.NodeId == id);
                pictures = _context.Pictures.Where(pic => pic.NodeId == id);
            }

            foreach (Key key in keys)
            {
                if (!keyvalues.Contains(key.KeyText))
                {
                    keyvalues.Add(key.KeyText);
                }
            }

            if (children.Count() > 0)
            {
                haschild = true;
                noofchildren = children.Count();
            }

            if (summaries.Count() > 0)
            {
                hassumm = true;
            }

            if (nodes.Single().TreeLevel == 1)
            {
                hasparent = false;
            }

            if (pictures.Count() > 0)
            {
                haspict = true;
            }

            if (Display == "Details")
            {
                paragraphs.TheText = nodes.FirstOrDefault().NodeText;
                paragraphs.NoOfChars = paragraphs.TheText.Length;

                showingdetails = true;
                showingsummary = false;
            }

            if (Display == "Summary")
            {
                paragraphs.TheText = summaries.FirstOrDefault().Summary1;
                paragraphs.NoOfChars = paragraphs.TheText.Length;

                showingsummary = true;
                showingdetails = false;
            }

            paragraphs.Paragrphs(out ParagraphsNoOf, ref paragrphs, out SentencesNoOf, ref sentences, ref SentenceInParagraph, out LinesNoOf, ref lines, 0, false, true, true, false, true, false, true);
            paragraphs.ListsAndTables(ParagraphsNoOf, paragrphs, out newNoOfParagraphs, out newParagraphs, out hasnofigpara, out hasnotabpara);

            //ViewBag.Pictures = pictures;
            ViewBag.SelectedNodeHeading = nodes.Single().Heading;

            BookIndexViewModel model = new BookIndexViewModel
            {
                Node = nodes.Single(),
                NoOfParagraphs = newNoOfParagraphs,
                Paragraphs = newParagraphs,
                Paragraph = "",
                DisplayPictures = true,
                HasPicture = haspict,
                NoOfPictures = pictures.Count(),
                PicturePointer = pictureptr,
                Pictures = pictures,
                PictureTitle = "",
                PictureFile = "",
                PictureFixed = picturefixed,
                HasSummary = hassumm,
                HasChildren = haschild,
                NoOfChildren = noofchildren,
                HasParent = hasparent,
                HasNoFigPara = hasnofigpara,
                HasNoTabPara = hasnotabpara,
                ShowingDetails = showingdetails,
                ShowingSummary = showingsummary,
                SearchKey = searchkey,
                NoOfKeys = keyvalues.Count(),
                Keys = keyvalues,
                Siblings = siblings
            };

            return View(model);
        }

        public RedirectToActionResult Up(int? id)
        {
            Node node = _context.Nodes
                .FirstOrDefault(n => n.NodeId == id);

            Node parent = _context.Nodes
                .FirstOrDefault(n => n.NodeId == node.ParentNodeId);

            ViewBag.SelectedNodeHeading = parent.Heading;

            return RedirectToAction("Details", new { id = node.ParentNodeId });
        }

        public RedirectToActionResult Down(int? id, string category = null)
        {
            ViewBag.OldCategory = category;

            Node node = _context.Nodes
                .FirstOrDefault(n => n.ParentNodeId == id);

            ViewBag.SelectedNodeHeading = node.Heading;

            return RedirectToAction("Details", new { id = node.NodeId });
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id, string Display = "Details", bool picturefixed = false, int pictureptr = 0, string searchkey = "", string keyText = "", string Distinct = "All", string Category = "All", int summaryid = 0, int OtherChapterSummId = 0)
        {
            bool haspict = false;
            bool hassumm = false;
            bool hasothersumm = false;
            bool hasotherchaptersummsiblings = false;
            bool haschild = false;
            bool hasparent = true;
            bool owner = false;
            bool showingdetails = true;
            bool showingsummary = false;
            bool hasnofigpara = true;
            bool hasnotabpara = true;
            bool NodeIDOK = false;
            int noofchildren = 0;
            int newNoOfParagraphs = 0;
            int summUser = 0;
            int noofsummaries = 0;
            int noofchaptersummsiblings = 0;
            int userchaptsumms = 0;


            int LinesNoOf, SentencesNoOf, ParagraphsNoOf, noofkeys;
            List<string> lines, sentences, paragrphs, newParagraphs, keyvalues;
            Paragraphs paragraphs;
            Node node, appnode;
            Summary summ;
            List<int> SentenceInParagraph;
            List<Node> MatchingNodes;
            IQueryable<Node> nodes;
            IQueryable<Node> chaptersummsiblings;
            IEnumerable<Summary> summaries;
            IEnumerable<Summary> othersummaries;
            IEnumerable<Key> keys;
            IEnumerable<Node> children;
            IQueryable<Node> siblings, newsiblings;
            IEnumerable<Picture> pictures;

            MatchingNodes = new List<Node>();
            SentenceInParagraph = new List<int>();
            paragrphs = new List<string>();
            sentences = new List<string>();
            lines = new List<string>();
            newParagraphs = new List<string>();
            keyvalues = new List<string>();
            paragraphs = new Paragraphs();

            ViewBag.KeyText = keyText;
            ViewBag.Distinct = Distinct;
            ViewBag.Category = Category;

            IdentityUser CurrentUser = await UserManager.GetUserAsync(User);

            if (OtherChapterSummId > 0)
            {
                id = OtherChapterSummId;
            }

            nodes = _context.Nodes.Where(m => m.NodeId == id);
            node = nodes.FirstOrDefault();
            pictures = _context.Pictures.Where(pic => pic.NodeId == id);
            children = _context.Nodes.Where(n => n.ParentNodeId == id);
            siblings = _context.Nodes.Where(n => n.ParentNodeId == node.ParentNodeId && n.Owner == node.Owner).OrderBy(n => n.NodeId);
            if (node.TreeLevel == 3 && OtherChapterSummId == 0)
            {
                chaptersummsiblings = _context.Nodes.Where(n => n.ParentNodeId == node.ParentNodeId && n.Heading == "Chapter Summary");
                noofchaptersummsiblings = _context.Summaries.Where(n => n.NodeId == id).Count();
                if (noofchaptersummsiblings > 1)
                {
                    hasotherchaptersummsiblings = true;
                    ViewData["OtherChapterSummId"] = new SelectList(chaptersummsiblings.OrderByDescending(s => s.Views).Take(5), "NodeId", "Owner");
                    userchaptsumms = siblings.Where(s => s.Owner == CurrentUser.NormalizedEmail).Count();
                }
            }
            else
            {
                chaptersummsiblings = null;
                hasotherchaptersummsiblings = false;
            }
            keys = _context.Keys.Where(k => k.NodeId == id);

            summaries = _context.Summaries.Where(n => n.NodeId == id);
            noofsummaries = _context.Summaries.Where(n => n.NodeId == id).Count();
            othersummaries = summaries;

            if (summaries.Any())
            {
                summUser = summaries.Where(s => s.Owner == CurrentUser.NormalizedEmail).Count();
                if (summUser > 0)
                {
                    summ = summaries.Where(s => s.Owner == CurrentUser.NormalizedEmail).FirstOrDefault();
                    hassumm = true;
                    if (noofsummaries > 1)
                    {
                        hasothersumm = true;
                        othersummaries = summaries;
                        ViewData["SummaryId"] = new SelectList(othersummaries.OrderByDescending(s => s.Views).Take(5), "SummaryId", "Owner");
                    }
                }
                else
                {
                    summ = summaries.FirstOrDefault();
                    hassumm = false;
                    hasothersumm = true;
                    othersummaries = summaries;
                    ViewData["SummaryId"] = new SelectList(othersummaries.OrderByDescending(s => s.Views).Take(5), "SummaryId", "Owner");
                }
            }
            else
            {
                summ = summaries.FirstOrDefault();
                hassumm = false;
                hasothersumm = false;
            }
            if (summaryid > 0)
            {
                summ = summaries.Where(s => s.SummaryId == summaryid).FirstOrDefault();
            }
            searchkey = "";
            foreach (Key k in keys)
            {
                searchkey += k.KeyText + " ";
            }

            if (node.TreeLevel == 1)
            {
                hasparent = false;
            }

            if (children.Count() > 0)
            {
                haschild = true;
                noofchildren = children.Count();
            }

            if (pictures.Count() > 0)
            {
                haspict = true;
            }

            if (Display == "Details")
            {
                paragraphs.TheText = node.NodeText;
                paragraphs.NoOfChars = paragraphs.TheText.Length;

                showingdetails = true;
                showingsummary = false;

                node.Views++;
                _context.Update(node);
                await _context.SaveChangesAsync();
            }

            if (Display == "Summary")
            {
                paragraphs.TheText = summ.Summary1;
                paragraphs.NoOfChars = paragraphs.TheText.Length;

                showingsummary = true;
                showingdetails = false;

                summ.Views++;
                _context.Update(summ);
                await _context.SaveChangesAsync();
            }

            if (CurrentUser.NormalizedEmail == node.Owner)
            {
                owner = true;
            }

            paragraphs.Paragrphs(out ParagraphsNoOf, ref paragrphs, out SentencesNoOf, ref sentences, ref SentenceInParagraph, out LinesNoOf, ref lines, 0, false, true, true, false, true, false, true);
            paragraphs.ListsAndTables(ParagraphsNoOf, paragrphs, out newNoOfParagraphs, out newParagraphs, out hasnofigpara, out hasnotabpara);

            //ViewBag.Pictures = pictures;
            ViewBag.SelectedNodeHeading = node.Heading;

            if (id == null || _context.Nodes == null)
            {
                return NotFound();
            }

            if (node == null)
            {
                return NotFound();
            }

            BookIndexViewModel model = new BookIndexViewModel
            {
                Node = node,
                Summary = summ,
                OtherSummaries = othersummaries,
                ChapterSummSiblings = chaptersummsiblings,
                UserChaptSumms = userchaptsumms,
                HasOtherChapterSummSiblings = hasotherchaptersummsiblings,
                NoOfParagraphs = newNoOfParagraphs,
                Paragraphs = newParagraphs,
                Paragraph = "",
                DisplayPictures = true,
                HasPicture = haspict,
                NoOfPictures = pictures.Count(),
                PicturePointer = pictureptr,
                Pictures = pictures,
                PictureTitle = "",
                PictureFile = "",
                PictureFixed = picturefixed,
                HasSummary = hassumm,
                HasOtherSummaries = hasothersumm,
                HasChildren = haschild,
                NoOfChildren = noofchildren,
                HasParent = hasparent,
                HasNoFigPara = hasnofigpara,
                HasNoTabPara = hasnotabpara,
                ShowingDetails = showingdetails,
                ShowingSummary = showingsummary,
                SearchKey = searchkey,
                NoOfKeys = keys.Count(),
                Keys = keyvalues,
                Siblings = siblings,
                CurrentUser = CurrentUser.NormalizedEmail,
                Owner = owner
            };

            return View(model);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["TreeId"] = new SelectList(_context.Trees, "TreeId", "Heading");
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Label");
            return View();
        }

        // POST: Books/Create
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

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Nodes == null)
            {
                return NotFound();
            }

            var node = await _context.Nodes.FindAsync(id);

            if (node == null)
            {
                return NotFound();
            }
            ViewData["TreeId"] = new SelectList(_context.Trees, "TreeId", "Heading", node.TreeId);
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Label", node.TypeId);
            return View(node);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NodeId,TreeId,TypeId,ParentNodeId,TreeLevel,Heading,NodeText")] Node node)
        {
            if (id != node.NodeId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            try
            {
                _context.Update(node);
                await _context.SaveChangesAsync();
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
            return RedirectToAction(nameof(Details), new { id = node.NodeId });
            //}
            ViewData["TreeId"] = new SelectList(_context.Trees, "TreeId", "Heading", node.TreeId);
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Label", node.TypeId);
            return View(node);
        }

        // GET: Books/Delete/5
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

        // POST: Books/Delete/5
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
    }
}
