namespace Application.ViewModel.UserVM
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
