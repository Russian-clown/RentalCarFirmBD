using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarRental.Models;
using ClosedXML.Excel;
using System.IO;

namespace CarRental.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Orders
                .Include(o => o.Customer);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Trucks)
                .Include(o => o.Trailers)
                .Include("Trucks.Brand")
                .Include("Trucks.Color")
                .Include("Trailers.Color")
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "LName");

            var rentedTrucks = _context.Orders.Where(o => o.ExpirationDate > DateTime.Now.Date).SelectMany(o => o.Trucks);
            var rentedTrailers = _context.Orders.Where(o => o.ExpirationDate > DateTime.Now.Date).SelectMany(o => o.Trailers);
            ViewData["Trucks"] = _context.Trucks.Where(t => !rentedTrucks.Contains(t))
                .Include(t => t.Brand)
                //.Include(t => t.Colors)
                .ToList();
            ViewData["Trailers"] = _context.Trailers.Where(t => !rentedTrailers.Contains(t))
                //.Include(t => t.Colors)
                .ToList();

            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RegistrationDate,ExpirationDate,CustomerId")] Order order, int[] trucks, int[] trailers)
        {
            if (ModelState.IsValid && (trucks.Length > 0 || trailers.Length > 0))
            {
                if (trucks != null)
                {
                    foreach (var t in _context.Trucks.Where(t => trucks.Contains(t.Id)))
                    {
                        order.Trucks.Add(t);
                    }
                }

                if (trailers != null)
                {
                    foreach (var t in _context.Trailers.Where(t => trailers.Contains(t.Id)))
                    {
                        order.Trailers.Add(t);
                    }
                }
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "LName", order.CustomerId);
            var rentedTrucks = _context.Orders.Where(o => o.ExpirationDate > DateTime.Now.Date).SelectMany(o => o.Trucks).ToList();
            var rentedTrailers = _context.Orders.Where(o => o.ExpirationDate > DateTime.Now.Date).SelectMany(o => o.Trailers).ToList();
            ViewData["Trucks"] = _context.Trucks.Where(t => !rentedTrucks.Contains(t))
                .Include(t => t.Brand)
                //.Include(t => t.Color)
                .ToList();
            ViewData["Trailers"] = _context.Trailers.Where(t => !rentedTrailers.Contains(t)).ToList();
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Trucks)
                .Include(o => o.Trailers)
                .Include("Trucks.Brand")
                //.Include("Trucks.Color")
                //.Include("Trailers.Color")
                .FirstOrDefaultAsync(o => o.Id == id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "LName", order.CustomerId);
            var rentedTrucks = _context.Orders.Where(o => o.ExpirationDate < DateTime.Now.Date).SelectMany(o => o.Trucks).ToList();
            var rentedTrailers = _context.Orders.Where(o => o.ExpirationDate < DateTime.Now.Date).SelectMany(o => o.Trailers).ToList();
            ViewData["Trucks"] = _context.Trucks.Where(t => !rentedTrucks.Contains(t))
                .Include(t => t.Brand)
                //.Include(t => t.Color)
                .ToList();
            ViewData["Trailers"] = _context.Trailers.Where(t => !rentedTrailers.Contains(t)).ToList();
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,RegistrationDate,ExpirationDate,CustomerId")] Order order, int[] trucks, int[] trailers)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid && (trucks.Length > 0 || trailers.Length > 0))
            {
                try
                {
                    Order edited = _context.Orders
                        .Include(o => o.Trucks)
                        .Include(o => o.Trailers)
                        .FirstOrDefault(o => o.Id == id);
                    edited.Trucks.Clear();
                    edited.Trailers.Clear();

                    edited.RegistrationDate = order.RegistrationDate;
                    edited.ExpirationDate = order.ExpirationDate;
                    edited.CustomerId = order.CustomerId;

                    if (trucks != null)
                    {
                        foreach (var t in _context.Trucks.Where(t => trucks.Contains(t.Id)))
                        {
                            edited.Trucks.Add(t);
                        }
                    }

                    if (trailers != null)
                    {
                        foreach (var t in _context.Trailers.Where(t => trailers.Contains(t.Id)))
                        {
                            edited.Trailers.Add(t);
                        }
                    }
                    _context.Update(edited);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "LName", order.CustomerId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Download()
        {
            using(var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Список заказов");
                var currentRow = 1;

                ws.Cell(currentRow, 1).Value = "Клиент";
                ws.Cell(currentRow, 2).Value = "Арендованные машины и прицепы";
                ws.Cell(currentRow, 3).Value = "Дата начала аренды";
                ws.Cell(currentRow, 4).Value = "Дата окончания аренды";
                ws.Cell(currentRow, 5).Value = "Стоимость аренды";

                var orders = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.Trucks)
                    .Include(o => o.Trailers)
                    .Include("Trucks.CarBody")
                    .ToList();

                int total = 0;

                foreach (var order in orders)
                {
                    currentRow++;

                    string items = string.Join(", ", order.Trucks.Select(t => t.CarBody.CarBodyNum)) + ", " +
                        string.Join(", ", order.Trailers.Select(t => t.LicensePlate));

                    ws.Cell(currentRow, 1).Value = order.Customer.LName;
                    ws.Cell(currentRow, 2).Value = items;
                    ws.Cell(currentRow, 3).Value = order.RegistrationDate;
                    ws.Cell(currentRow, 4).Value = order.ExpirationDate;

                    int days = (int)order.ExpirationDate.Subtract(order.RegistrationDate).TotalDays;

                    int sum = 0;

                    foreach (Truck t in order.Trucks)
                        sum += days * t.Price;

                    foreach (Trailer t in order.Trailers)
                        sum += days * t.Price;

                    ws.Cell(currentRow, 5).Value = sum;

                    total += sum;
                }
                currentRow++;
                ws.Cell(currentRow, 1).Value = "ИТОГО: ";
                ws.Cell(currentRow, 5).Value = total;

                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var file = stream.ToArray();

                    return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Список заказов.xlsx");
                }
            }
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
