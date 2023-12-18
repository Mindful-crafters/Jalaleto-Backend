namespace Application.ViewModel.UserVM
{
    public class LoginResponseModel : ApiResponse
    {
        public LoginResponseModel(string token)
        {
            Success = true;
            Code = 200;
            Message = "Logged in";
            Token = token;
        }
        public string Token { get; set; }
    }
}
