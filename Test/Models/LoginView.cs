using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class LoginView
    {
        [Required]
        [Display(Name = "Имя поль-ля")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Запомнить")]
        public bool RememberMe { get; set; }
    }
}