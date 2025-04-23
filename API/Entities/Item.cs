using System;

namespace API.Entities;

public class Item
{
    public int Id { get; set; }
    public string? PartnerItemRef { get; set; }
    public string? Name { get; set; }
    public int Qty { get; set; }
    public long UnitPrice { get; set; }
    public int MyProperty { get; set; }

    public Partner? Partner { get; set; }
}
