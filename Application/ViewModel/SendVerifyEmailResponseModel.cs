namespace Application.ViewModel
{
    public class SendVerifyEmailResponseModel : ApiResponse
    {
        public SendVerifyEmailResponseModel(byte[] hash)
        {
            this.Success = true;
            this.Code = 200;
            this.Message = "Email sent";
            this.HashString = hash;
        }
        public byte[] HashString { get; set; }
    }
}
