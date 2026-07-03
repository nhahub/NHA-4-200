using Microsoft.AspNetCore.Mvc;
using ZoneSync.Service.Contracts;
using ZoneSync.Web.ViewModels.Account;

namespace ZoneSync.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityService identityService;

        public AccountController(IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        public IActionResult RegisterOwner()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterOwner(RegisterOwnerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await identityService.RegisterOwnerAsync(
                    model.Email,
                    model.Password,
                    model.FirstName,
                    model.LastName,
                    model.PhoneNumber,
                    model.SSN);

                return RedirectToAction(nameof(Login));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(model);
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var isLoggedIn = await identityService.PasswordSignInAsync(model.Email, model.Password, model.RememberMe);

            if (!isLoggedIn)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password");
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await identityService.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult CreateInvitation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInvitation(CreateInvitationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var invitation = await identityService.CreateInvitationAsync(
                model.SentByUserId,
                model.FarmId,
                model.InvitedEmail,
                model.InvitedPhone,
                model.InvitedRole);

            model.CreatedToken = invitation.InvitationToken;
            model.VerificationCode = invitation.VerificationCode;

            return View(model);
        }

        public IActionResult AcceptInvitation(string? token)
        {
            return View(new AcceptInvitationViewModel
            {
                Token = token ?? string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptInvitation(AcceptInvitationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var invitation = await identityService.AcceptInvitationAsync(
                model.Token,
                model.VerificationCode,
                model.ReceivedByUserId);

            if (invitation is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid or expired invitation");
                return View(model);
            }

            TempData["SuccessMessage"] = "Invitation accepted successfully";

            return RedirectToAction("Index", "Home");
        }
    }
}
