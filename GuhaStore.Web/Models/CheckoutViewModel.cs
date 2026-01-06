using System.ComponentModel.DataAnnotations;
using GuhaStore.Core.Entities;

namespace GuhaStore.Web.Models;

public class CheckoutViewModel
{
    [Required(ErrorMessage = "Tên người nhận là bắt buộc")]
    [Display(Name = "Tên người nhận")]
    [StringLength(100)]
    public string DeliveryName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Display(Name = "Số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(20)]
    public string DeliveryPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    [Display(Name = "Địa chỉ")]
    [StringLength(200)]
    public string DeliveryAddress { get; set; } = string.Empty;

    [Display(Name = "Ghi chú")]
    [StringLength(200)]
    public string DeliveryNote { get; set; } = string.Empty;

    public Dictionary<string, object> CartItems { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

