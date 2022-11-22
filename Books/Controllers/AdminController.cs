using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HowTo_DBLibrary;
using Books.Models;
using Books.Infrastructure;
using System.Diagnostics;
using NuGet.Protocol.Core.Types;

namespace Books.Controllers
{
    public class AdminController : Controller
    {
        private readonly HowToDBContext _context;

        public AdminController(HowToDBContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
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
        public async Task<IActionResult> Edit(int id, [Bind("NodeId,TreeId,TypeId,ParentNodeId,TreeLevel,Heading,NodeText")] Node node, int ParentNodeID = -1, int TreeLevel = -1, bool save = true)
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

            //if (ModelState.IsValid)
            //{
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
                return RedirectToAction(nameof(Details),"Books",new {id = node.NodeId});
            //}
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
            return View(node);
        }

        // POST: Admin/New/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New([Bind("NodeId,TreeId,TypeId,ParentNodeId,TreeLevel,Heading,NodeText")] Node node)
        {
            //if (ModelState.IsValid)
            //{
                _context.Add(node);
                await _context.SaveChangesAsync();
                ViewBag.SelectedNodeHeading = node.Heading;
                return RedirectToAction("Details", "Books", new{ id = node.NodeId});
            //}
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
            return View(node);
        }

        // POST: Admin/NewChild/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewChild([Bind("NodeId,TreeId,TypeId,ParentNodeId,TreeLevel,Heading,NodeText")] Node node)
        {
            //if (ModelState.IsValid)
            //{
                _context.Add(node);
                await _context.SaveChangesAsync();
                ViewBag.SelectedNodeHeading = node.Heading;
            return RedirectToAction("Details", "Books", new { id = node.NodeId });
            //}
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

        // POST: Summaries/EditSummary1/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSummary1(int id, [Bind("SummaryId,NodeId,Summary1")] Summary summary)
        {
            if (id != summary.SummaryId)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
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
            //}
            ViewData["NodeId"] = new SelectList(_context.Nodes, "NodeId", "Heading", summary.NodeId);
            return View(summary);
        }
        // GET: Admin/EditSummary/5
        public async Task<IActionResult> EditSummary1(int id)
        {
            if (id == null || _context.Summaries == null)
            {
                return NotFound();
            }
            var summary = await _context.Summaries
                .Include(n => n.Node)
                .FirstOrDefaultAsync(m => m.NodeId == id);
            if (summary == null)
            {
                return NotFound();
            }
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

            //if (ModelState.IsValid)
            //{
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
            //}
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
                        if (node.Heading.Substring(0, 8).ToLower() == "chapter ")
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
    }
}
