using Moq;
using NRules.Rete;
using NUnit.Framework;

namespace NRules.Tests
{
    [TestFixture]
    public class AgendaTest
    {
        private EventAggregator _eventAggregator;

        [SetUp]
        public void Setup()
        {
            _eventAggregator = new EventAggregator();
        }

        [Test]
        public void Activate_NotCalled_ActivationQueueEmpty()
        {
            // Arrange
            // Act
            var target = CreateTarget();

            // Assert
            Assert.False(target.HasActiveRules());
        }

        [Test]
        public void Activate_Called_ActivationEndsUpInQueue()
        {
            // Arrange
            var ruleMock1 = new Mock<ICompiledRule>();
            var activation = new Activation(ruleMock1.Object, new Tuple());
            var target = CreateTarget();

            // Act
            _eventAggregator.Activate(activation);

            // Assert
            Assert.True(target.HasActiveRules());
            Assert.AreEqual(ruleMock1.Object, target.NextActivation().Rule);
        }

        [Test]
        public void Activate_CalledWithMultipleRules_RulesAreQueuedInOrder()
        {
            // Arrange
            var ruleMock1 = new Mock<ICompiledRule>();
            var ruleMock2 = new Mock<ICompiledRule>();
            var activation1 = new Activation(ruleMock1.Object, new Tuple());
            var activation2 = new Activation(ruleMock2.Object, new Tuple());
            var target = CreateTarget();

            // Act
            _eventAggregator.Activate(activation1);
            _eventAggregator.Activate(activation2);

            // Assert
            Assert.True(target.HasActiveRules());
            Assert.AreEqual(ruleMock1.Object, target.NextActivation().Rule);
            Assert.True(target.HasActiveRules());
            Assert.AreEqual(ruleMock2.Object, target.NextActivation().Rule);
        }

        private Agenda CreateTarget()
        {
            return new Agenda(_eventAggregator);
        }
    }
}