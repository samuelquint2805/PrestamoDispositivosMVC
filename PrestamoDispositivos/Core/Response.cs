namespace PrestamoDispositivos.Core
{
    public class Response<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();
        public T? Result { get; set; }

        
        public Response() { }

        // Constructor para respuestas exitosas
        public Response(T result, string message = "Operación exitosa")
        {
            IsSuccess = true;
            Message = message;
            Result = result;
        }

        // Constructor para respuestas de error
        public Response(string message, List<string>? errors = null)
        {
            IsSuccess = false;
            Message = message;
            Errors = errors ?? new List<string>();
        }
    }
}
