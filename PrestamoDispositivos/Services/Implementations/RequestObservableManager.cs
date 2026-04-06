using PrestamoDispositivos.Services.Abstractions;

namespace PrestamoDispositivos.Services.Implementations
{
    public class RequestObservableManager : IRequestObservable
    {
        private readonly List<Irequestobserver> _observers = new();
        private readonly object _lock = new();

        public void AddObserver(Irequestobserver observer)
        {
            lock (_lock)
            {
                if (!_observers.Contains(observer))
                    _observers.Add(observer);
            }
        }

        public void RemoveObserver(Irequestobserver observer)
        {
            lock (_lock)
            {
                _observers.Remove(observer);
            }
        }

        public async Task NotifyAll(Guid idSolicitud, string nuevoEstado, string mensaje)
        {
            List<Irequestobserver> snapshot;
            lock (_lock) { snapshot = new List<Irequestobserver>(_observers); }

            // Notificar a todos los observadores de forma paralela
            var tasks = snapshot.Select(o => o.Update(idSolicitud, nuevoEstado, mensaje));
            await Task.WhenAll(tasks);
        }
    }
}
