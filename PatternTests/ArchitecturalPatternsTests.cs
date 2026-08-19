using Patterns.ArchitecturalPatterns;
using Xunit;

// Mirrors the library's Patterns.ArchitecturalPatterns namespace, which gives the
// Test Explorer a node to group the architectural patterns under.
namespace PatternTests.ArchitecturalPatterns
{
    public class MvvmTests
    {
        [Fact]
        public void Mvvm()
        {
            MvvmExample.Mvvm();
        }
    }

    public class MvpTests
    {
        [Fact]
        public void Mvp()
        {
            MvpExample.Mvp();
        }
    }
}
