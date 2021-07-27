using Microsoft.Extensions.Logging;
using Moq;

namespace Application.UnitTests.Mocks
{
    public static class MockFakeLogger<T> where T : class
    {
        public static ILogger<T> FakeLogger()
        {
            return Mock.Of<ILogger<T>>();
        }
    }
}
