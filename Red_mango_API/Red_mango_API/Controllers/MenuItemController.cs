using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Red_mango_API.Data;
using Red_mango_API.Models;
using Red_mango_API.Models.Dto;
using Red_mango_API.Services;
using System.Net;

namespace Red_mango_API.Controllers
{
    [Route("api/MenuItem")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ApiResponse _response;
        private IImageService _imageService;
        private readonly string _imagesPath;

        public MenuItemController(ApplicationDbContext db, IImageService imageService)
        {
            _db = db;
            _imageService = imageService;
            _response = new ApiResponse();
            _imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
        }

        [HttpGet]
        public async Task<IActionResult> GetMenuItems()
        {
            var menuItems= await _db.MenuItems.ToListAsync();
            //foreach(var menuItem in menuItems)
            //{
            //    string image = Path.Combine("F:\\Projects\\Red_Mango\\red_mango_app\\src\\Assets\\Images\\foods", menuItem.Image);
            //    menuItem.Image = image.Replace("\\", "/");
            //}
            _response.Result = menuItems;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        [HttpGet("{id:int}",Name = "GetMenuItem")]
        public async Task<IActionResult> GetMenuItem(int id)
        {
            if(id == 0)
            {
                _response.StatusCode=HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            MenuItem menuItem = await _db.MenuItems.FirstOrDefaultAsync(u=>u.Id==id);
            if(menuItem == null)
            {
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }
            //string image = Path.Combine("F:\\Projects\\Red_Mango\\red_mango_app\\src\\Assets\\Images\\foods", menuItem.Image);
            //menuItem.Image = image.Replace("\\", "/");
            _response.Result = menuItem;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> CreateMenuItem([FromForm] MenuItemCreateDTO menuItemCreateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (menuItemCreateDTO.File == null || menuItemCreateDTO.File.Length == 0)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }
                    string filename = $"{Guid.NewGuid()}{Path.GetExtension(menuItemCreateDTO.File.FileName)}";
                    MenuItem menuItemToCreate = new()
                    {
                        Name = menuItemCreateDTO.Name,
                        Price = menuItemCreateDTO.Price,
                        Category = menuItemCreateDTO.Category,
                        SpecialTag = menuItemCreateDTO.SpecialTag,
                        Description = menuItemCreateDTO.Description,
                        Image = await _imageService.UploadImage(filename, menuItemCreateDTO.File)
                    };
                    await _db.MenuItems.AddAsync(menuItemToCreate);
                    await _db.SaveChangesAsync();
                    _response.StatusCode = HttpStatusCode.Created;
                    _response.Result = menuItemToCreate;
                    return CreatedAtRoute("GetMenuItem", new { id = menuItemToCreate.Id }, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ApiResponse>> UpdateMenuItem(int id, [FromForm] MenuItemUpdateDTO menuItemUpdateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (menuItemUpdateDTO == null || menuItemUpdateDTO.Id != id)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }

                    MenuItem menuItemFromDb = await _db.MenuItems.FindAsync(id);
                    if (menuItemFromDb == null)
                    {
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        return BadRequest();
                    }


                    menuItemFromDb.Name = menuItemUpdateDTO.Name;
                    menuItemFromDb.Price = menuItemUpdateDTO.Price;
                    menuItemFromDb.Category = menuItemUpdateDTO.Category;
                    menuItemFromDb.SpecialTag = menuItemUpdateDTO.SpecialTag;
                    menuItemFromDb.Description = menuItemUpdateDTO.Description;
                    menuItemFromDb.Image = menuItemFromDb.Image;

                    if (menuItemUpdateDTO.File != null && menuItemUpdateDTO.File.Length > 0)
                    {
                        await _imageService.DeleteImage(menuItemFromDb.Image);
                        string filename = $"{Guid.NewGuid()}{Path.GetExtension(menuItemUpdateDTO.File.FileName)}";
                        menuItemFromDb.Image = await _imageService.UploadImage(filename, menuItemUpdateDTO.File);
                    }

                    _db.MenuItems.Update(menuItemFromDb);
                    await _db.SaveChangesAsync();
                    _response.StatusCode = HttpStatusCode.NoContent;
                    //string image = Path.Combine("F:\\Projects\\Red_Mango\\red_mango_app\\src\\Assets\\Images\\foods", menuItemFromDb.Image);
                    //menuItemFromDb.Image = image.Replace("\\", "/");
                    _response.Result = menuItemFromDb;

                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return Ok(_response);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ApiResponse>> DeleteMenuItem(int id)
        {
            try
            {

                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest();
                }

                MenuItem menuItemFromDb = await _db.MenuItems.FindAsync(id);
                if (menuItemFromDb == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest();
                }
                await _imageService.DeleteImage(menuItemFromDb.Image);
                int millseconds = 2000;
                Thread.Sleep(millseconds);
                _db.MenuItems.Remove(menuItemFromDb);
                await _db.SaveChangesAsync();
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }
            return Ok(_response);
        }
    }
}
