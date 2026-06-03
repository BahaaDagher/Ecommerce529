using Ecommerce529.LifeTime.InterfaceLifeTime;

namespace Ecommerce529.LifeTime.ClassLifeTime
{
    public class SingletonClass : ISingletonInterface
    {
        public Guid Id { get;   } = Guid.NewGuid();
    }
}
