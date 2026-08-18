using Patterns.ArchitecturalPatterns;
using Xunit;

// Mirrors the library's Patterns.ArchitecturalPatterns namespace, which gives the
// Test Explorer a node to group the architectural patterns under.
namespace PatternTests.ArchitecturalPatterns
{
    public class MvvmTests
    {
        // Builds the triple the pattern is about and hands back all three parts.
        private static (CustomerViewModel ViewModel, CustomerView View, InMemoryCustomerRepository Repository) CreateScreen()
        {
            InMemoryCustomerRepository repository = new InMemoryCustomerRepository();
            CustomerViewModel viewModel = new CustomerViewModel(new Customer(), repository);

            return (viewModel, new CustomerView(viewModel), repository);
        }

        [Fact]
        public void Mvvm()
        {
            MvvmExample.Mvvm();
        }

        [Fact]
        public void EditingAPropertyRefreshesTheBoundView()
        {
            var (viewModel, view, _) = CreateScreen();

            viewModel.Name = "Customer1";

            // The View never asked - it was notified.
            Assert.Contains(nameof(CustomerViewModel.Name), view.RenderedProperties);
            Assert.Contains(nameof(CustomerViewModel.DisplayName), view.RenderedProperties);
        }

        [Fact]
        public void WritingTheSameValueAgainDoesNotRefreshTheView()
        {
            var (viewModel, view, _) = CreateScreen();
            viewModel.Name = "Customer1";
            int renderCount = view.RenderedProperties.Count;

            viewModel.Name = "Customer1";

            Assert.Equal(renderCount, view.RenderedProperties.Count);
        }

        [Fact]
        public void TheDerivedPropertyFormatsModelDataForTheView()
        {
            var (viewModel, _, _) = CreateScreen();

            viewModel.Name = "Customer1";
            viewModel.Balance = 100.5m;

            Assert.Equal("Customer1 (100.50)", viewModel.DisplayName);
        }

        [Fact]
        public void TheCommandStaysDisabledWhileThereIsNothingToSave()
        {
            var (viewModel, view, repository) = CreateScreen();

            view.ClickSave();

            Assert.False(viewModel.SaveCommand.CanExecute(null));
            Assert.False(view.SaveButtonEnabled);
            Assert.Empty(repository.Saved);
        }

        [Fact]
        public void EditingEnablesTheCommandAndExecutingItReachesTheModel()
        {
            var (viewModel, view, repository) = CreateScreen();

            viewModel.Name = "Customer1";
            viewModel.Balance = 100.5m;

            // The View learned about it through CanExecuteChanged, not by polling.
            Assert.True(view.SaveButtonEnabled);

            view.ClickSave();

            Customer saved = Assert.Single(repository.Saved);
            Assert.Equal("Customer1", saved.Name);
            Assert.Equal(100.5m, saved.Balance);
        }

        [Fact]
        public void AfterSavingThereIsNothingLeftToSave()
        {
            var (viewModel, view, _) = CreateScreen();
            viewModel.Name = "Customer1";

            view.ClickSave();

            Assert.False(viewModel.IsDirty);
            Assert.False(view.SaveButtonEnabled);
        }
    }
}
