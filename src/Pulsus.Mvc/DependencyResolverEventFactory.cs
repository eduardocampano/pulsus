using System.Web.Mvc;

namespace Pulsus.Mvc
{
    public class DependencyResolverEventFactory : DefaultEventFactory
    {
        public override T Create<T>()
        {
            var dependencyResolver = DependencyResolver.Current;
            
            // WORKAROUND: this is because in ASP.NET MVC 4 the DefaultDependencyResolver will try to use
            // Activator.CreateInstance if the type is not abstract and not an interface  
            if ((dependencyResolver.GetType().FullName ?? string.Empty).Equals("System.Web.Mvc.DependencyResolver+DefaultDependencyResolver"))
                return base.Create<T>();

            var instance = dependencyResolver.GetService<T>();

            if (instance == null)
                instance = base.Create<T>();

            return instance;
        }
    }
}
