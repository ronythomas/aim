namespace Client
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public T Result { get; set; }

        public ApiResponse(bool isSuccess, T result = default(T), string errorMessage = null)
        {
            IsSuccess = isSuccess;
            Result = result;
            ErrorMessage = errorMessage;
        }
    }
}