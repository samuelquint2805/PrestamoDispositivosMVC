namespace PrestamoDispositivos.Core
{
    public class Response<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();
        public T? Result { get; set; }

    }
}
