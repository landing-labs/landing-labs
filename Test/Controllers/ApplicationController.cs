using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Test.CustomAuthentication;
using Test.DataAccess;
using Test.DomainModel;
using Test.Models;

namespace Test.Controllers
{
    public class ApplicationController : Controller
    {
        [HttpGet]
        [CustomAuthorize(Roles = "CLIENT")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize(Roles = "CLIENT")]
        public async Task<ActionResult> Create(ApplicationView applicationView, HttpPostedFileBase image)
        {
            string message = string.Empty;
            bool status = false;
            if (ModelState.IsValid)
            {
                Application app = new Application();
                app.Created = DateTime.Now;
                app.Theme = applicationView.Theme;
                app.Body = applicationView.Body;
                app.UserId = ((CustomPrincipal)HttpContext.User).Id;

                string emailTo = ((CustomPrincipal)HttpContext.User).Email;

                app.IsAnswered = false;
                if (image != null)
                {
                    app.FileName = image.FileName;
                    app.FileBody = new byte[image.ContentLength];
                    image.InputStream.Read(app.FileBody, 0, image.ContentLength);
                }

                using (ApplicationDB dbContext = new ApplicationDB())
                {
                    DateTime date = DateTime.Now.AddDays(-1);

                    var lastApp = dbContext.Applications.FirstOrDefault(a => a.Created > date);

                    if (lastApp == null)
                    {
                        dbContext.Applications.Add(app);
                        await dbContext.SaveChangesAsync();
                        status = true;
                        await SendEmail(app, emailTo);
                        message = "Ваша заявка успешно сохранена.";
                    }
                    else
                        message = $"Вы не можете создать заявку т.к. Можно создавать одну заявку в день. Последняя заявка была создана: {lastApp.Created}";

                }
              
            }
            else
                message = "Упс! Что-то пошло не так...";

            ViewBag.Status = status;
            ViewBag.Message = message;
            return View(applicationView);

        }


        [HttpGet]
        [CustomAuthorize(Roles = "MANAGER")]
        public ActionResult List()
        {
            var result = new List<Application>();
            using (ApplicationDB dbContext = new ApplicationDB())
            {
                result = dbContext.Applications.Include("User").ToList();
            }
            return View(result);
        }

        [HttpPost]
        [CustomAuthorize(Roles = "MANAGER")]
        public ActionResult List(FormCollection formCollection)
        {
            if (formCollection["AnswerIds"] == null)
                return RedirectToAction("List");

            var result = new List<Application>();
            string[] ids = formCollection["AnswerIds"].Split(new char[] { ',' });


            using (ApplicationDB dbContext = new ApplicationDB())
            {
                var appEdit = dbContext.Applications.ToList();

                foreach (string id in ids)
                {
                    int currentId = 0;
                    
                    if (int.TryParse(id, out currentId))
                    {
                        var editApp = dbContext.Applications.SingleOrDefault(a => a.Id == currentId);
                        editApp.IsAnswered = true;
                    }                   
                }
                dbContext.SaveChanges();

                result = dbContext.Applications.Include("User").ToList();
            }

            return View(result);

        }



        #region private 

        [NonAction]
        private async Task<bool> SendEmail(Application application, string emailTo)
        {
            string fromEmail = "specialist@landing-labs.ru";
            string fromEmailPassword = "19812208";
            
            StringBuilder msgBuilder = new StringBuilder();

            msgBuilder.Append("<html><body style='font-family:Arial,sans-serif;'>");
            msgBuilder.Append("<h2 style='font-weight:bold;border-bottom:1px dotted #ccc;'>Информация о заявке</h2>\r\n");
            msgBuilder.Append("<p><strong>Имя клиента:</strong> " + emailTo + "</p>\r\n");
            msgBuilder.Append("<p><strong>Тема:</strong> " + application.Theme + "</p>\r\n");
            msgBuilder.Append("<p><strong>Сообщение:</strong> </br>" + application.Body + "</p>\r\n");
            msgBuilder.Append("</body></html>");
                        
            MailMessage mailMessage = new MailMessage(fromEmail, GetManagerEmail());
            mailMessage.From = new MailAddress(fromEmail);
            mailMessage.Subject = "Новая заявка";
            mailMessage.Body = msgBuilder.ToString();
            mailMessage.IsBodyHtml = true;
            
            using (var smtpClient = new SmtpClient("smtp.yandex.ru", 25))
            {
                smtpClient.Credentials = new NetworkCredential(fromEmail, fromEmailPassword);
                smtpClient.EnableSsl = true;
                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        private string GetManagerEmail()
        {
            using (ApplicationDB dbContext = new ApplicationDB())
            {
                var user = dbContext.Users.Include("Roles").
                    Where(u => u.Roles.Any(r => r.Name.Contains("MANAGER"))).FirstOrDefault();


                return (user != null) ? user.Email : string.Empty; 
            }
        }

        #endregion 
    }
}