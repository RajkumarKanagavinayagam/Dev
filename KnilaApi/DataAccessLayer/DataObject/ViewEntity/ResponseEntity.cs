namespace KnilaApi.DataAccessLayer.DataObject.ViewEntity
{
    public class ResponseEntity<T>
    {
        public T? Result { get; set; }
        public List<T> ListResult { get; set; } = new List<T>();
        public bool IsSuccess { get; set; }
        public string? StatusMessage { get; set; }
        public int StatusCode { get; set; }
        public string? ResponseMessage { get; set; }
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
