namespace TaskCraft.DTOs{
public class RegisterUserDTO
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string NickName { get; set; }
    public string PhoneNumber { get; set; }  
}
public class LoginUserDTO
{
    public string Login { get; set; }
    public string Password { get; set; }
}
public class GetUserDTO
{
    public Guid Id { get; set; }
    public string NickName { get; set; }
    public string Login { get; set; }
    public string PhoneNumber { get; set; }  
    public List<GetInListChannelDTO>? Channels { get; set; }
}
    public class UpdateUserDTO
    {
        public string Login { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NickName { get; set; }
        public string PhoneNumber { get; set; }  
}

}

