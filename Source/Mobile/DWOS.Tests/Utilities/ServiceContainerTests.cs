using DWOS.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DWOS.Tests.Utilities
{
    [TestClass]
    public class ServiceContainerTests
    {
        [TestMethod]
        public void ContainerTest()
        {
            ServiceContainer.Register<IService>(() => new ServiceImplementation());
            var actualService = ServiceContainer.Resolve<IService>();
            Assert.IsNotNull(actualService);
            Assert.AreEqual("Service", actualService.Name);
        }

        public interface IService
        {
            string Name { get; }
        }

        public class ServiceImplementation : IService
        {
            public string Name => "Service";
        }
    }
}
