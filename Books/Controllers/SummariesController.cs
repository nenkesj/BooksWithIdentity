using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HowTo_DBLibrary;
using NuGet.Protocol.Core.Types;

namespace Books.Controllers
{
    public class SummariesController : Controller
    {
        private readonly HowToDBContext _context;

        public SummariesController(HowToDBContext context)
        {
            _context = context;
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
        public void SpanSummaries(ref string Summaries, int NodeID, ref int ChapterSummNode)
        {
            string Heading;
            int nodePtr, summariescount, nodescount;
            Node node = _context.Nodes.Single(n => n.NodeId == NodeID);
            Heading = node.Heading;
            if (Heading == "Chapter Summary")
            {
                ChapterSummNode = node.NodeId;
            }
            else
            {
                Heading = Heading.ToUpper();
            }
            Heading += " " + (Char)13 + (Char)10;
            IEnumerable<Summary> summaries = _context.Summaries.Where(s => s.NodeId == node.NodeId);
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
            IEnumerable<Node> nodes = _context.Nodes.Where(n => n.ParentNodeId == node.NodeId);
            nodescount = nodes.Count();
            if (nodes.Count() > 0)
            {
                for (int ptr = 0; ptr <= nodes.Count() - 1; ptr++)
                {
                    nodePtr = nodes.ToArray()[ptr].NodeId;
                    SpanSummaries(ref Summaries, nodePtr, ref ChapterSummNode);
                }
            }
        }

        public ActionResult Chapter(int id)
        {
            string Summaries;
            int ChapterSummNode, nodePtr;
            Summaries = "";
            ChapterSummNode = -1;
            nodePtr = id;
            SpanSummaries(ref Summaries, nodePtr, ref ChapterSummNode);
            if (ChapterSummNode != -1)
            {
                // Replace existing Chapter Summary with latest Summaries
                Node node = _context.Nodes.Single(n => n.NodeId == id);
                node.Heading = "Chapter Summary";
                node.NodeText = Summaries;
                _context.Update(node);
                _context.SaveChangesAsync();
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
                    NodeText = Summaries
                };
                _context.Add(node);
                _context.SaveChangesAsync();
                ChapterSummNode = node.NodeId;
            }
            // Display Chapter Summary
            return RedirectToAction("Details", "Books", new { id = ChapterSummNode });
        }
    }
}
