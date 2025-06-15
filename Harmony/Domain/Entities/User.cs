namespace TaskCraft.Entities
{
    public class User
    {
        public Guid Id { get; set; } // Уникальный идентификатор пользователя
        public string Login { get; set; } // Логин для входа в систему
        public string Password { get; set; } // Хэшированный пароль
        public string NickName { get; set; } // Отображаемое имя пользователя
        public string PhoneNumber { get; set; } // Номер телефона пользователя
        public List<Channel>? Channels { get; set; } = new List<Channel>(); // Каналы, в которых участвует пользователь
        public List<Channel> OwnedChannels { get; set; } = new List<Channel>(); // Каналы, созданные пользователем (является владельцем)
    }
}