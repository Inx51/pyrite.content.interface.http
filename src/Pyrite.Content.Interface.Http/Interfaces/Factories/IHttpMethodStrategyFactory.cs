namespace Pyrite.Content.Interface.Http.Interfaces.Factories
{
    public interface IHttpMethodStrategyFactory
    {
        public IHttpMethodStrategy Create(string method);
    }
}