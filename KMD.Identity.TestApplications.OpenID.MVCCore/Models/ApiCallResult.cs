namespace KMD.Identity.TestApplications.OpenID.MVCCore.Models
{
    public class ApiCallResult
    {
        public bool Success { get; set; }

        public string Error { get; set; }

        public bool HasError => !string.IsNullOrWhiteSpace(Error);
    }

    public class ApiCallResult<T> : ApiCallResult
    {
        public T Result { get; set; }
    }
}
