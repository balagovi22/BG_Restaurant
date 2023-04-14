using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.ProductAPI.Database;
using Restaurant.ShoppingCartAPI.Models;
using Restaurant.ShoppingCartAPI.Models.Dto;

namespace Restaurant.ShoppingCartAPI.Repository
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _db;

        public ShoppingCartRepository(IMapper mapper, ApplicationDbContext db)
        {
            _mapper = mapper;
            _db = db;
        }

        public async Task<ShoppingCartDto> GetShoppingCartByUserId(string userId)
        {
            ShoppingCart cart = new()
            {
                CartHeader = await _db.CartHeader.FirstOrDefaultAsync(u => u.UserId == userId),
            };

            cart.CartDetails = _db.CartDetail
                                    .Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId)
                                    .Include(u => u.Product);

            return _mapper.Map<ShoppingCartDto>(cart);
        }

        public async Task<bool> ApplyDiscount(string userId, string couponCode)
        {
            var cartHeaderFromDB = await _db.CartHeader.FirstOrDefaultAsync(u => u.UserId == userId);
            if (cartHeaderFromDB != null)
            {
                cartHeaderFromDB.CouponCode = couponCode;
                _db.CartHeader.Update(cartHeaderFromDB);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> ClearShoppingCart(string userId)
        {
            var cartHeaderFromDB = await _db.CartHeader.FirstOrDefaultAsync(u => u.UserId == userId);
            if (cartHeaderFromDB != null) {
                int cartHeaderId = cartHeaderFromDB.CartHeaderId;
                _db.CartDetail.RemoveRange(_db.CartDetail.Where(u => u.CartHeaderId == cartHeaderId));
                _db.CartHeader.Remove(cartHeaderFromDB);
                return true;
            }
            return false;
        }

       

        public async Task<ShoppingCartDto> CreateOrUpdateCart(ShoppingCartDto cartDto)
        {
            ShoppingCart cart = _mapper.Map<ShoppingCart>(cartDto);

            //Check if product exist in database, if not create it
            var productInDb = _db.Products.Find(cartDto.CartDetails.FirstOrDefault().ProductId);
            if (productInDb == null)
            {
                _db.Products.Add(cart.CartDetails.FirstOrDefault().Product);
                await _db.SaveChangesAsync();
            }

            //check if header is null
            var cartHeaderFromDb = await _db.CartHeader
                                        .AsNoTracking().FirstOrDefaultAsync(u=>u.UserId==cart.CartHeader.UserId);
            if (cartHeaderFromDb == null)
            {
                _db.CartHeader.Add(cart.CartHeader);
                await _db.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.CartHeaderId;
                cart.CartDetails.FirstOrDefault().Product = null;
                _db.CartDetail.Add(cart.CartDetails.FirstOrDefault());
                await _db.SaveChangesAsync();
            }
            else
            {
                var cardDetailsFromDb = await _db.CartDetail.AsNoTracking().FirstOrDefaultAsync(
                            u => u.ProductId == cart.CartDetails.FirstOrDefault().ProductId &&
                            u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                if (cardDetailsFromDb == null)
                {
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _db.CartDetail.Add(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //update the count
                    cart.CartDetails.FirstOrDefault().Count += cardDetailsFromDb.Count;
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cardDetailsFromDb.CartHeaderId;
                    cart.CartDetails.FirstOrDefault().CartDetailsId = cardDetailsFromDb.CartDetailsId;
                    _db.CartDetail.Add(cart.CartDetails.FirstOrDefault());
                    await _db.SaveChangesAsync();
                }
            }
            return _mapper.Map<ShoppingCartDto>(cart);
        }

      

        public async Task<bool> RemoveDiscount(string userId)
        {
            var cartHeaderFromDB = await _db.CartHeader.FirstOrDefaultAsync(u => u.UserId == userId);
            if (cartHeaderFromDB != null)
            {
                cartHeaderFromDB.CouponCode = "";
                _db.CartHeader.Update(cartHeaderFromDB);
                await _db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveFromCart(int CartDetailId)
        {

            try
            {
                CartDetail cartDetail = _db.CartDetail.FirstOrDefault(u => u.CartDetailsId == CartDetailId);
                int totalItemsInCart = _db.CartDetail.Where(u => u.CartHeaderId == cartDetail.CartHeaderId).Count();
                _db.CartDetail.Remove(cartDetail);
                if (totalItemsInCart == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeader.FirstOrDefaultAsync(u => u.CartHeaderId != cartDetail.CartHeaderId);
                    _db.CartHeader.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
