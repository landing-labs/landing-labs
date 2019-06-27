using System;

namespace Test.DomainModel
{
    public class Application : Entity
    {
        public int UserId { get; set; }

        public string Theme { get; set; }
        public string Body { get; set; }
        public DateTime Created { get; set; }

        public string FileName { get; set; }
        public byte[] FileBody { get; set; }
        
        public User User { get; set; }

        public bool IsAnswered { get; set; }
    }
}

//* ID, тема, сообщение, имя клиента, почта клиента, ссылка на прикрепленный файл, время создания