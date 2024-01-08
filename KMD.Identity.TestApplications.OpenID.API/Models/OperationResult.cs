namespace KMD.Identity.TestApplications.OpenID.API.Models
{
    public class OperationResult
    {
        public bool Success { get; set; }

        public string Error { get; set; }

        public static OperationResult Fail(string error)
        {
            return new OperationResult { Success = false, Error = error };
        }

        public static OperationResult Pass()
        {
            return new OperationResult { Success = true };
        }
    }

    public class OperationResult<T>
    {
        public bool Success { get; set; }

        public string Error { get; set; }

        public T Result { get; set; }

        public static OperationResult<T> Fail(string error)
        {
            return new OperationResult<T> { Success = false, Error = error };
        }

        public static OperationResult<T> Pass(T result)
        {
            return new OperationResult<T> { Success = true, Result = result};
        }
    }
}
