using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace All4GYM.Models;

public class Payment
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }
    public int OrderId { get; set; }

    public float Amount { get; set; }
    public string StripePaymentId { get; set; } = null!;
    public string Status { get; set; } = "Pending";
    public DateTime Date { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; } = null!;
}