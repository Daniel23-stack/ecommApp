using EcommApp.Data;
using EcommApp.Models;
using Stripe;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommApp.Controllers;

public class PaymentController
{
    protected ApiResponse _response;
    private readonly IConfiguration _congifuration;
    private readonly ApplicationDbContext _dbContext;

    public PaymentController(IConfiguration congifuration, ApplicationDbContext dbContext)
    {
        _congifuration = congifuration;
        _dbContext = dbContext;
        _response = new ApiResponse();
    }
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> MakePayment(string userId)
    {
        ShoppingCart shoppingCart = _dbContext.ShoppingCarts
            .Include(u => u.CartItems)
            .ThenInclude(u => u.MenuItem).FirstOrDefault(u => u.UserId == userId);

        if (shoppingCart == null || shoppingCart.CartItems == null || shoppingCart.CartItems.Count() == 0)
        {
            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            return BadRequest(_response);
        }

        //region Create Payment Intent

        StripeConfiguration.ApiKey = _congifuration["StripeSettings:SecretKey"];
        shoppingCart.CartTotal = shoppingCart.CartItems.Sum(u => u.Quantity * u.MenuItem.Price);

        PaymentIntentCreateOptions options = new()
        {
            Amount = (int)(shoppingCart.CartTotal * 100),
            Currency = "usd",
            PaymentMethodTypes = new List<string>
            {
                "card",
            },
        };
        PaymentIntentService service = new();
        PaymentIntent response = service.Create(options);
        shoppingCart.StripePaymentIntentId = response.Id;
        shoppingCart.ClientSecret = response.ClientSecret;
        
        //endregion
        _response.Result = shoppingCart;
        _response.StatusCode = HttpStatusCode.OK;
        return Ok(_response);
    }

    private ActionResult<ApiResponse> Ok(ApiResponse response)
    {
        throw new NotImplementedException();
    }

    private ActionResult<ApiResponse> BadRequest(ApiResponse response)
    {
        throw new NotImplementedException();
    }
}