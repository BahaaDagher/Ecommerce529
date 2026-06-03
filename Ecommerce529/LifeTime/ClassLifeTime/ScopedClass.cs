using Ecommerce529.LifeTime.InterfaceLifeTime;

namespace Ecommerce529.LifeTime.ClassLifeTime
{
    public class ScopedClass : IScopedInterface
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}
