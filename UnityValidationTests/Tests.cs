using System;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnityValidation;

namespace UnityValidationTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void CustomerCannotBeNull()
        {
            var container = new UnityInitializer().Initialize(new UnityContainer());

            var badRequest = new ServiceRequest();
            var response = container.Resolve<IService>().GetSomething(badRequest);

            Assert.AreEqual(response.Code, ResponseCode.Error);
            Assert.AreEqual(response.Description, "Customer must be provided");
        }

        [TestMethod]
        public void CustomerCannotBeTest()
        {
            var container = new UnityInitializer().Initialize(new UnityContainer());

            var badRequest = new ServiceRequest {
                Customer = new Customer
                {
                    CustomerAccountId = 1,
                    TradingAccountId = 1,
                    UserId = 1
                }
            };
            var response = container.Resolve<IService>().GetSomething(badRequest);

            Assert.AreEqual(response.Code, ResponseCode.Error);
            Assert.AreEqual(response.Description, "Customer should not be test user");
        }
    }
}
