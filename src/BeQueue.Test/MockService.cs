using System;

namespace BeQueue.Test
{
    public class MockService : IDisposable
    {
        public void Dispose()
        {
        }
    }

    public class MockServiceFactory : ExecutionServiceFactory<MockService>
    {
        public override MockService CreateService()
        {
            return new MockService();
        }

        public override bool ServiceReady(MockService service)
        {
            return true;
        }
    }
}