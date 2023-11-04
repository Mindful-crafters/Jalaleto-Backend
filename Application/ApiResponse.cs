namespace Application
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }

        public static ApiResponse Ok(string msg = "success")
        {
            return new ApiResponse()
            {
                Success = true,
                Code = 200,
                Message = msg,
            };
        }


    }
}
