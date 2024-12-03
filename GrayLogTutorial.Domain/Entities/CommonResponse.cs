namespace GrayLogTutorial.Domain.Entities;

public class CommonResponse<T>
{
    public int ResultCode { get; set; }
    public string ResultMessage { get; set; }
    public T Payload { get; set; }
}