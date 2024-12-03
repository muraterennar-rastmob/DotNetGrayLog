using GrayLogTutorial.Domain.Entities;

namespace GrayLogTutorial.Application.Services.UserServices;

public interface IUserService
{
    Task<CommonResponse<List<User>>> GetAllUsers();
}