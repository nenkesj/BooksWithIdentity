using Books.Infrastructure;
using Books.Models;
using HowTo_DBLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Books.Controllers
{
    public class SummariesController : Controller
    {
        private readonly HowToDBContext _context;

        public UserManager<IdentityUser> UserManager { get; set; }

        public SummariesController(HowToDBContext context, UserManager<IdentityUser> userMgr)
        {
            _context = context;
            UserManager = userMgr;
        }

        // GET: Summaries
        public async Task<IActionResult> Index()
        {
            var howToDBContext = _context.Summaries.Include(s => s.Node);
            return View(await howToDBContext.ToListAsync());
        }

        // GET: Summaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Summaries == null)
            {
                return NotFound();
            }

            var summary = await _context.Summaries
                .Include(s => s.Node)
                .FirstOrDefaultAsync(m => m.SummaryId == id);
            if (summary == null)
            {
                return NotFound();
            }
            summary.Views++;
            _context.Update(summary);
            await _context.SaveChangesAsync();

            return View(summary);
        }

        // GET: Summaries/Create
        public IActionResult Create()
        {
            ViewData["NodeId"] = new SelectList(_context.Nodes, "NodeId", "Heading");
            return View();
        }

        // POST: Summaries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SummaryId,NodeId,Summary1")] Summary summary)
        {
            if (ModelState.IsValid)
            {
                _context.Add(summary);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NodeId"] = new SelectList(_context.Nodes, "NodeId", "Heading", summary.NodeId);
            return View(summary);
        }

        // GET: Summaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
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
            ViewData["NodeId"] = new SelectList(_context.Nodes, "NodeId", "Heading", summary.NodeId);
            return View(summary);
        }

        // POST: Summaries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SummaryId,NodeId,Summary1")] Summary summary)
        {
            if (id != summary.SummaryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(summary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SummaryExists(summary.SummaryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["NodeId"] = new SelectList(_context.Nodes, "NodeId", "Heading", summary.NodeId);
            return View(summary);
        }

        // GET: Summaries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Summaries == null)
            {
                return NotFound();
            }

            var summary = await _context.Summaries
                .Include(s => s.Node)
                .FirstOrDefaultAsync(m => m.SummaryId == id);
            if (summary == null)
            {
                return NotFound();
            }

            return View(summary);
        }

        // POST: Summaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Summaries == null)
            {
                return Problem("Entity set 'HowToDBContext.Summaries'  is null.");
            }
            var summary = await _context.Summaries.FindAsync(id);
            if (summary != null)
            {
                _context.Summaries.Remove(summary);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SummaryExists(int id)
        {
            return _context.Summaries.Any(e => e.SummaryId == id);
        }
        public void SpanSummaries(ref string Summaries, int NodeID, ref int ChapterSummNode, IdentityUser CurrentUser)
        {
            string Heading;
            int nodePtr, summariescount, nodescount;
            Node node = _context.Nodes.Single(n => n.NodeId == NodeID);
            Heading = node.Heading;
            if (Heading == "Chapter Summary" && node.Owner == CurrentUser.NormalizedEmail)
            {
                ChapterSummNode = node.NodeId;
            }
            else
            {
                Heading = Heading.ToUpper();
            }
            Heading += " " + (Char)13 + (Char)10;
            IEnumerable<Summary> summaries = _context.Summaries.Where(s => s.NodeId == node.NodeId && s.Owner == CurrentUser.NormalizedEmail).OrderBy(n => n.NodeId);
            summariescount = summaries.Count();
            if (summaries.Count() > 0)
            {
                if (summaries.First().Summary1.Substring(0, 7) == "Summary" || summaries.First().Summary1.Substring(0, 11) == "<h4>Summary")
                {
                    Summaries += Heading + summaries.First().Summary1.Substring((summaries.First().Summary1.IndexOf((Char)10) + 1));
                }
                else
                {
                    Summaries += Heading + summaries.First().Summary1;
                }
            }
            IEnumerable<Node> nodes = _context.Nodes.Where(n => n.ParentNodeId == node.NodeId).OrderBy(n => n.NodeId);
            nodescount = nodes.Count();
            if (nodes.Count() > 0)
            {
                for (int ptr = 0; ptr <= nodes.Count() - 1; ptr++)
                {
                    nodePtr = nodes.ToArray()[ptr].NodeId;
                    SpanSummaries(ref Summaries, nodePtr, ref ChapterSummNode, CurrentUser);
                }
            }
        }

        public async Task<IActionResult> Chapter(int id)
        {
            string Summaries;
            int ChapterSummNode, nodePtr;
            Summaries = "";
            ChapterSummNode = -1;
            nodePtr = id;
            IdentityUser CurrentUser = await UserManager.GetUserAsync(User);
            SpanSummaries(ref Summaries, nodePtr, ref ChapterSummNode, CurrentUser);
            if (ChapterSummNode != -1)
            {
                // Replace existing Chapter Summary with latest Summaries
                Node node = _context.Nodes.Single(n => n.NodeId == ChapterSummNode);
                node.NodeText = Summaries;
                _context.Update(node);
                await _context.SaveChangesAsync();
                ChapterSummNode = node.NodeId;
            }
            else
            {
                // Create the new node
                Node nde = _context.Nodes.Single(n => n.NodeId == id);
                Node node = new Node
                {
                    TreeId = 2,
                    TypeId = 2,
                    TreeLevel = (short)(nde.TreeLevel + 1),
                    ParentNodeId = nde.NodeId,
                    Heading = "Chapter Summary",
                    NodeText = Summaries,
                    Owner = CurrentUser.NormalizedEmail
                };
                _context.Add(node);
                await _context.SaveChangesAsync();
                ChapterSummNode = node.NodeId;
            }
            // Display Chapter Summary
            return RedirectToAction("Details", "Books", new { id = ChapterSummNode });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Summary(int id, bool picturefixed = false, int pictureptr = 0)
        {
            bool haspict = false;
            bool hassumm = false;
            bool hasothersumm = false;
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


            int LinesNoOf, SentencesNoOf, ParagraphsNoOf, noofkeys;
            List<string> lines, sentences, paragrphs, newParagraphs, keyvalues;
            Paragraphs paragraphs;
            Node node;
            Summary summ;
            List<int> SentenceInParagraph;
            List<Node> MatchingNodes;
            IEnumerable<Node> nodes;
            IEnumerable<Summary> summaries;
            IEnumerable<Summary> othersummaries;
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

            IdentityUser CurrentUser = await UserManager.GetUserAsync(User);
            string Email = CurrentUser?.Email ?? "(No Value)";
            string Phone = CurrentUser?.PhoneNumber ?? "(No Value)";

            summ = _context.Summaries.Where(s => s.SummaryId == id).FirstOrDefault();
            node = _context.Nodes.FirstOrDefault(n => n.NodeId == summ.NodeId);
            pictures = _context.Pictures.Where(pic => pic.NodeId == id);

            paragraphs.TheText = summ.Summary1;
            paragraphs.NoOfChars = paragraphs.TheText.Length;

            showingsummary = true;
            showingdetails = false;

            summ.Views++;
            _context.Update(summ);
            await _context.SaveChangesAsync();

            paragraphs.Paragrphs(out ParagraphsNoOf, ref paragrphs, out SentencesNoOf, ref sentences, ref SentenceInParagraph, out LinesNoOf, ref lines, 0, false, true, true, false, true, false, true);
            paragraphs.ListsAndTables(ParagraphsNoOf, paragrphs, out newNoOfParagraphs, out newParagraphs, out hasnofigpara, out hasnotabpara);

            if (CurrentUser.NormalizedEmail == node.Owner)
            {
                owner = true;
            }

            if (pictures.Count() > 0)
            {
                haspict = true;
            }

            SummaryIndexViewModel model = new SummaryIndexViewModel
            {
                Node = node,
                Summary = summ,
                Paragraphs = newParagraphs,
                NoOfParagraphs = newNoOfParagraphs,
                Paragraph = "",
                DisplayPictures = true,
                HasPicture = haspict,
                NoOfPictures = pictures.Count(),
                PicturePointer = pictureptr,
                Pictures = pictures,
                PictureTitle = "",
                PictureFile = "",
                PictureFixed = picturefixed,
                HasNoFigPara = hasnofigpara,
                HasNoTabPara = hasnotabpara,
                ShowingDetails = showingdetails,
                ShowingSummary = showingsummary,
                Owner = owner
            };

            return View(model);
        }
    }
}
