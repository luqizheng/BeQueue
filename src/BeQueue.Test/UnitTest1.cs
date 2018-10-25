using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeQueue.Test
{
    [TestClass]
    public class TestInjectService
    {
        [TestMethod]
        public void TestMethod1()
        {
            IServiceCollection service = new ServiceCollection();
            service.AddBeQueueSerivce<MockServiceFactory, MockService>();

            var sp = service.BuildServiceProvider(true);
            Assert.IsNotNull(sp.GetService<ExecutionPool<MockServiceFactory, MockService>>());

            Assert.IsNotNull(sp.GetService<MockServiceFactory>());
        }
    }
}