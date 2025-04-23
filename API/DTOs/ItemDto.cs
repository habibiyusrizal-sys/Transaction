using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class ItemDto
{
    [Required(ErrorMessage = "Reference ID cannot be null or empty")]
    [MaxLength(50)]
    public string? PartnerItemRef { get; set; }

    [Required(ErrorMessage = "Name cannot be null or empty")]
    [MaxLength(100)]
    public required string Name { get; set; }

    //[Range(1, 5, ErrorMessage = "Value atleast more than 1 and must not exceed 5")]
    public int Qty { get; set; }

    //[Range(1, long.MaxValue, ErrorMessage = "Unit price must be positive value in cents")]
    public long UnitPrice { get; set; }
}
