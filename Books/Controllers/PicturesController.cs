using HowTo_DBLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Books.Controllers
{
    public class PicturesController : Controller
    {
        private readonly HowToDBContext _context;

        public PicturesController(HowToDBContext context)
        {
            _context = context;
        }

        // GET: Pictures
        public async Task<IActionResult> Index()
        {
            var howToDBContext = _context.Pictures.Include(p => p.Node).Include(p => p.Type);
            return View(await howToDBContext.ToListAsync());
        }

        // GET: Pictures/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pictures == null)
            {
                return NotFound();
            }

            var picture = await _context.Pictures
                .Include(p => p.Node)
                .Include(p => p.Type)
                .FirstOrDefaultAsync(m => m.PictureId == id);
            if (picture == null)
            {
                return NotFound();
            }

            return View(picture);
        }

        // GET: Pictures/Create
        public IActionResult Create()
        {
            ViewData["NodeId"] = new SelectList(_context.Nodes, "NodeId", "Heading");
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Category");
            return View();
        }

        // POST: Pictures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PictureId,NodeId,TypeId,Picture1,Title,PictureSize,DisplayAt,DisplayStopAt,InfoId")] Picture picture)
        {
            if (ModelState.IsValid)
            {
                _context.Add(picture);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NodeId"] = new SelectList(_context.Nodes, "NodeId", "Heading", picture.NodeId);
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Category", picture.TypeId);
            return View(picture);
        }

        // GET: Pictures/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pictures == null)
            {
                return NotFound();
            }

            var picture = await _context.Pictures.FindAsync(id);
            if (picture == null)
            {
                return NotFound();
            }
            ViewData["NodeId"] = new SelectList(_context.Nodes, "NodeId", "Heading", picture.NodeId);
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Category", picture.TypeId);
            return View(picture);
        }

        // POST: Pictures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PictureId,NodeId,TypeId,Picture1,Title,PictureSize,DisplayAt,DisplayStopAt,InfoId")] Picture picture)
        {
            if (id != picture.PictureId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(picture);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PictureExists(picture.PictureId))
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
            ViewData["NodeId"] = new SelectList(_context.Nodes, "NodeId", "Heading", picture.NodeId);
            ViewData["TypeId"] = new SelectList(_context.Types, "TypeId", "Category", picture.TypeId);
            return View(picture);
        }

        // GET: Pictures/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pictures == null)
            {
                return NotFound();
            }

            var picture = await _context.Pictures
                .Include(p => p.Node)
                .Include(p => p.Type)
                .FirstOrDefaultAsync(m => m.PictureId == id);
            if (picture == null)
            {
                return NotFound();
            }

            return View(picture);
        }

        // POST: Pictures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pictures == null)
            {
                return Problem("Entity set 'HowToDBContext.Pictures'  is null.");
            }
            var picture = await _context.Pictures.FindAsync(id);
            if (picture != null)
            {
                _context.Pictures.Remove(picture);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PictureExists(int id)
        {
            return _context.Pictures.Any(e => e.PictureId == id);
        }
    }
}
