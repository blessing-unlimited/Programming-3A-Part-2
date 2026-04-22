using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMove.Models;
using TechMove.Models.ViewModel;
using TechMove.Services;

namespace TechMove.Controllers
{
    // Controller for managing Service Requests and currency-based costing
    public class ServiceRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IExchangeRateServices _exchangeRateServices;
        private readonly ICurrencyConverter _currencyConverter;

        // Services are 'injected' here so the controller can use them
        public ServiceRequestsController(ApplicationDbContext context,IExchangeRateServices exchangeRateServices,ICurrencyConverter currencyConverter)
        {
            _context = context;
            _exchangeRateServices = exchangeRateServices;
            _currencyConverter = currencyConverter;
        }

        // ServiceRequests - Displays a list of all service requests
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            //Fetch requests including related Contract and Client data for the table
            var items = await _context.ServiceRequests
                .AsNoTracking()
                .Include(sr => sr.Contract)
                .ThenInclude(c => c!.Client)
                .OrderByDescending(sr => sr.ServiceRequestId)
                .ToListAsync(cancellationToken);

            return View(items);
        }

        // ServiceRequests/Details/5 - Displays detailed info for a request
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .FirstOrDefaultAsync(sr => sr.ServiceRequestId == id);

            if (item == null) return NotFound();
            return View(item);
        }

        //ServiceRequests/Create - Prepares view with dynamic contract statuses
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateContractsDropDownAsync();
            return View(new ServiceRequestCreateViewModel());
        }

        // ServiceRequests/Create - Handles the creation of a new service request
        [HttpPost]
        public async Task<IActionResult> Create(ServiceRequestCreateViewModel viewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                await PopulateContractsDropDownAsync(viewModel.ContractId);
                return View(viewModel);
            }

            //Fetch the current live exchange rate from external API
            var rate = await _exchangeRateServices.GetUsdToZarRateAsync(cancellationToken);

            //Use our tested Service to calculate the ZAR cost with correct rounding
            var costZar = _currencyConverter.ConvertToZar(viewModel.CostUsd, rate);

            // Transfer data from the ViewModel to the Database Entity
            var entity = new ServiceRequest
            {
                ContractId = viewModel.ContractId,
                Description = viewModel.Description,
                CostUsd = viewModel.CostUsd,
                ExchangeRateUsdToZar = rate,
                CostZar = costZar,
                ExchangeRateFetchedAtUtc = DateTime.UtcNow,
                Status = viewModel.Status
            };

            _context.ServiceRequests.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            // Redirect to Index after successful creation
            return RedirectToAction(nameof(Index));
        }

        // ServiceRequests/Edit/5 - Prepares edit view
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest == null) return NotFound();

            await PopulateContractsDropDownAsync(serviceRequest.ContractId);

            return View(new ServiceRequestCreateViewModel
            {
                ServiceRequestId = serviceRequest.ServiceRequestId,
                ContractId = serviceRequest.ContractId,
                Description = serviceRequest.Description,
                CostUsd = serviceRequest.CostUsd,
                Status = serviceRequest.Status
            });
        }

        //  ServiceRequests/Edit/5 - Updates data and recalculates ZAR if USD changed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ServiceRequestCreateViewModel viewModel, CancellationToken cancellationToken)
        {
            if (id != viewModel.ServiceRequestId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var serviceRequest = await _context.ServiceRequests.FindAsync(id);
                    if (serviceRequest == null) return NotFound();

                    // Logic: If USD cost changed, fetch new rate and recalculate ZAR
                    if (serviceRequest.CostUsd != viewModel.CostUsd)
                    {
                        var rate = await _exchangeRateServices.GetUsdToZarRateAsync(cancellationToken);
                        serviceRequest.ExchangeRateUsdToZar = rate;
                        serviceRequest.CostZar = _currencyConverter.ConvertToZar(viewModel.CostUsd, rate);
                        serviceRequest.ExchangeRateFetchedAtUtc = DateTime.UtcNow;
                    }

                    serviceRequest.ContractId = viewModel.ContractId;
                    serviceRequest.Description = viewModel.Description;
                    serviceRequest.CostUsd = viewModel.CostUsd;
                    serviceRequest.Status = viewModel.Status;

                    _context.Update(serviceRequest);
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestExists(viewModel.ServiceRequestId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            await PopulateContractsDropDownAsync(viewModel.ContractId);
            return View(viewModel);
        }

        // ServiceRequests/Delete/5 - Confirmation view
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _context.ServiceRequests
                .Include(s => s.Contract)
                .FirstOrDefaultAsync(m => m.ServiceRequestId == id);
            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        //ServiceRequests/Delete/5 - Deletes request
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviceRequest = await _context.ServiceRequests.FindAsync(id);
            if (serviceRequest != null)
            {
                _context.ServiceRequests.Remove(serviceRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper: Fetches contracts and their statuses for UI dropdown
        private async Task PopulateContractsDropDownAsync(int? selectedId = null)
        {
            var contracts = await _context.Contracts
                .AsNoTracking()
                .OrderBy(c => c.ContractId)
                .Select(c => new
                {
                    c.ContractId,
                    c.Status,
                    Label = $"Contract #{c.ContractId}"
                })
                .ToListAsync();

            ViewBag.ContractId = new SelectList(contracts, "ContractId", "Label", selectedId);

            var statusMap = contracts.ToDictionary(c => c.ContractId.ToString(), c => c.Status);
            ViewBag.ContractStatusByIdJson = System.Text.Json.JsonSerializer.Serialize(statusMap);
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.ServiceRequests.Any(e => e.ServiceRequestId == id);
        }
    }
}