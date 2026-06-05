using GLMS2.Interfaces;
using GLMS2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS2.Controllers
{
    public class ServiceRequestController : Controller
    {
        private readonly IServiceRequestApiService _serviceRequestApiService;

        public ServiceRequestController(
            IServiceRequestApiService serviceRequestApiService)
        {
            _serviceRequestApiService = serviceRequestApiService;
        }

        public async Task<IActionResult> Index()
        {
            var serviceRequests =
                await _serviceRequestApiService.GetAllServiceRequestsAsync();

            return View(serviceRequests);
        }

        public async Task<IActionResult> Create()
        {
            await LoadContractsDropdown();

            var model = new ServiceRequestCreateViewModel();

            try
            {
                model.ExchangeRate =
                    await _serviceRequestApiService.GetUsdToZarRateAsync();
            }
            catch
            {
                model.ExchangeRate = 0;
                ViewBag.ApiError =
                    "Exchange rate could not be loaded. Please try again later.";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequestCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadContractsDropdown();
                await RecalculateCurrencyForCreate(model);
                return View(model);
            }

            try
            {
                var createdRequest =
                    await _serviceRequestApiService.CreateServiceRequestAsync(model);

                if (createdRequest == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                return RedirectToAction(
                    nameof(Details),
                    new { id = createdRequest.ServiceRequestId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                await LoadContractsDropdown();
                await RecalculateCurrencyForCreate(model);

                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var serviceRequest =
                await _serviceRequestApiService.GetServiceRequestByIdAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var serviceRequest =
                await _serviceRequestApiService.GetServiceRequestByIdAsync(id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            return View(serviceRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted =
                await _serviceRequestApiService.DeleteServiceRequestAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model =
                await _serviceRequestApiService.GetServiceRequestForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            await LoadContractsDropdown();

            try
            {
                model.ExchangeRate =
                    await _serviceRequestApiService.GetUsdToZarRateAsync();
            }
            catch
            {
                model.ExchangeRate = 0;
                ViewBag.ApiError = "Exchange rate could not be loaded.";
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiceRequestEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadContractsDropdown();
                await RecalculateCurrencyForEdit(model);
                return View(model);
            }

            try
            {
                var updated =
                    await _serviceRequestApiService.UpdateServiceRequestAsync(model);

                if (!updated)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                await LoadContractsDropdown();
                await RecalculateCurrencyForEdit(model);

                return View(model);
            }
        }

        private async Task LoadContractsDropdown()
        {
            var contracts =
                await _serviceRequestApiService.GetActiveContractsAsync();

            ViewBag.Contracts = new SelectList(
                contracts.Select(c => new
                {
                    c.ContractId,
                    DisplayText =
                        $"Contract #{c.ContractId} - {c.Client!.Name} ({c.Status})"
                }),
                "ContractId",
                "DisplayText");
        }

        private async Task RecalculateCurrencyForCreate(
            ServiceRequestCreateViewModel model)
        {
            try
            {
                model.ExchangeRate =
                    await _serviceRequestApiService.GetUsdToZarRateAsync();

                model.CostZAR = model.CostUSD > 0
                    ? Math.Round(model.CostUSD * model.ExchangeRate, 2)
                    : 0;
            }
            catch
            {
                ViewBag.ApiError = "Exchange rate could not be loaded.";
            }
        }

        private async Task RecalculateCurrencyForEdit(
            ServiceRequestEditViewModel model)
        {
            try
            {
                model.ExchangeRate =
                    await _serviceRequestApiService.GetUsdToZarRateAsync();

                model.CostZAR = model.CostUSD > 0
                    ? Math.Round(model.CostUSD * model.ExchangeRate, 2)
                    : 0;
            }
            catch
            {
                ViewBag.ApiError = "Exchange rate could not be loaded.";
            }
        }
    }
}