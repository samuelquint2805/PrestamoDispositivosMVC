namespace PrestamoDispositivos.Services.Abstractions
{
    public interface Irequestobserver
    {
        /// <summary>
        /// Llamado cuando el estado de una solicitud cambia.
        /// </summary>
        /// <param name="idSolicitud">ID de la solicitud afectada.</param>
        /// <param name="nuevoEstado">Nuevo estado: Pendiente, Aprobada, Rechazada.</param>
        /// <param name="mensaje">Mensaje descriptivo del cambio.</param>
        Task Update(Guid idSolicitud, string nuevoEstado, string mensaje);
    }

    /// <summary>
    /// Sujeto observable: gestiona la lista de observadores y notifica cambios.
    /// </summary>
    public interface IRequestObservable
    {
        void AddObserver(Irequestobserver observer);
        void RemoveObserver(Irequestobserver observer);
        Task NotifyAll(Guid idSolicitud, string nuevoEstado, string mensaje);
    }
}

