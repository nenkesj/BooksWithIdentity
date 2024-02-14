using HowTo_DBLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Books.Controllers
{
    public class KeysController : Controller
    {
        private readonly HowToDBContext _context;

        public KeysController(HowToDBContext context)
        {
            _context = context;
        }

        // GET: Keys
        public async Task<IActionResult> Index(string calledfrom = "NotIndex", string Distinct = "All", string Category = "All", string StartsWith = "All")
        {
            if (HttpContext.Session.GetString("Distinct") == null)
            {
                HttpContext.Session.SetString("Distinct", Distinct);
                await HttpContext.Session.CommitAsync();
            }
            else
            {
                if (calledfrom == "Index" && Distinct != null)
                {
                    HttpContext.Session.SetString("Distinct", Distinct);
                }
            }
            Distinct = HttpContext.Session.GetString("Distinct");
            ViewBag.Distinct = Distinct;
            if (HttpContext.Session.GetString("Category") == null)
            {
                HttpContext.Session.SetString("Category", Category);
                await HttpContext.Session.CommitAsync();
            }
            else
            {
                if (calledfrom == "Index" && Category != null)
                {
                    HttpContext.Session.SetString("Category", Category);
                }
            }
            Category = HttpContext.Session.GetString("Category");
            ViewBag.Category = Category;
            if (HttpContext.Session.GetString("StartsWith") == null)
            {
                HttpContext.Session.SetString("StartsWith", StartsWith);
                await HttpContext.Session.CommitAsync();
            }
            else
            {
                if (calledfrom == "Index" && StartsWith != null)
                {
                    HttpContext.Session.SetString("StartsWith", StartsWith);
                }
            }
            StartsWith = HttpContext.Session.GetString("StartsWith");
            ViewBag.StartsWith = StartsWith;
            List<Key> keysDistinct = new List<Key>();
            List<Key> keysAll = new List<Key>();
            List<string> KeyText = new List<string>();
            ViewBag.Distinct = Distinct;
            var keys = _context.Keys.OrderBy(k => k.KeyText);
            if (StartsWith != "All" && StartsWith.Length > 0)
            {
                foreach (Key k in keys)
                {
                    if (Distinct == "Distinct")
                    {
                        if (!KeyText.Contains(k.KeyText))
                        {
                            KeyText.Add(k.KeyText);
                            if (k.KeyText.Length >= StartsWith.Length)
                            {
                                if (StartsWith.ToLower() == k.KeyText.Substring(0, StartsWith.Length).ToLower())
                                {
                                    if (Category == "All")
                                    {
                                        keysDistinct.Add(k);
                                    }
                                    else
                                    {
                                        if (Category == k.Category)
                                        {
                                            keysDistinct.Add(k);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        KeyText.Add(k.KeyText);
                        if (k.KeyText.Length >= StartsWith.Length)
                        {
                            if (StartsWith.ToLower() == k.KeyText.Substring(0, StartsWith.Length).ToLower())
                            {
                                if (Category == "All")
                                {
                                    keysAll.Add(k);
                                }
                                else
                                {
                                    if (Category == k.Category)
                                    {
                                        keysAll.Add(k);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Key k in keys)
                {
                    if (Distinct == "Distinct")
                    {
                        if (!KeyText.Contains(k.KeyText))
                        {
                            KeyText.Add(k.KeyText);
                            if (Category == "All")
                            {
                                keysDistinct.Add(k);
                            }
                            else
                            {
                                if (Category == k.Category)
                                {
                                    keysDistinct.Add(k);
                                }
                            }
                        }
                    }
                    else
                    {
                        KeyText.Add(k.KeyText);
                        if (Category == "All")
                        {
                            keysAll.Add(k);
                        }
                        else
                        {
                            if (Category == k.Category)
                            {
                                keysAll.Add(k);
                            }
                        }
                    }
                }
            }

            if (Distinct == "Distinct")
            {
                return View(keysDistinct);
            }
            else
            {
                return View(keysAll);
            }
        }

        // GET: Keys/Details/5
        public async Task<IActionResult> Details(int? id, string? keyText, string Distinct = "All", string Category = "All")
        {
            ViewBag.KeyText = keyText;
            ViewBag.Distinct = Distinct;
            ViewBag.Category = Category;
            Node n;
            IQueryable<Key> keys;
            if (keyText == null || _context.Keys == null)
            {
                return NotFound();
            }

            if (Distinct == "Distinct")
            {
                if (Category != "All")
                {
                    keys = _context.Keys.Where(k => k.KeyText == keyText && k.Category == Category);
                }
                else
                {
                    keys = _context.Keys.Where(k => k.KeyText == keyText);
                }
            }
            else
            {
                keys = _context.Keys.Where(k => k.KeyId == id);
            }
            if (keys == null)
            {
                return NotFound();
            }

            if (Distinct == "Distinct")
            {
                List<Node> nodes = new List<Node>();
                foreach (Key key in keys)
                {
                    n = _context.Nodes.Where(n => n.NodeId == key.NodeId).FirstOrDefault();
                    nodes.Add(n);
                }
                return View(nodes);
            }
            else
            {
                Key k = keys.FirstOrDefault();
                n = _context.Nodes.Where(n => n.NodeId == k.NodeId).FirstOrDefault();
                return RedirectToAction(nameof(BooksController.Details), nameof(Books), new { id = n.NodeId, keyText = k.KeyText, Distinct = Distinct, Category = Category });
            }
        }

        // GET: Keys/Create
        public IActionResult Create()
        {
            //ViewData["NodeId"] = new SelectList(_context.Nodes, "NodeId", "Heading");
            return View();
        }

        // POST: Keys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("KeyId,NodeId,Key")] Key key)
        {
            if (ModelState.IsValid)
            {
                _context.Add(key);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["NodeId"] = new SelectList(_context.Nodes, "NodeId", "Heading", Key.NodeId);
            return View(key);
        }

        // GET: Keys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Keys == null)
            {
                return NotFound();
            }
            TempData["message"] = null;
            var key = await _context.Keys.FindAsync(id);
            if (key == null)
            {
                return NotFound();
            }
            //ViewData["NodeId"] = new SelectList(_context.Nodes, "NodeId", "Heading", Key.NodeId);
            return View(key);
        }

        // POST: Keys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("KeyId,NodeId,KeyText,Category")] Key key)
        {
            if (id != key.KeyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    key.TreeId = 2;
                    key.TypeId = 7;
                    _context.Update(key);
                    await _context.SaveChangesAsync();
                    TempData["message"] = string.Format("Key: {0}, {1} ... has been edited", key.KeyId, key.KeyText);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KeyExists(key.KeyId))
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
            //ViewData["NodeId"] = new SelectList(_context.Nodes, "NodeId", "Heading", Key.NodeId);
            return View(key);
        }

        // GET: Keys/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Keys == null)
            {
                return NotFound();
            }
            TempData["message"] = null;
            var key = await _context.Keys
                .FirstOrDefaultAsync(m => m.KeyId == id);
            if (key == null)
            {
                return NotFound();
            }

            return View(key);
        }

        // POST: Keys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Keys == null)
            {
                return Problem("Entity set 'HowToDBContext.Keys'  is null.");
            }
            var key = await _context.Keys.FindAsync(id);
            if (key != null)
            {
                TempData["message"] = string.Format("Key: {0}, {1} ... has been deleted", key.KeyId, key.KeyText);
                _context.Keys.Remove(key);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool KeyExists(int id)
        {
            return _context.Keys.Any(e => e.KeyId == id);
        }
    }
}
