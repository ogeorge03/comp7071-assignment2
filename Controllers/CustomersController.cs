using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment2;
using Assignment2.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.FileProviders;

namespace Assignment2.Controllers
{
    public class CustomersController : Controller
    {
        private readonly Context _context;

        public CustomersController(Context context)
        {
            _context = context;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var customers = await _context.CustomersDetails.FromSqlRaw(@"
                            select
                            p.Id as [Id]
                            ,p.First_Name as [FirstName]
                            ,p.Middle_Name as [MiddleName]
                            ,p.Last_Name as [LastName]
                            ,p.Date_Of_Birth as [Date_Of_Birth]
                            ,c.Customer_Notes
                            ,c.Payment_Information
                            from Customer c
                            inner join Person p on p.Id = c.Id
            ").AsNoTracking().ToListAsync();
            return View(customers);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Customer_Notes,Payment_Information,First_Name,Middle_Name,Last_Name,Date_Of_Birth")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Customer_Notes,Payment_Information,First_Name,Middle_Name,Last_Name,Date_Of_Birth")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
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
            return View(customer);
        
        }

        public async Task<IActionResult> RegisterService(int? id) {

            var customerServices = await _context.CustomerServiceDetails.FromSqlRaw(@"
                            select
                            cs.Id,
                            cs.Description
                            ,cs.Hourly_Rate
                            ,cs.Certification_RequiredId as [Certification_Required]
                            ,cert.Id as [Certification_Id]
                            ,cert.Certification_Description
                            ,cert.Certification_Authority
                            ,cert.Certification_Number
                            from Customer_Service cs 
                            inner join Certification cert on cert.Id = cs.Certification_RequiredId
            ").AsNoTracking().ToListAsync();
            ViewData["CustomerId"] = id;
            return View(customerServices);
        }

        public IActionResult ScheduleService(int? id, int? CustomerId, DateTime ScheduledDate) {
            if (ScheduledDate == default) {
                return BadRequest("Scheduled date is required");
            }

            string sql = "insert into Customer_Service_Scheduled (CustomerId, Customer_ServiceId, Scheduled_DateTime) values ({0}, {1}, {2})";
            _context.Database.ExecuteSqlRaw(sql, CustomerId, id, ScheduledDate);
            
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ViewServices(int? id) {

            if (id == null) {
                return NotFound();
            }

            var services = await _context.CustomerServiceScheduleDetails.FromSqlRaw(@"

                                select 
                                schedule.Id as [Id]
                                ,service.Description as [Description]
                                ,service.Hourly_Rate as [HourlyRate]
                                ,cert.Certification_Authority as [CertificationAuthority]
                                ,cert.Certification_Description as [CertificationDescription]
                                ,schedule.Scheduled_DateTime as [ScheduledDateTime]
                                from Customer_Service_Scheduled schedule
                                left join Customer_Service service on service.Id = schedule.Customer_ServiceId
                                left join Facility f on f.Id = schedule.FacilityId
                                left join Certification cert on cert.Id = service.Certification_RequiredId
                                where schedule.CustomerId = {0}
                                order by schedule.Scheduled_DateTime asc
            ", id).AsNoTracking().ToListAsync();

            return View(services);
        }


        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
