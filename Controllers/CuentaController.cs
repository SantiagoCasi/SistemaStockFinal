using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SistemaStock.Models.Entities;
using SistemaStock.Models.ViewModels.Cuenta;

namespace SistemaStock.Controllers
{
    public class CuentaController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public CuentaController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        //---------- LOGIN / LOGOUT ----------
        [HttpGet]
        [AllowAnonymous]
        // Permitir acceso sin autenticación
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // PasswordSignInAsync valida el usuario, la contraseña y si la cuenta está bloqueada
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RecordarSesion,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    var usuario = await _userManager.FindByEmailAsync(model.Email);
                    if (usuario != null && !usuario.Activo)
                    {
                        await _signInManager.SignOutAsync();
                        ModelState.AddModelError("", "El usuario está inactivo. Consulte con un administrador.");
                        return View(model);
                    }
                    return LocalRedirect(returnUrl);
                }
                if (result.IsLockedOut)
                    ModelState.AddModelError("", "Cuenta bloqueada. Intente en 5 minutos.");
                else
                    ModelState.AddModelError("", "Email o contraseña incorrectos.");
            }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccesoDenegado() => View();

        // ---------- CAMBIAR PASSWORD ----------
        [HttpGet, Authorize]
        public IActionResult CambiarPassword() => View(new CambiarPasswordViewModel());

        [HttpPost, Authorize, ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(CambiarPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            var result = await _userManager.ChangePasswordAsync(
                user, model.PasswordActual, model.NuevaPassword);

            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user); // mantiene la sesión
            TempData["Ok"] = "Contraseña actualizada.";
            return RedirectToAction("Index", "Home");
        }

    }
}
