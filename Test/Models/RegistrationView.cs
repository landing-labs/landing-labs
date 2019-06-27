using System;
using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class RegistrationView
    {
        [Required(ErrorMessage = "требуется ввести имя поль-ля")]
        [Display(Name = "Имя поль-ля")]
        public string Username { get; set; }
                
        [Required(ErrorMessage = "требуется ввести email")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        public Guid ActivationCode { get; set; }

        [Required(ErrorMessage = "требуется ввести пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password требуется ввести")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Ошибка : не совпадение паролей")]
        public string ConfirmPassword { get; set; }

    }
}