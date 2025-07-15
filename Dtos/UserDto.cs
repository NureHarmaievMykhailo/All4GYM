using All4GYM.Models;

namespace All4GYM.Dtos;

public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public bool HasActiveSubscription { get; set; } = false;
    public SubscriptionTier SubscriptionTier { get; set; } = SubscriptionTier.Basic;
}