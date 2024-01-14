using Domain.Entities;

namespace Application.ViewModel.EventVM
{
    public class AddEventReviewResponseModel : ApiResponse
    {
        public EventReview[] EventReviews { get; set; }
        public AddEventReviewResponseModel(EventReview[] EventReviews)
        {
            Success = true;
            Code = 200;
            Message = "Events reveiews sent!";
            this.EventReviews = EventReviews;
        }
    }
}
