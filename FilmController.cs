using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using film.Models;
using System.Linq;
using System.IO;

namespace film.Controllers
{
    public class FilmController : Controller
    {
        private readonly FilmDbContext _context;
        private readonly string _filmlerKlasoru;

        public FilmController(FilmDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _filmlerKlasoru = Path.Combine(env.WebRootPath, "Filmler");
            KlasorleriOlustur();
        }

        private void KlasorleriOlustur()
        {
            // Ana Filmler klasörünü oluştur
            if (!Directory.Exists(_filmlerKlasoru))
            {
                Directory.CreateDirectory(_filmlerKlasoru);
            }

            // Her film türü için klasör oluştur
            var filmTurleri = _context.Filmler.Select(f => f.FilmTuru).Distinct();
            foreach (var tur in filmTurleri)
            {
                var turKlasoru = Path.Combine(_filmlerKlasoru, tur);
                if (!Directory.Exists(turKlasoru))
                {
                    Directory.CreateDirectory(turKlasoru);
                }
            }
        }

        // Film listesi
        public async Task<IActionResult> Index(string aramaMetni)
        {
            var filmler = from f in _context.Filmler
                         select f;

            if (!string.IsNullOrEmpty(aramaMetni))
            {
                filmler = filmler.Where(f => 
                    f.FilmAdi.Contains(aramaMetni) || 
                    f.Yonetmen.Contains(aramaMetni) || 
                    f.FilmTuru.Contains(aramaMetni));
                
                ViewData["AramaMetni"] = aramaMetni;
            }

            return View(await filmler.ToListAsync());
        }

        // Türe göre filmler
        public async Task<IActionResult> FilmTurleri()
        {
            var filmTurleri = await _context.Filmler
                .GroupBy(f => f.FilmTuru)
                .Select(g => new FilmTuruViewModel
                {
                    FilmTuru = g.Key,
                    Filmler = g.ToList()
                })
                .ToListAsync();

            return View(filmTurleri);
        }

        // Yönetmene göre filmler
        public async Task<IActionResult> Yonetmenler()
        {
            var yonetmenler = await _context.Filmler
                .GroupBy(f => f.Yonetmen)
                .Select(g => new YonetmenViewModel
                {
                    YonetmenAdi = g.Key,
                    Filmler = g.ToList()
                })
                .ToListAsync();

            return View(yonetmenler);
        }

        // Film detayları
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Filmler.FirstOrDefaultAsync(m => m.Id == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // Yeni film ekleme formu
        public IActionResult Create()
        {
            return View();
        }

        // Yeni film ekleme post işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FilmAdi,Yonetmen,CikisYili,FilmTuru,IMDBPuani,Izlendimi")] Film film)
        {
            if (ModelState.IsValid)
            {
                _context.Add(film);
                await _context.SaveChangesAsync();

                // Film türü için klasör oluştur
                var turKlasoru = Path.Combine(_filmlerKlasoru, film.FilmTuru);
                if (!Directory.Exists(turKlasoru))
                {
                    Directory.CreateDirectory(turKlasoru);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(film);
        }

        // Film düzenleme formu
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Filmler.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }
            return View(film);
        }

        // Film düzenleme post işlemi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FilmAdi,Yonetmen,CikisYili,FilmTuru,IMDBPuani,Izlendimi")] Film film)
        {
            if (id != film.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(film);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.Id))
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
            return View(film);
        }

        // Film silme
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Filmler
                .FirstOrDefaultAsync(m => m.Id == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // Film silme onay işlemi
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var film = await _context.Filmler.FindAsync(id);
            if (film != null)
            {
                _context.Filmler.Remove(film);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FilmExists(int id)
        {
            return _context.Filmler.Any(e => e.Id == id);
        }
    }
} 