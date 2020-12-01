using System.Linq;
using AutoMapper;
using ZwajAPI.Dtos;
using ZwajAPI.Models;

namespace ZwajAPI.Helpers
{
    public class AutoMapperProfiles :Profile 
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,UserForListDto>()
            .ForMember(dest=>dest.PhotoURL,opt=>{opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url);})
            .ForMember(dest=>dest.Age ,opt=>{opt.ResolveUsing(src=>src.DateOfBirth.CalculateAge());});


            CreateMap<User,UserForDetailsDto>().
            ForMember(dest=>dest.PhotoURL,opt=>{opt.MapFrom(src=>src.Photos.FirstOrDefault(p=>p.IsMain).Url);})
            .ForMember(dest=>dest.Age ,opt=>{opt.ResolveUsing(src=>src.DateOfBirth.CalculateAge());});


            CreateMap<Photo,PhotoForDetailsDto>() ; 
            CreateMap<UserUpdateDto,User>() ; 
            CreateMap<Photo,PhotoFromReturnDto>() ;
            CreateMap<PhotoForCreateDto,Photo>() ;
            
            CreateMap<UserForRegisterDto,User>() ;
        
            CreateMap<MessageForCreationDto,Message>().ReverseMap();
            CreateMap<Message,MessageToReturnDto>()
            .ForMember(dest=>dest.SenderPhotoUrl,opt=>{opt.MapFrom(src=>src.Sender.Photos.FirstOrDefault(u=>u.IsMain).Url);})
            .ForMember(dest=>dest.RecipientPhotoUrl,opt=>{opt.MapFrom(src=>src.Recipient.Photos.FirstOrDefault(u=>u.IsMain).Url);})
            .ForMember(des=>des.SenderKnownAs,map => map.MapFrom(src=>src.Sender.KnownAs));




            


        }
        
    }
}