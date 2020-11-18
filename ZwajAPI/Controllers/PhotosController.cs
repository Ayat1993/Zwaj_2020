
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ZwajAPI.Data;
using ZwajAPI.Dtos;
using ZwajAPI.Helpers;
using ZwajAPI.Models;

namespace ZwajAPI.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;

        private readonly IMapper _mapper;

        private readonly IZwajRepository _repo;
        private Cloudinary _cloudinary;

        public PhotosController(IZwajRepository repo, IOptions<CloudinarySettings> cloudinaryConfig, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
            _cloudinaryConfig = cloudinaryConfig;
            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret


            );
            _cloudinary = new Cloudinary(acc);



        }
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photFromRepository = await _repo.GetPhoto(id);
            var photo = Mapper.Map<PhotoFromReturnDto>(photFromRepository);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreateDto photoForCreateDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userForRepo = await _repo.GetUser(userId);
            var file = photoForCreateDto.File;
            var uploadResult = new ImageUploadResult();
            if (file != null && file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {

                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")

                    };
                    uploadResult = _cloudinary.Upload(uploadParams);

                }

            }
            photoForCreateDto.Url = uploadResult.Uri.ToString();
            photoForCreateDto.PublicId = uploadResult.PublicId;
            var photo = _mapper.Map<Photo>(photoForCreateDto);
            if (!userForRepo.Photos.Any(p => p.IsMain))
                photo.IsMain = true;
                //اضافة الصورة للمستخدم وبالتالي اضافتها للداتا بيس  يتم 
            userForRepo.Photos.Add(photo);
            if (await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoFromReturnDto>(photo);
                //جلب للصورة 
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);

            }
            //  return Ok() ;
            return BadRequest("خطأ في اضافة الصورة");




        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {

            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userForRepo = await _repo.GetUser(userId);
            if (!userForRepo.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();

            }
            var DesiredMainPhoto = await _repo.GetPhoto(id);
            if (DesiredMainPhoto.IsMain)
            {
                return BadRequest("هذه الصورة الاساسية بالفعل");
            }
            var CurrentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            CurrentMainPhoto.IsMain = false;
            DesiredMainPhoto.IsMain = true;
            
            if (await _repo.SaveAll())
            {
                return NoContent();


            }
            return BadRequest("لا يمكن تعديل الصورة الشخصية");



        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userForRepo = await _repo.GetUser(userId);
            if (!userForRepo.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();

            }
            var photo = await _repo.GetPhoto(id);
            if (photo.IsMain)
            {
                return BadRequest("هذه الصورة الاساسية لا يمكن حذفها ");
            }
            if (photo.PublicId != null)
            {
                var deletionParams = new DeletionParams(photo.PublicId);
            
                var result = this._cloudinary.Destroy(deletionParams);
                if(result.Result=="ok"){
                    _repo.Delete(photo) ;

                }
            }
            else
            {
                _repo.Delete(photo) ;

            }
            if(await _repo.SaveAll()) {
                return Ok() ; 
            } 
            else {
                return BadRequest("فشل حذف الصورة") ; 
            }

        }

    }
}