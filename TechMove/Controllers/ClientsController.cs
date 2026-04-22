using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMove.Models;

namespace TechMove.Controllers
{
    // Controller for managing Client-related operations
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor injects the Database Context
        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Clients - Displays a list of all clients
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Fetch all clients from the database and return to the Index view
            return View(await _context.Clients.ToListAsync());
        }

        // Clients/Details/5 - Displays detailed information for a specific client
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            //  Find the client by ID in the database
            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null) return NotFound();

            return View(client);
        }

        //Clients/Create - Returns the view to create a new client
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        //Clients/Create - Handles the submission of a new client
        [HttpPost]
        public async Task<IActionResult> Create([Bind("ClientId,ClientName,ContactDetails,Region")] Client client)
        {
            // Ensure the submitted data matches the model rules
            if (ModelState.IsValid)
            {
                // Add the new client to the context and save changes
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        //Clients/Edit/5 - Returns the view to edit an existing client
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Fetch the client to be edited
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();
            return View(client);
        }

        //  Clients/Edit/5 - Handles the update of an existing client
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("ClientId,ClientName,ContactDetails,Region")] Client client)
        {
            if (id != client.ClientId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Mark the client as modified and save to database
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Check if the client still exists if a concurrency error occurs
                    if (!ClientExists(client.ClientId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Delete/5 - Returns the confirmation view for deleting a client
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            // Fetch the client to be deleted
            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null) return NotFound();

            return View(client);
        }

        // POST: Clients/Delete/5 - Handles the final deletion of a client
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Find and remove the client from the database
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Checks if a client exists by ID
        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.ClientId == id);
        }
    }
}
