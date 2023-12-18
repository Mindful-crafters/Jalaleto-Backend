using System.ComponentModel.DataAnnotations;

namespace Application.ViewModel.UserVM
{
    public class ResetPasswordRequestModel
    {
        [EmailAddress]
        public string Mail { get; set; } = string.Empty;
        public int Code { get; set; }
        public string NewPassword { get; set; } = string.Empty;
        public required byte[] HashString { get; set; }
    }
}
