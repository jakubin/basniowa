using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Cqrs
{
    /// <summary>
    /// Resolves command and event handlers using <see cref="IServiceProvider"/>.
    /// </summary>
    public class ServiceProviderHandlerResolver : IHandlerResolver
    {
        private IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceProviderHandlerResolver"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ServiceProviderHandlerResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public IList<IHandler<T>> Resolve<T>() where T : IMessage
        {
            return _serviceProvider.GetServices<IHandler<T>>().ToList();
        }
    }
}
