using Common.Cqrs;

namespace Common.Tests.Cqrs
{
    public sealed class TestCommand : ICommand
    {
        public string Text { get; }

        public TestCommand(string text = null)
        {
            Text = text;
        }
    }
}
