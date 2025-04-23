using System.Globalization;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using log4net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers;

public class SubmitTrxMessageController : BaseApiController
{
    private static readonly ILog _logger = LogManager.GetLogger(typeof(SubmitTrxMessageController));


    [HttpPost("submittrxmessage")]
    public ActionResult<ResponseMessageSuccessDto> SubmitTrx([FromBody]PartnerDto partnerDto)
    {

        _logger.Info($"REQUEST BODY: {JsonConvert.SerializeObject(partnerDto)}");

        if (partnerDto.TotalAmount <=0)
        {
            var errorResponse =  new ResponseMessageFailedDto
            {
                Result = 0,
                ResultMessage="Total amount must not negative value"
            };

            _logger.Warn($"Validation Failed - {JsonConvert.SerializeObject(errorResponse)}");

            return BadRequest(errorResponse);
        }

        string passwordMatch = DecodeBase64(partnerDto.PartnerPassword);

        if (passwordMatch != "FAKEPASSWORD1234" && passwordMatch != "FAKEPASSWORD4578")
        {
            var errorResponse = new ResponseMessageFailedDto
            {
                Result = 0,
                ResultMessage = "Access Denied!"
            };

            _logger.Warn($"Validation Failed - {JsonConvert.SerializeObject(errorResponse)}");

            return BadRequest(errorResponse);

        }

        var serverTime = DateTime.UtcNow;

        if (!DateTime.TryParse(partnerDto.TimeStamp,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal,
                out var clientTime))
        {
            var errorResponse = new ResponseMessageFailedDto
            {
                Result = 0,
                ResultMessage = "Invalid timestamp format."
            };

            _logger.Warn($"Validation Failed - {JsonConvert.SerializeObject(errorResponse)}");

            return BadRequest(errorResponse);
        }


        Console.WriteLine($"Client Time: {clientTime: yyyy-MM-dd HH:mm:ss} UTC");

        var timeDifference = (serverTime - clientTime).Duration(); // absolute time difference

        if (timeDifference > TimeSpan.FromMinutes(5))
        {
            var errorResponse = new ResponseMessageFailedDto
            {
                Result = 0,
                ResultMessage = "Expired."
            };

            _logger.Warn($"Validation Failed - {JsonConvert.SerializeObject(errorResponse)}");

            return BadRequest(errorResponse);
        }


        // Sig validation
        string timestampFormatted = clientTime.ToString("yyyyMMddHHmmss");

        string rawSignature = timestampFormatted +
                                partnerDto.PartnerKey +
                                partnerDto.PartnerRefNo +
                                partnerDto.TotalAmount.ToString() +
                                partnerDto.PartnerPassword;


        using var sha256 = SHA256.Create();

        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawSignature));

        string serverGeneratedSig = Convert.ToBase64String(hashBytes);

        Console.WriteLine("Server Generated Sig: " + serverGeneratedSig);

        if (partnerDto.Sig != serverGeneratedSig)
        {
            var errorResponse = new ResponseMessageFailedDto
            {
                Result = 0,
                ResultMessage = "Access Denied!"
            };

            _logger.Warn($"Validation Failed - {JsonConvert.SerializeObject(errorResponse)}");

            return Unauthorized(errorResponse);
        }

        if (partnerDto.Items.Any( i => i.UnitPrice <=0))
        {
            var errorResponse = new ResponseMessageFailedDto
            {
                Result = 0,
                ResultMessage = "Unit price must be a positive value in cents."
            };

            _logger.Warn($"Validation Failed - {JsonConvert.SerializeObject(errorResponse)}");

            return BadRequest(errorResponse);
        }

        if (partnerDto.Items.Any(i => i.Qty > 5))
        {
            var errorResponse = new ResponseMessageFailedDto
            {
                Result = 0,
                ResultMessage = "Quantity must not exceed 5."
            };

            _logger.Warn($"Validation Failed - {JsonConvert.SerializeObject(errorResponse)}");

            return BadRequest(errorResponse);

        }

        decimal amountInCents = partnerDto.TotalAmount / 100m;

        decimal baseDiscountPercent = 0m;

        // Base Discounts
        if (amountInCents >= 200 && amountInCents <= 500)
            baseDiscountPercent = 5m;
        else if (amountInCents > 500 && amountInCents <=800)
            baseDiscountPercent = 7m;
        else if (amountInCents > 800 && amountInCents <= 1200)
            baseDiscountPercent = 10m;
        else if (amountInCents > 1200)
            baseDiscountPercent = 15m;

        decimal totalDiscountPercent = baseDiscountPercent;

        // Conditional Discounts
        if (amountInCents > 500 && IsPrime((long) amountInCents))
            totalDiscountPercent += 8m;

        if (amountInCents > 900 && amountInCents % 10 == 5)
            totalDiscountPercent += 10m;

        // Cap discount to 20%
        if (totalDiscountPercent > 20m)
            totalDiscountPercent = 20m;

        decimal totalDiscount = totalDiscountPercent / 100m * amountInCents;
        decimal finalAmount = amountInCents - totalDiscount;

        var responseMessageSuccessDto = new ResponseMessageSuccessDto
        {
            Result = 1,
            TotalAmount = partnerDto.TotalAmount,
            TotalDiscount = (long)totalDiscount * 100,
            FinalAmount = (long)finalAmount * 100
        };

        _logger.Info("RESPONSE BODY:");
        _logger.Info(JsonConvert.SerializeObject(responseMessageSuccessDto));

        return responseMessageSuccessDto;
    }

    private string DecodeBase64(string base64Encoded)
    {
        byte[] bytes = Convert.FromBase64String(base64Encoded);
        return Encoding.UTF8.GetString(bytes);
    }

    private bool IsPrime(long number)
    {
        if (number <= 1) return false;

        if (number == 2) return true;

        if (number % 2 == 0) return false;

        var boundary = (long)Math.Floor(Math.Sqrt(number));

        for (long i = 3; i < boundary; i+=2)
        {
            if (number % i == 0) return false;
        }

        return true;
    }

}
