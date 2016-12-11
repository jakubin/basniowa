using System.Threading.Tasks;
using Common.Cqrs;

namespace Logic.Tests.Common.Logging
{
    public class TestCommandHandler : IHandler<TestCommand>
    {
        public Task Handle(TestCommand message)
        {
            return Task.CompletedTask;
        }
    }
}