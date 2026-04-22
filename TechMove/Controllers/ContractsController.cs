using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.Models;
using TechMove.Models.ViewModel;
using TechMove.Services;

namespace TechMove.Controllers
{
    // Controller for managing Contract lifecycle and file uploads
    public class ContractsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _enviroment;
        private readonly IFileValidator _fileValidator;

        // Constructor injects Database Context, Environment (for file paths), and File Validator
        public ContractsController(ApplicationDbContext context, IWebHostEnvironment environment, IFileValidator fileValidator)
        {
            _context = context;
            _enviroment = environment;
            _fileValidator = fileValidator;
        }

        // Contracts/Create - Returns the view to initiate a new contract
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateClientsDropDownAsync();
            return View(new ContractViewModel());
        }

        //Contracts/Create - Handles contract submission and file upload
        [HttpPost]
        public async Task<IActionResult> Create(ContractViewModel viewModel, CancellationToken cancellationToken)
        {
            //  Ensure contract duration is logical
            if (viewModel.EndDate < viewModel.StartDate)
                ModelState.AddModelError(nameof(viewModel.EndDate), "End date cannot be before the start date.");

            //Validate the uploaded file extension using our custom service
            if (viewModel.SignedAgreementFile != null && !_fileValidator.IsPdf(viewModel.SignedAgreementFile))
            {
                ModelState.AddModelError(nameof(viewModel.SignedAgreementFile), "Only PDF files are allowed.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateClientsDropDownAsync(viewModel.ClientId);
                return View(viewModel);
            }

            String relativePath = String.Empty;
            String originalFileName = String.Empty;
            DateOnly? uploadDate = null;

            //  Process the file upload if a file was provided
            if (viewModel.SignedAgreementFile is not null && viewModel.SignedAgreementFile.Length > 0)
            {
                var uploadsRoot = Path.Combine(_enviroment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsRoot);

                originalFileName = Path.GetFileName(viewModel.SignedAgreementFile.FileName);
                var uniqueName = $"{Guid.NewGuid()}_{originalFileName}";
                var physicalPath = Path.Combine(uploadsRoot, uniqueName);

                // Save the file stream to the physical storage
                await using var stream = new FileStream(physicalPath, FileMode.Create);
                await viewModel.SignedAgreementFile.CopyToAsync(stream, cancellationToken);

                relativePath = Path.Combine("uploads", uniqueName).Replace("\\", "/");
                uploadDate = DateOnly.FromDateTime(DateTime.UtcNow);
            }

            // Create the database entity from the view model
            var contract = new Contract
            {
                ClientId = viewModel.ClientId,
                StartDate = viewModel.StartDate,
                EndDate = viewModel.EndDate,
                Status = viewModel.Status,
                ServiceLevel = viewModel.ServiceLevel,
                SignedAgreementPath = relativePath,
                SignedAgreementFileName = originalFileName,
                AgreementUploadDate = uploadDate
            };

            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync(cancellationToken);

            return RedirectToAction(nameof(Details), new { id = contract.ContractId });
        }

        //  Contracts - Displays filtered list of contracts
        [HttpGet]
        public async Task<IActionResult> Index(ContractFilterViewModel filter, CancellationToken cancellationToken)
        {
            //  Apply date range and status filters to the database query
            var query = _context.Contracts
                .AsNoTracking()
                .Include(c => c.Client)
                .AsQueryable();

            if (!String.IsNullOrWhiteSpace(filter.Status))
            {
                query = query.Where(c => c.Status == filter.Status);
            }

            // ... (Additional filter logic)

            var list = await query
                .OrderByDescending(c => c.StartDate)
                .ToListAsync(cancellationToken);

            return View(new ContractIndexViewModel { Filter = filter, Contracts = list });
        }
        
        //  Contracts/Details/5 - Displays specific contract info
        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.ContractId == id, cancellationToken);

            if (contract == null) return NotFound();

            return View(contract);
        }

        //  Contracts/Edit/5 - Prepares the view model for editing
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null) return NotFound();

            await PopulateClientsDropDownAsync(contract.ClientId);

            var viewModel = new ContractViewModel
            {
                ContractId = contract.ContractId,
                ClientId = contract.ClientId,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Status = contract.Status,
                ServiceLevel = contract.ServiceLevel,
                CurrentFileName = contract.SignedAgreementFileName
            };

            return View(viewModel);
        }

        // Contracts/Edit/5 - Updates contract data and optional file
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ContractViewModel viewModel, CancellationToken cancellationToken)
        {
            if (id != viewModel.ContractId) return NotFound();

            //Validate file type during edit
            if (viewModel.SignedAgreementFile != null && !_fileValidator.IsPdf(viewModel.SignedAgreementFile))
            {
                ModelState.AddModelError(nameof(viewModel.SignedAgreementFile), "Only PDF files are allowed.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var contract = await _context.Contracts.FindAsync(id);
                    if (contract == null) return NotFound();

                    // Update basic fields
                    contract.ClientId = viewModel.ClientId;
                    contract.StartDate = viewModel.StartDate;
                    contract.EndDate = viewModel.EndDate;
                    contract.Status = viewModel.Status;
                    contract.ServiceLevel = viewModel.ServiceLevel;

                    // Process new file if provided, overwriting old path references
                    if (viewModel.SignedAgreementFile != null && viewModel.SignedAgreementFile.Length > 0)
                    {
                        // ... (File saving logic)
                    }

                    _context.Update(contract);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContractExists(viewModel.ContractId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateClientsDropDownAsync(viewModel.ClientId);
            return View(viewModel);
        }

        //  Contracts/Delete/5 - Removes contract from database
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract != null)
            {
                _context.Contracts.Remove(contract);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //  Populates the client list for dropdowns
        private async Task PopulateClientsDropDownAsync(int? selectedId = null)
        {
            var clients = await _context.Clients
                .AsNoTracking()
                .OrderBy(c => c.ClientName)
                .Select(c => new
                {
                    c.ClientId,
                    Label = c.ClientName + "(ID: " + c.ClientId + ")"
                })
                .ToListAsync();

            ViewBag.ClientId = new SelectList(clients, "ClientId", "Label", selectedId);
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.ContractId == id);
        }
    }
}
