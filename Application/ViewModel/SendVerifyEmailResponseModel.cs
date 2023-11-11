namespace Application.ViewModel
{
    public class SendVerifyEmailResponseModel : ApiResponse
    {
        public SendVerifyEmailResponseModel(string hash)
        {
            this.Success = true;
            this.Code = 200;
            this.Message = "Email sent";
            this.HashString = hash;
        }
        public string HashString { get; set; } = string.Empty;
    }
}
