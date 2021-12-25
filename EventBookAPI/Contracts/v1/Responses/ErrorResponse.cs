namespace EventBookAPI.Contracts.v1.Responses;

public class ErrorResponse
{
    public List<ErrorModel> Errors { get; set; } = new ();
}