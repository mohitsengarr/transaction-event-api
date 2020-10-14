using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage.Files.Shares;
using Glasswall.Administration.K8.TransactionEventApi;
using Glasswall.Administration.K8.TransactionEventApi.Business.Store;
using Glasswall.Administration.K8.TransactionEventApi.Common.Configuration;
using Glasswall.Administration.K8.TransactionEventApi.Common.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using NUnit.Framework;
using TestCommon;

namespace TransactionEventApi.Tests.StartupTests
{
    [TestFixture]
    public class WhenUsingStartup : UnitTestBase<Startup>
    {
        [OneTimeSetUp]
        public void Setup()
        {
            ClassInTest = new Startup(Mock.Of<IConfiguration>());
        }

        [Test]
        public void Can_Resolve_Transaction_Service()
        {
            var services = new ServiceCollection();
            
            ClassInTest.ConfigureServices(services);

            Assert.That(services.Any(s =>
                s.ServiceType == typeof(IEnumerable<ShareClient>)), "No share client was added");

            Assert.That(services.Any(s =>
                s.ServiceType == typeof(IEnumerable<IFileShare>)), "No file store was added");

            services.Replace(new ServiceDescriptor(typeof(IEnumerable<ShareClient>),
                new [] { Mock.Of<ShareClient>() }));

            services.BuildServiceProvider().GetRequiredService<ITransactionService>();
        }

        [Test]
        public void Configuration_Can_Be_Parsed()
        {
            var services = new ServiceCollection();

            ClassInTest.ConfigureServices(services);

            Environment.SetEnvironmentVariable(nameof(ITransactionEventApiConfiguration.TransactionStoreConnectionStringCsv), "string1,string2");
            Environment.SetEnvironmentVariable(nameof(ITransactionEventApiConfiguration.ShareName), "transactions");

            var config = services.BuildServiceProvider().GetRequiredService<ITransactionEventApiConfiguration>();

            Assert.That(config.ShareName, Is.EqualTo("transactions"));
            Assert.That(config.TransactionStoreConnectionStringCsv, Is.EqualTo("string1,string2"));
        }

        [Test]
        public void Constructor_Constructs_With_Mocked_Params()
        {
            ConstructorAssertions.ConstructsWithMockedParameters<Startup>();
        }

        [Test]
        public void Constructor_Is_Guarded_Against_Null()
        {
            ConstructorAssertions.ClassIsGuardedAgainstNull<Startup>();
        }
    }
}
