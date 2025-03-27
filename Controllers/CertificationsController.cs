using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment2;
using Assignment2.Models;

namespace Assignment2.Controllers
{
    public class CertificationsController : Controller
    {
        private readonly Context _context;

        public CertificationsController(Context context)
        {
            _context = context;
        }

        // GET: Certifications
        public async Task<IActionResult> Index()
        {
            return View(await _context.Certifications.ToListAsync());
        }

        // GET: Certifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certification = await _context.Certifications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (certification == null)
            {
                return NotFound();
            }

            return View(certification);
        }

        // GET: Certifications/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Certifications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Certification_Description,Certification_Authority,Certification_Number")] Certification certification)
        {

            //Console.WriteLine(ModelState.ErrorCount.ToString());
            if (ModelState.IsValid) // model state not valid
            {
                

                _context.Add(certification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(certification);
        }

        // GET: Certifications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certification = await _context.Certifications.FindAsync(id);
            if (certification == null)
            {
                return NotFound();
            }
            return View(certification);
        }

        // POST: Certifications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Certification_Description,Certification_Authority,Certification_Number")] Certification certification)
        {
            if (id != certification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(certification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CertificationExists(certification.Id))
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
            return View(certification);
        }

        // GET: Certifications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certification = await _context.Certifications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (certification == null)
            {
                return NotFound();
            }

            return View(certification);
        }

        // POST: Certifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var certification = await _context.Certifications.FindAsync(id);
            if (certification != null)
            {
                _context.Certifications.Remove(certification);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CertificationExists(int id)
        {
            return _context.Certifications.Any(e => e.Id == id);
        }
    }
}
