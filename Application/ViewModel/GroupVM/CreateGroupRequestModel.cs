using Microsoft.AspNetCore.Http;

namespace Application.ViewModel.GroupVM
{
    public class CreateGroupRequestModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile Image { get; set; }
        public List<string> InvitedEmails { get; set; }
    }
}
