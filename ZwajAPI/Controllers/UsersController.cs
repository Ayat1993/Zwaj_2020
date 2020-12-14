using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using ZwajAPI.Data;
using ZwajAPI.Dtos;
using ZwajAPI.Helpers;
using ZwajAPI.Models;

namespace ZwajAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogUserActivity))]
    public class UsersController : ControllerBase
    {
        private readonly IZwajRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<StripeSettings> _stripeSettings;
        public UsersController(IZwajRepository repo, IMapper mapper, IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings;
            _mapper = mapper;
            _repo = repo;

        }
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(currentUserId);
            userParams.UserId = currentUserId;
            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "1" ? "2" : "1";
            }
            var users = await _repo.GetUsers(userParams);
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(usersToReturn);

        }
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailsDto>(user);
            return Ok(userToReturn);

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto userUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userForRepo = await _repo.GetUser(id);

            _mapper.Map(userUpdateDto, userForRepo);
            if (await _repo.SaveAll())
            {
                return NoContent();
            }
            throw new Exception($"حدثت مشكلة في تعديل بيانات المشترك {id}");

        }
        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();

            }
            var like = await _repo.GetLike(id, recipientId);
            if (like != null)
            {
                return BadRequest("لقد قمت بالاعجاب من قبل");

            }
            if (await _repo.GetUser(recipientId) == null)
            {
                return NotFound();

            }
            like = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

            _repo.Add<Like>(like);
            if (await _repo.SaveAll())
            {
                return Ok();

            }
            return BadRequest("فشل في الاعجاب");



        }

        [HttpDelete("{id}/delete/{recipientId}")]
        public async Task<IActionResult> DeleteLike(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();

            }
            if (await _repo.GetUser(recipientId) == null)
            {
                return NotFound();

            }

            var like = await _repo.GetLike(id, recipientId);
            if (like != null)
            {
                _repo.Delete(like);

            }


            if (await _repo.SaveAll())
            {
                return Ok();
            }
            else
            {
                return BadRequest("فشل حذف الاعجاب");
            }

        }

        [HttpPost("{userId}/charge/{stripeToken}")]
        public async Task<IActionResult> Charge(int userId, string stripeToken)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var customers = new CustomerService();
            var charges = new ChargeService();
			
            // var options = new TokenCreateOptions
            // {
            // Card = new CreditCardOptions
            //     {
            //         // Number = "4242424242424242",
            //         // ExpYear = 2020,
            //         // ExpMonth = 3,
            //         // Cvc = "123"
            //     }
            // };

            // var service = new TokenService();
            // Token stripeToken = service.Create(options);

            var customer = customers.Create(new CustomerCreateOptions {
                
            Source = stripeToken
            });

            var charge = charges.Create(new ChargeCreateOptions {
            Amount = 5000,
            Description = "إشتراك مدى الحياة",
            Currency = "usd",
            Customer = customer.Id
            });

            var payment = new Payment{
                PaymentDate = DateTime.Now,
                Amount = charge.Amount/100,
                UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value),
                ReceiptUrl = charge.ReceiptUrl,
                Description = charge.Description,
                Currency = charge.Currency,
                IsPaid = charge.Paid
            };
            _repo.Add<Payment>(payment);
            if(await _repo.SaveAll()){
           return Ok(new {IsPaid = charge.Paid } );
            }
            
            return BadRequest("فشل في السداد");

        }
        [HttpGet("{userId}/payment")]
        public async Task<IActionResult> GetPaymentForUser(int userId)
        {
              if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

                var payment = await _repo.GetPaymentForUser(userId);
                return Ok(payment); 
            

        }

        


    }
}