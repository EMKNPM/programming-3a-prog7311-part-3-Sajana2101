using GLMS2.Enums;
using GLMS2.Interfaces;
using GLMS2.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GLMS2.Controllers
{
    public class ContractController : Controller
    {
        private readonly IContractApiService _contractApiService;
        private readonly IClientApiService _clientApiService;

        public ContractController(
            IContractApiService contractApiService,
            IClientApiService clientApiService)
        {
            _contractApiService = contractApiService;
            _clientApiService = clientApiService;
        }

        public async Task<IActionResult> Index(
            DateTime? startDateFrom,
            DateTime? startDateTo,
            ContractStatus? status)
        {
            try
            {
                var contracts = await _contractApiService.FilterContractsAsync(
                    startDateFrom,
                    startDateTo,
                    status);

                LoadStatusFilterDropdown(status);

                ViewBag.SelectedStartDateFrom = startDateFrom?.ToString("yyyy-MM-dd");
                ViewBag.SelectedStartDateTo = startDateTo?.ToString("yyyy-MM-dd");
                ViewBag.SelectedStatus = status?.ToString();

                return View(contracts);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                LoadStatusFilterDropdown(status);

                return View(new List<GLMS2.Models.Contract>());
            }
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContractCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            try
            {
                await _contractApiService.CreateContractAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadDropdowns();
                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var contract = await _contractApiService.GetContractByIdAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var contract = await _contractApiService.GetContractByIdAsync(id);

            if (contract == null)
            {
                return NotFound();
            }

            return View(contract);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _contractApiService.DeleteContractAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var file = await _contractApiService.DownloadAgreementAsync(id);

            if (file == null || file.Content.Length == 0)
            {
                return NotFound();
            }

            return File(file.Content, file.ContentType, file.FileName);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _contractApiService.GetContractForEditAsync(id);

            if (model == null)
            {
                return NotFound();
            }

            await LoadDropdowns();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContractEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns();
                return View(model);
            }

            try
            {
                var updated = await _contractApiService.UpdateContractAsync(model);

                if (!updated)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await LoadDropdowns();
                return View(model);
            }
        }

        private async Task LoadDropdowns()
        {
            var clients = await _clientApiService.GetAllClientsAsync();

            ViewBag.Clients = new SelectList(clients, "ClientId", "Name");

            ViewBag.StatusList = Enum.GetValues(typeof(ContractStatus))
                .Cast<ContractStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString()
                })
                .ToList();

            ViewBag.ContractTypes = Enum.GetValues(typeof(ContractType))
                .Cast<ContractType>()
                .Select(t => new SelectListItem
                {
                    Value = t.ToString(),
                    Text = t.ToString()
                })
                .ToList();
        }

        private void LoadStatusFilterDropdown(ContractStatus? selectedStatus)
        {
            ViewBag.StatusList = Enum.GetValues(typeof(ContractStatus))
                .Cast<ContractStatus>()
                .Select(s => new SelectListItem
                {
                    Value = s.ToString(),
                    Text = s.ToString(),
                    Selected = selectedStatus.HasValue && selectedStatus.Value == s
                })
                .ToList();
        }
    }
}