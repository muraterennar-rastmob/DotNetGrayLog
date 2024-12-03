using GrayLogTutorial.Application.Services.UserServices;
using Microsoft.AspNetCore.Mvc;

namespace GrayLogTutorial.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
  private readonly IUserService _userService;

  public UsersController(IUserService userService)
  {
    _userService = userService;
  }


  [HttpGet("GetAll")]
  public async Task<IActionResult> GetAll()
  {
    var response = await _userService.GetAllUsers();
    
    return Ok(response); 
  }
}