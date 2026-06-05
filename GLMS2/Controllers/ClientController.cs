using GLMS2.Interfaces;
using GLMS2.Models;
using Microsoft.AspNetCore.Mvc;
using GLMS2.ViewModels;

namespace GLMS2.Controllers
{
    public class ClientController : Controller
    {
        private readonly IClientApiService _clientApiService;

        public ClientController(IClientApiService clientApiService)
        {
            _clientApiService = clientApiService;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _clientApiService.GetAllClientsAsync();
            return View(clients);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = await _clientApiService.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        public IActionResult Create()
        {
            return View(new ClientCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = new Client
                {
                    Name = model.Name,
                    Region = model.Region,
                    Email = model.Email
                };

                await _clientApiService.CreateClientAsync(client);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = await _clientApiService.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            var model = new ClientEditViewModel
            {
                ClientId = client.ClientId,
                Name = client.Name,
                Region = client.Region,
                Email = client.Email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClientEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var client = new Client
                {
                    ClientId = model.ClientId,
                    Name = model.Name,
                    Region = model.Region,
                    Email = model.Email
                };

                var updated = await _clientApiService.UpdateClientAsync(client);

                if (!updated)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = await _clientApiService.GetClientByIdAsync(id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _clientApiService.DeleteClientAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}