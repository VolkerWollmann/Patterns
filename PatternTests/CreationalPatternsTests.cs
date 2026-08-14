using Patterns.CreationalPatterns;
// Needed as a plain using, not just the alias: extension methods are only found
// through an imported namespace.
using Patterns.CreationalPatterns.ExtensionMethodVsConstructor;
using PCE = Patterns.CreationalPatterns.ExtensionMethodVsConstructor;
using Xunit;

namespace PatternTests
{
    public class CreationalPatternsTests
    {
        [Fact]
        public void AbstractFactory()
        {
            AbstractFactoryExample.AbstractFactory();
        }

        [Fact]
        public void FactoryMethod()
        {
            FactoryMethodExample.Test();
        }

        [Fact]
        public void ExtensionMethodVsConstructor()
        {
            PCE.ExtensionMethodVsConstructorExample.Example();
        }

        [Fact]
        public void Singleton()
        {
            SingletonExample.Example();
        }

        [Fact]
        public void Builder()
        {
            BuilderExample.Example();
        }

        [Fact]
        public void Prototype()
        {
            PrototypeExample.Test();
        }

        [Fact]
        public void ADeepCopyIsIndependentOfTheOriginal()
        {
            PrototypeExample.Person original = new PrototypeExample.Person
            {
                Age = 42,
                Name = "Sam",
                IdInfo = new PrototypeExample.IdInfo(6565)
            };

            PrototypeExample.Person copy = original.DeepCopy();

            Assert.Equal(original.Name, copy.Name);
            Assert.Equal(original.Age, copy.Age);
            Assert.Equal(original.IdInfo.IdNumber, copy.IdInfo.IdNumber);

            original.Name = "George";
            original.Age = 39;
            original.IdInfo.IdNumber = 8641;

            // Nothing of the original bleeds into the copy - not even IdInfo,
            // which is a reference.
            Assert.Equal("Sam", copy.Name);
            Assert.Equal(42, copy.Age);
            Assert.Equal(6565, copy.IdInfo.IdNumber);
        }

        [Fact]
        public void AShallowCopyKeepsSharingItsReferences()
        {
            PrototypeExample.Person original = new PrototypeExample.Person
            {
                Age = 42,
                Name = "Sam",
                IdInfo = new PrototypeExample.IdInfo(6565)
            };

            PrototypeExample.Person copy = original.ShallowCopy();

            original.Age = 39;
            original.IdInfo.IdNumber = 8641;

            // The value type is copied ...
            Assert.Equal(42, copy.Age);

            // ... but both still point at the same IdInfo. That is the difference
            // the pattern is about.
            Assert.Equal(8641, copy.IdInfo.IdNumber);
        }

        [Fact]
        public void ConstructorAndExtensionMethodsBuildTheSameObject()
        {
            PCE.SampleClass byConstructor = new PCE.SampleClass(1, 2);
            PCE.SampleClass byExtensionMethods = new PCE.SampleClass().WithP1(1).WithP2(2);

            Assert.Equal(byConstructor._p1, byExtensionMethods._p1);
            Assert.Equal(byConstructor._p2, byExtensionMethods._p2);
            Assert.Equal(2, byExtensionMethods._p2);
        }
    }
}
