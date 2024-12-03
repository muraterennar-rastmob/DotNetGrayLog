using System.Net.Http.Json;
using GrayLogTutorial.Core.Logs;
using GrayLogTutorial.Domain.Entities;

namespace GrayLogTutorial.Application.Services.UserServices;

public class UserService:IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CommonResponse<List<User>>> GetAllUsers()
    {
        var getAllUser = await _httpClient.GetFromJsonAsync<List<User>>("https://fakestoreapi.com/users");

        var response = new CommonResponse<List<User>>();
        response.ResultCode = 200;
        response.ResultMessage = "Success";
        response.Payload = getAllUser;
        
        return response;
    }
}