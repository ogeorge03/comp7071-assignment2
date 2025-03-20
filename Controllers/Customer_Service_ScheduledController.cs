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
    public class Customer_Service_ScheduledController : Controller
    {
        private readonly Context _context;

        public Customer_Service_ScheduledController(Context context)
        {
            _context = context;
        }

        // GET: Customer_Service_Scheduled
        public async Task<IActionResult> Index()
        {
            return View(await _context.Customer_Service_Scheduleds.ToListAsync());
        }

        // GET: Customer_Service_Scheduled/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer_Service_Scheduled = await _context.Customer_Service_Scheduleds
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer_Service_Scheduled == null)
            {
                return NotFound();
            }

            return View(customer_Service_Scheduled);
        }

        // GET: Customer_Service_Scheduled/Create
        public IActionResult Create()
        {
            ViewBag.CustomerId = new SelectList(_context.Customers, "Id", "First_Name" );

            ViewBag.Customer_ServiceId = new SelectList(_context.Customer_Services, "Id", "Street_Name");

            return View();
        }

        // POST: Customer_Service_Scheduled/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Scheduled_DateTime,Rendered_DateTime,Service_Fee,Customer_Satisfaction_Rating,Comments,CustomerId,Customer_ServiceId")] Customer_Service_Scheduled customer_Service_Scheduled)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer_Service_Scheduled);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.CustomerId = new SelectList(_context.Customers, "Id", "First_Name", customer_Service_Scheduled.Customer);

            ViewBag.Customer_ServiceId = new SelectList(_context.Customer_Services, "Id", "Street_Name", customer_Service_Scheduled.Customer_Service);

            return View(customer_Service_Scheduled);
        }

        // GET: Customer_Service_Scheduled/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer_Service_Scheduled = await _context.Customer_Service_Scheduleds.FindAsync(id);
            if (customer_Service_Scheduled == null)
            {
                return NotFound();
            }

            ViewBag.CustomerId = new SelectList(_context.Customers, "Id", "First_Name", customer_Service_Scheduled.Customer);

            ViewBag.Customer_ServiceId = new SelectList(_context.Customer_Services, "Id", "Street_Name", customer_Service_Scheduled.Customer_Service);


            return View(customer_Service_Scheduled);
        }

        // POST: Customer_Service_Scheduled/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Scheduled_DateTime,Rendered_DateTime,Service_Fee,Customer_Satisfaction_Rating,Comments,CustomerId,Customer_ServiceId")] Customer_Service_Scheduled customer_Service_Scheduled)
        {
            if (id != customer_Service_Scheduled.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer_Service_Scheduled);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Customer_Service_ScheduledExists(customer_Service_Scheduled.Id))
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

            ViewBag.CustomerId = new SelectList(_context.Customers, "Id", "First_Name", customer_Service_Scheduled.Customer);

            ViewBag.Customer_ServiceId = new SelectList(_context.Customer_Services, "Id", "Street_Name", customer_Service_Scheduled.Customer_Service);


            return View(customer_Service_Scheduled);
        }

        // GET: Customer_Service_Scheduled/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer_Service_Scheduled = await _context.Customer_Service_Scheduleds
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer_Service_Scheduled == null)
            {
                return NotFound();
            }

            return View(customer_Service_Scheduled);
        }

        // POST: Customer_Service_Scheduled/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer_Service_Scheduled = await _context.Customer_Service_Scheduleds.FindAsync(id);
            if (customer_Service_Scheduled != null)
            {
                _context.Customer_Service_Scheduleds.Remove(customer_Service_Scheduled);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Customer_Service_ScheduledExists(int id)
        {
            return _context.Customer_Service_Scheduleds.Any(e => e.Id == id);
        }
    }
}
