using System;
using System.ComponentModel.DataAnnotations;
using API.Entities;

namespace API.DTOs;

public class PartnerDto
{
    [Required(ErrorMessage = "partnerkey is required")]
    [MaxLength(50)]
    public required string PartnerKey { get; set; }

    [Required(ErrorMessage = "partnerrefno is required")]
    [MaxLength(50)]
    public required string PartnerRefNo { get; set; }

    [Required(ErrorMessage = "partnerpassword is required")]
    [MaxLength(50)]
    public required string PartnerPassword { get; set; }

    //[Required(ErrorMessage = "totalamount is required")]
    public required long TotalAmount { get; set; }

    [Required(ErrorMessage = "items are required")]
    public ICollection<ItemDto> Items { get; set; } = new List<ItemDto>();

    [Required(ErrorMessage = "timestamp is required")]
    public required string TimeStamp { get; set; }

    [Required(ErrorMessage = "sig is required")]
    public required string Sig { get; set; }
}