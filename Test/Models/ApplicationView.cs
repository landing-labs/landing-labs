using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class ApplicationView
    {
        [Required]
        [Display(Name = "Тема сообщения")]
        public string Theme { get; set; }
        
        [Display(Name = "Сообщение")]
        public string Body { get; set; }

        [Display(Name = "Файл")]
        public string FileName { get; set; }
    }
}