using Ecommerce529.LifeTime.InterfaceLifeTime;

namespace Ecommerce529.LifeTime.ClassLifeTime
{
    public class TransientClass : ITransientInterface
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}
