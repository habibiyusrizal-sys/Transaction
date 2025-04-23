using System;

namespace API.DTOs;

public class ResponseMessageSuccessDto
{
    public int Result { get; set; }
    public long TotalAmount { get; set; }
    public long TotalDiscount { get; set; }
    public long FinalAmount { get; set; }
}
