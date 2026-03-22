using System.Linq;
using System.Reflection;
using ExampleAPI.Services;
using FluentAssertions;
using Microsoft.Extensions.ObjectPool;
using Moq;
using RabbitMQ.Client;
using Xunit;

namespace ExampleAPI.Tests.Services
{
    public class RabbitManagerTests
    {
        private readonly Mock<IPooledObjectPolicy<IModel>> _mockPolicy;
        private readonly Mock<IModel> _mockChannel;

        public RabbitManagerTests()
        {
            _mockChannel = new Mock<IModel>();
            _mockPolicy = new Mock<IPooledObjectPolicy<IModel>>();
            _mockPolicy.Setup(p => p.Create()).Returns(_mockChannel.Object);
            _mockPolicy.Setup(p => p.Return(It.IsAny<IModel>())).Returns(true);
        }

        [Fact]
        public void RabbitManager_ShouldImplementIDisposable()
        {
            var manager = new RabbitManager(_mockPolicy.Object);

            manager.Should().BeAssignableTo<IDisposable>();
        }

        [Fact]
        public void Dispose_ShouldDisposePolicy_WhenPolicyIsDisposable()
        {
            var disposablePolicy = new Mock<IPooledObjectPolicy<IModel>>();
            var disposableMock = disposablePolicy.As<IDisposable>();
            var manager = new RabbitManager(disposablePolicy.Object);

            manager.Dispose();

            disposableMock.Verify(d => d.Dispose(), Times.Once);
        }

        [Fact]
        public void Dispose_ShouldBeIdempotent()
        {
            var disposablePolicy = new Mock<IPooledObjectPolicy<IModel>>();
            var disposableMock = disposablePolicy.As<IDisposable>();
            var manager = new RabbitManager(disposablePolicy.Object);

            manager.Dispose();
            manager.Dispose();

            disposableMock.Verify(d => d.Dispose(), Times.Once);
        }

        [Fact]
        public void Publish_WithNullMessage_ShouldNotPublish()
        {
            var manager = new RabbitManager(_mockPolicy.Object);

            manager.Publish<string>(null, "test-queue");

            _mockChannel.Verify(c => c.BasicPublish(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<IBasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>()),
                Times.Never);
        }

        [Fact]
        public void Publish_ShouldDeclareQueue()
        {
            var manager = new RabbitManager(_mockPolicy.Object);
            var props = new Mock<IBasicProperties>();
            _mockChannel.Setup(c => c.CreateBasicProperties()).Returns(props.Object);

            manager.Publish(new { Name = "test" }, "my-queue");

            _mockChannel.Verify(c => c.QueueDeclare("my-queue", true, false, false, null), Times.Once);
        }

        [Fact]
        public void Publish_ShouldSetPersistentProperty()
        {
            var manager = new RabbitManager(_mockPolicy.Object);
            var props = new Mock<IBasicProperties>();
            _mockChannel.Setup(c => c.CreateBasicProperties()).Returns(props.Object);

            manager.Publish(new { Name = "test" }, "my-queue");

            props.VerifySet(p => p.Persistent = true);
        }

        [Fact]
        public void Publish_ShouldPublishToCorrectQueue()
        {
            var manager = new RabbitManager(_mockPolicy.Object);
            var props = new Mock<IBasicProperties>();
            _mockChannel.Setup(c => c.CreateBasicProperties()).Returns(props.Object);

            manager.Publish(new { Name = "test" }, "my-queue");

            _mockChannel.Verify(c => c.BasicPublish(
                "",
                "my-queue",
                false,
                props.Object,
                It.IsAny<ReadOnlyMemory<byte>>()),
                Times.Once);
        }

        [Fact]
        public void Publish_ShouldSerializeMessageAsJson()
        {
            var manager = new RabbitManager(_mockPolicy.Object);
            var props = new Mock<IBasicProperties>();
            _mockChannel.Setup(c => c.CreateBasicProperties()).Returns(props.Object);

            ReadOnlyMemory<byte> capturedBody = default;
            _mockChannel.Setup(c => c.BasicPublish(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<IBasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>()))
                .Callback<string, string, bool, IBasicProperties, ReadOnlyMemory<byte>>(
                    (exchange, routingKey, mandatory, properties, body) => capturedBody = body);

            manager.Publish(new { Name = "test" }, "my-queue");

            var bodyString = System.Text.Encoding.UTF8.GetString(capturedBody.ToArray());
            bodyString.Should().Contain("\"Name\"");
            bodyString.Should().Contain("\"test\"");
        }

        [Fact]
        public void Publish_ShouldNotContainCatchBlock_SoStackTraceIsPreserved()
        {
            // Verify the Publish method does not have a catch block (which could use 'throw ex;' and destroy stack traces).
            // The method should either use 'throw;' in a catch or have no catch at all (only try/finally).
            var publishMethod = typeof(RabbitManager)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .First(m => m.Name == "Publish");

            var methodBody = publishMethod.GetMethodBody();
            var il = methodBody.ExceptionHandlingClauses;

            // Ensure there are no catch clauses — only finally clauses are acceptable
            var catchClauses = il.Where(c => c.Flags == ExceptionHandlingClauseOptions.Clause).ToList();
            catchClauses.Should().BeEmpty(
                "Publish() should not have a catch block; use try/finally to preserve stack traces");
        }

        [Fact]
        public void Publish_WhenExceptionThrown_ShouldPreserveOriginalException()
        {
            var manager = new RabbitManager(_mockPolicy.Object);
            _mockChannel.Setup(c => c.CreateBasicProperties())
                .Throws(new InvalidOperationException("channel error"));

            var act = () => manager.Publish(new { Name = "test" }, "my-queue");

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("channel error");
        }

        [Fact]
        public void Publish_ShouldReturnChannelToPool_OnSuccess()
        {
            var manager = new RabbitManager(_mockPolicy.Object);
            var props = new Mock<IBasicProperties>();
            _mockChannel.Setup(c => c.CreateBasicProperties()).Returns(props.Object);

            manager.Publish(new { Name = "test" }, "my-queue");

            _mockPolicy.Verify(p => p.Return(_mockChannel.Object), Times.Once);
        }

        [Fact]
        public void Publish_ShouldReturnChannelToPool_WhenExceptionThrown()
        {
            var manager = new RabbitManager(_mockPolicy.Object);
            _mockChannel.Setup(c => c.CreateBasicProperties())
                .Throws(new InvalidOperationException("channel error"));

            try { manager.Publish(new { Name = "test" }, "my-queue"); } catch { }

            _mockPolicy.Verify(p => p.Return(_mockChannel.Object), Times.Once);
        }
    }

    public class RabbitModelPooledObjectPolicyTests
    {
        [Fact]
        public void Return_ShouldReturnTrue_WhenChannelIsOpen()
        {
            var policy = CreatePolicyWithMockConnection(out _);
            var mockChannel = new Mock<IModel>();
            mockChannel.Setup(c => c.IsOpen).Returns(true);

            var result = policy.Return(mockChannel.Object);

            result.Should().BeTrue();
            mockChannel.Verify(c => c.Dispose(), Times.Never);
        }

        [Fact]
        public void Return_ShouldDisposeClosedChannel_AndReturnFalse()
        {
            var policy = CreatePolicyWithMockConnection(out _);
            var mockChannel = new Mock<IModel>();
            mockChannel.Setup(c => c.IsOpen).Returns(false);

            var result = policy.Return(mockChannel.Object);

            result.Should().BeFalse();
            mockChannel.Verify(c => c.Dispose(), Times.Once);
        }

        [Fact]
        public void Dispose_ShouldCleanUpChannelPool_ViaFullDisposalChain()
        {
            // Verifies the full chain: RabbitManager.Dispose() → policy.Dispose() → connection closed & disposed
            var policy = CreatePolicyWithMockConnection(out var mockConnection);
            var manager = new RabbitManager(policy);

            manager.Dispose();

            mockConnection.Verify(c => c.Close(), Times.Once);
            mockConnection.Verify(c => c.Dispose(), Times.Once);
        }

        [Fact]
        public void Dispose_ShouldCloseAndDisposeConnection()
        {
            var policy = CreatePolicyWithMockConnection(out var mockConnection);

            policy.Dispose();

            mockConnection.Verify(c => c.Close(), Times.Once);
            mockConnection.Verify(c => c.Dispose(), Times.Once);
        }

        [Fact]
        public void Dispose_ShouldBeIdempotent_ConnectionClosedOnce()
        {
            var policy = CreatePolicyWithMockConnection(out var mockConnection);

            policy.Dispose();
            policy.Dispose();

            mockConnection.Verify(c => c.Close(), Times.Once);
            mockConnection.Verify(c => c.Dispose(), Times.Once);
        }

        private static RabbitModelPooledObjectPolicy CreatePolicyWithMockConnection(out Mock<IConnection> mockConnection)
        {
            var policy = (RabbitModelPooledObjectPolicy)System.Runtime.Serialization.FormatterServices
                .GetUninitializedObject(typeof(RabbitModelPooledObjectPolicy));

            mockConnection = new Mock<IConnection>();
            var connectionField = typeof(RabbitModelPooledObjectPolicy)
                .GetField("_connection", BindingFlags.NonPublic | BindingFlags.Instance);
            connectionField.SetValue(policy, mockConnection.Object);

            return policy;
        }
    }
}
