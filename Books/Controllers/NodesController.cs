using HowTo_DBLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Books.Controllers
{
    public class NodesController : Controller
    {
        private readonly HowToDBContext _context;

        public NodesController(HowToDBContext context)
        {
            _context = context;
        }

        // GET: Nodes
        public async Task<IActionResult> Index()
        {
            var howToDBContext = _context.Nodes.Include(n => n.Tree).Include(n => n.Type);
            return View(await howToDBContext.ToListAsync());
        }

        // GET: Nodes/Details/5
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

        // GET: Nodes/Create
        public IActionResult Create()
        {
            ViewData["TreeId"] = new SelectList(_context.Trees, "TreeId", "Heading");
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Category");
            return View();
        }

        // POST: Nodes/Create
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
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Category", node.TypeId);
            return View(node);
        }

        // GET: Nodes/Edit/5
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
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Category", node.TypeId);
            return View(node);
        }

        // POST: Nodes/Edit/5
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

            if (ModelState.IsValid)
            {
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["TreeId"] = new SelectList(_context.Trees, "TreeId", "Heading", node.TreeId);
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Category", node.TypeId);
            return View(node);
        }

        // GET: Nodes/Delete/5
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

        // POST: Nodes/Delete/5
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
