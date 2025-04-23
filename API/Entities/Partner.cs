using System;

namespace API.Entities;

public class Partner
{
    public int Id { get; set; }
    public string? PartnerKey { get; set; }
    public string? PartnerRefNo { get; set; }
    public string? PartnerPassword { get; set; }
    public long TotalAmount { get; set; }
    public ICollection <Item> Items { get; set; } = new List<Item>();
    public string? TimeStamp { get; set; }
    public string? Sig { get; set; }
}
