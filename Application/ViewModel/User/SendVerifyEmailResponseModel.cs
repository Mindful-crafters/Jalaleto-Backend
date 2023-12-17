namespace Application.ViewModel.User
{
    public class SendVerifyEmailResponseModel : ApiResponse
    {
        public SendVerifyEmailResponseModel(byte[] hash)
        {
            Success = true;
            Code = 200;
            Message = "Email sent";
            HashString = hash;
        }
        public byte[] HashString { get; set; }
    }
}
