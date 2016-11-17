namespace Common.Tests.Cqrs
{
    public class TestContext
    {
        public string SomeValue { get; }

        public TestContext(string someValue = null)
        {
            SomeValue = someValue;
        }
    }
}
