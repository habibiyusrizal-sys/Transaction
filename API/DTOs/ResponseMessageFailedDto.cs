using System;

namespace API.DTOs;

public class ResponseMessageFailedDto
{
    public int Result { get; set; }
    public required string ResultMessage { get; set; }
}
