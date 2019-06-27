using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Test.CustomAuthentication;
using Test.DataAccess;
using Test.DomainModel;
using Test.Models;

namespace Test.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {

        private const string ROLENAME = "CLIENT";

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login(string ReturnUrl = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                return LogOut();
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginView loginView)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(loginView.UserName, loginView.Password))
                {
                    var user = (CustomMembershipUser)Membership.GetUser(loginView.UserName, false);
                    if (user != null)
                    {
                        CustomSerializeModel userModel = new CustomSerializeModel()
                        {
                            Id = user.Id,
                            UserName = user.UserName,
                            RoleName = user.Roles.Select(r => r.Name).ToList()
                        };

                        string userData = JsonConvert.SerializeObject(userModel);
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket
                            (
                            1, loginView.UserName, DateTime.Now, DateTime.Now.AddMinutes(15), false, userData
                            );

                        string enTicket = FormsAuthentication.Encrypt(authTicket);
                        HttpCookie faCookie = new HttpCookie("CookieTest", enTicket);
                        Response.Cookies.Add(faCookie);
                    }
                    
                    if (user.Roles.Where(s => s.Name.Contains("MANAGER")).SingleOrDefault() != null)
                        return RedirectToAction("List", "Application");

                    return RedirectToAction("Create", "Application");

                }
            }
            ModelState.AddModelError("", "УПС! Поль-ля с такими именем и паролем - нет.");
            return View(loginView);
        }

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(RegistrationView registrationView)
        {
            bool statusRegistration = false;
            string messageRegistration = string.Empty;

            if (ModelState.IsValid)
            {
                string userName = Membership.GetUserNameByEmail(registrationView.Email);
                if (!string.IsNullOrEmpty(userName))
                {
                    ModelState.AddModelError("Неверный Email", "Ошибка: Email уже существует");
                    return View(registrationView);
                }

                using (ApplicationDB dbContext = new ApplicationDB())
                {
                    var user = new User()
                    {
                        Username = registrationView.Username,
                        Email = registrationView.Email,
                        Password = registrationView.Password,
                        IsActive = true
                    };

                    var managerRole = (from roles in dbContext.Roles
                                       where string.Compare(roles.Name, ROLENAME, StringComparison.OrdinalIgnoreCase) == 0
                                       select roles).FirstOrDefault();
                    user.Roles.Add(managerRole);

                    dbContext.Users.Add(user);
                    dbContext.SaveChanges();
                }

                messageRegistration = "Аккаунт - успешно создан.";
                statusRegistration = true;
            }
            else
            {
                messageRegistration = "Упс! Что-то пошло не так...";
            }
            ViewBag.Message = messageRegistration;
            ViewBag.Status = statusRegistration;

            return View(registrationView);
        }

       

        public ActionResult LogOut()
        {
            HttpCookie cookie = new HttpCookie("CookieTest", "");
            cookie.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie);

            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account", null);
        }

        
    }
}