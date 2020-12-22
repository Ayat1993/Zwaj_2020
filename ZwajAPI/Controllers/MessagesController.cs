using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZwajAPI.Data;
using ZwajAPI.Dtos;
using ZwajAPI.Helpers;
using ZwajAPI.Models;

namespace ZwajAPI.Controllers
{

    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogUserActivity))]
    public class MessagesController : ControllerBase
    {
        private readonly IZwajRepository _repo;
        private readonly IMapper _mapper;
        public MessagesController(IZwajRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;

        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messageForRepo = await _repo.GetMessage(id);
            if (messageForRepo == null)
            {
                return NotFound();

            }
            return Ok(messageForRepo);



        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery] MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            messageParams.UserId = userId;
            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);
            var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);
            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);
            return Ok(messages);


        }



        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreation)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
      
            messageForCreation.SenderId = userId;
            var sender = await _repo.GetUser(messageForCreation.SenderId,true);
            var recipient = await _repo.GetUser(messageForCreation.RecipientId,false);
            if (recipient == null)
            {
                return BadRequest("لم يتم الوصول للمرسل اليه");

            }

            var message = _mapper.Map<Message>(messageForCreation);
            _repo.Add(message);

            if (await _repo.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageToReturnDto>(message);

                return CreatedAtRoute("GetMessage", new { id = message.Id }, messageToReturn);

            }
            throw new Exception("حدث خطأ اثناء حفظ الرسالة الجديدة");




        }

        [HttpGet("chat/{recipientId}")]
        public async Task<IActionResult> GetConversation(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var messagesForRepo =  await _repo.GetConversation(userId,recipientId) ; 
            var messagesToReturn = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesForRepo) ;
            return Ok(messagesToReturn) ;


        }
        [HttpGet("count")]
        public async Task<IActionResult> GetUnreadMessagesForUser(int userId){
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            var count = await _repo.GetUnreadMessagesForUser( int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value));
            return Ok(count);
        }

        [HttpPost("read/{id}")]
        public async Task<IActionResult> MarkMessageAsRead(int userId,int id){
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
             return Unauthorized();
             var message = await _repo.GetMessage(id);
             if(message.RecipientId != userId)
                 return Unauthorized();
            message.IsRead = true;
            message.DateRead=DateTime.Now;
            await _repo.SaveAll();
            return NoContent();
       }	
       [HttpPost("{id}")]
       public async Task<IActionResult> DeleteMessage(int id , int userId)
       {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
             return Unauthorized();
              var message = await _repo.GetMessage(id);
              if(message.SenderId==userId)
              {
                  message.SenderDeleted = true ; 

              }
              if(message.RecipientId==userId)
              {
                  message.RecipientDeleted = true ; 

              }
              if(message.SenderDeleted && message.RecipientDeleted)
                  _repo.Delete(message) ; 
              if(await _repo.SaveAll())
                return NoContent() ;

            throw new Exception("حدث خطأ في حذف الرسالة") ; 





       }	






    }
}