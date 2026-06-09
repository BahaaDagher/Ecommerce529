using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ecommerce529.Areas.Identity.Controllers
{
    [Area(CD.IDENTITY_AREA)]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM )
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM); 
            }
            var user = new ApplicationUser()
            {
                FirstName = registerVM.FirstName  , 
                LastName = registerVM.LastName ,
                Address = registerVM.Address ,
                Email = registerVM.Email ,
                UserName = registerVM.UserName 
            }; 
            var result =  await _userManager.CreateAsync(user , registerVM.Password); 
            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description); 
                }
                return View(registerVM); 
            }
            // Send Email 
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user); 
            var link = Url.Action(nameof(ConfirmEmail) , "Account" , new { area  = CD.IDENTITY_AREA , userId  = user.Id , token = token } , Request.Scheme ); 
            await _emailSender.SendEmailAsync(
                registerVM.Email , 
                "Ecommerce confirm Email" ,
                $"<h1>Please click <a href={link}>here</a> to Confirm Your Mail</h1>"
                );

            TempData["Success_Notification"] = "send Email Successfully";

            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return View(loginVM);
            }
            var user = await _userManager.FindByEmailAsync(loginVM.UserNameOrEmail)??
                        await _userManager.FindByNameAsync(loginVM.UserNameOrEmail);
            if (user is null )  
            {
                ModelState.AddModelError("", "Invalid UserName or Password"); 
                return View(loginVM); 
            }
            var result  = await _signInManager.PasswordSignInAsync(user  , loginVM.Password , loginVM.RememberMe,true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "user is Locked Please try again Later");
                }
                else if (result.IsNotAllowed)
                {
                    ModelState.AddModelError("", "please Confirm Your Email First");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid UserName or Password");
                }
                return View(loginVM); 
            }
            return RedirectToAction("Index", "Home",new { area = CD.CUSTOMER_AREA} );
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId  , string token )
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound(); 

            var result = await _userManager.ConfirmEmailAsync(user , token);
            if (!result.Succeeded)
            {
                string errors = ""; 
                foreach(var error in result.Errors)
                {
                    errors += error.Description + "\n"; 
                }
                TempData["Error_Notification"] = errors;
                return RedirectToAction(nameof(Login) , "Account" , new { area  =  CD.IDENTITY_AREA } ); 
            }
            TempData["Success_Notification"] = "Email Confirmed Successfully";
            return  RedirectToAction(nameof(Login), "Account", new { area = CD.IDENTITY_AREA });
        }


        [HttpGet]
        public IActionResult ResendEmailConfirmation(){
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendEmailConfirmationVM resendEmailConfirmationVM)
        {
            var user = await _userManager.FindByEmailAsync(resendEmailConfirmationVM.UserNameOrEmail) ??
                       await _userManager.FindByNameAsync(resendEmailConfirmationVM.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError("", "Invalid UserName or Password");
                return View(resendEmailConfirmationVM);
            }
            // Send Email 
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var link = Url.Action(nameof(ConfirmEmail), "Account", new { area = CD.IDENTITY_AREA, userId = user.Id, token = token }, Request.Scheme);
            await _emailSender.SendEmailAsync(
                user.Email,
                "Ecommerce confirm Email",
                $"<h1>Please click <a href={link}>here</a> to Confirm Your Mail</h1>"
                );

            return RedirectToAction(nameof(Login));
        }




    }
}
