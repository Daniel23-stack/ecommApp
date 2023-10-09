using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EcommApp.Data;
using EcommApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommApp.Controllers
{
    [Route("api/shoppinCart")]
    [ApiController]
    public class ShoppingCartController : ControllerBase
    {
        protected ApiResponse _response;
        private readonly ApplicationDbContext _dbContext;

        public ShoppingCartController(ApiResponse response, ApplicationDbContext dbContext)
        {
            _response = response;
            _dbContext = dbContext;
        }
        //get shopping cart
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetShoppingCart(string userId)
        {
            try
            {
                ShoppingCart shoppingCart;
                if (string.IsNullOrEmpty(userId))
                {
                    shoppingCart = new();
                }
                else
                {
                    shoppingCart= _dbContext.ShoppingCarts
                        .Include(u => u.CartItems).ThenInclude(u => u.MenuItem)
                        .FirstOrDefault(u => u.UserId == userId);

                }
                if (shoppingCart.CartItems != null && shoppingCart.CartItems.Count > 0) {
                    shoppingCart.CartTotal = shoppingCart.CartItems.Sum(u => u.Quantity * u.MenuItem.Price);
                }
                _response.Result = shoppingCart;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                    = new List<string>() { ex.ToString() };
                _response.StatusCode = HttpStatusCode.BadRequest;
            }
            return _response;
        }
        
        //edit or update shop cart
         [HttpPost]
        public async Task<ActionResult<ApiResponse>> AddOrUpdateItemInCart(string userId, int menuItemId, int updateQuantityBy)
        {
            
            ShoppingCart shoppingCart = _dbContext.ShoppingCarts.Include(u=>u.CartItems).FirstOrDefault(u => u.UserId == userId);
            MenuItem menuItem = _dbContext.MenuItems.FirstOrDefault(u => u.Id == menuItemId);
            if(menuItem == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            if(shoppingCart==null && updateQuantityBy > 0)
            {
                //create a shopping cart & add cart item

                ShoppingCart newCart = new() { UserId = userId };
                _dbContext.ShoppingCarts.Add(newCart);
                _dbContext.SaveChanges();

                CartItem newCartItem = new()
                {
                    MenuItemId = menuItemId,
                    Quantity = updateQuantityBy,
                    ShoppingCartId = newCart.Id,
                    MenuItem=null
                };
                _dbContext.CartItems.Add(newCartItem);
                _dbContext.SaveChanges();
            }
            else
            {
                //shopping cart exists

                CartItem cartItemInCart = shoppingCart.CartItems.FirstOrDefault(u => u.MenuItemId == menuItemId);
                if(cartItemInCart == null)
                {
                    //item does not exist in current cart
                    CartItem newCartItem = new()
                    {
                        MenuItemId = menuItemId,
                        Quantity = updateQuantityBy,
                        ShoppingCartId = shoppingCart.Id,
                        MenuItem = null
                    };
                    _dbContext.CartItems.Add(newCartItem);
                    _dbContext.SaveChanges();
                }
                else
                {
                    //item already exist in the cart and we have to update quantity
                    int newQuantity = cartItemInCart.Quantity + updateQuantityBy;
                    if(updateQuantityBy==0 || newQuantity <= 0)
                    {
                        //remove cart item from cart and if it is the only item then remove cart
                        _dbContext.CartItems.Remove(cartItemInCart);
                        if (shoppingCart.CartItems.Count() == 1)
                        {
                            _dbContext.ShoppingCarts.Remove(shoppingCart);
                        }
                        _dbContext.SaveChanges();
                    }
                    else
                    {
                        cartItemInCart.Quantity= newQuantity;
                        _dbContext.SaveChanges();
                    }
                }
            }
            return _response;

        }
    }
}
