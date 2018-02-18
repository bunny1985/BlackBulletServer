using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationBackend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace NotificationBackend.Controllers
{

    [Route("api/[controller]")]
    
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;


        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)

        {
            _userManager = userManager;
            _signInManager = signInManager;

        }

        [HttpPost]
        [AllowAnonymous]

        [Route("Register")]
        public async Task<IActionResult> RegisterMe([FromBody]RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    "Please confirm your account by clicking this link: <a href=\"" + callbackUrl + "\">link</a>");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    
                    return Ok();
                }
            
            }
            return BadRequest();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> LoginMe([FromBody]LoginViewModel model, [FromQuery]string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    
                    return Ok("logged IN!");
                }
                if (result.RequiresTwoFactor)
                {
                    return BadRequest("TWO FACTOR???");
                }
                if (result.IsLockedOut)
                {
                    
                    return BadRequest("LOCKED");
                }
                else
                {
                    return BadRequest("Invalid name or password");
                }
                
            }
            return BadRequest();
            // If we got this far, something failed, redisplay form
            
        }

    }
}