namespace All4GYM.Dtos;

public class RegisterUserDto
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int? RoleId { get; set; } // опційно, бо зазвичай встановлюється автоматично
}