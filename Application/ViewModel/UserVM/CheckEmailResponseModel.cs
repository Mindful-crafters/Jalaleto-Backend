namespace Application.ViewModel.UserVM
{
    public class CheckEmailResponseModel : ApiResponse
    {
        public bool Exists { get; set; }
        public CheckEmailResponseModel(bool exists)
        {
            Success = true;
            Code = 200;
            Message = "success";
            Exists = exists;

        }
    }
}

