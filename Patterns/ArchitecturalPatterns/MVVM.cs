using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

// Source : https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm
//          https://en.wikipedia.org/wiki/Model-view-viewmodel
// MVVM separates the presentation state and behaviour (ViewModel) from the user interface
// (View) and from the business data (Model). The View knows the ViewModel, the ViewModel
// knows the Model - never the other way round. The only channels pointing back at the View
// are INotifyPropertyChanged (see Observer) and ICommand (see Command). That is precisely
// why a ViewModel can be exercised without any UI framework, as the tests do.

namespace Patterns.ArchitecturalPatterns
{
    /// <summary>
    /// The 'Model' - plain business data, no knowledge of any presentation concern.
    /// </summary>
    class Customer
    {
        public string Name { get; set; } = "";
        public decimal Balance { get; set; }
    }

    /// <summary>
    /// The 'Model' side service the ViewModel delegates to.
    /// </summary>
    interface ICustomerRepository
    {
        void Save(Customer customer);
    }

    class InMemoryCustomerRepository : ICustomerRepository
    {
        // What the repository was asked to store. Visible to the test project, because
        // this is the effect a command execution is supposed to have.
        internal List<Customer> Saved { get; } = new List<Customer>();

        public void Save(Customer customer)
        {
            Saved.Add(customer);
            Console.WriteLine("Saved customer {0} with balance {1}", customer.Name, customer.Balance);
        }
    }

    /// <summary>
    /// Base class of every ViewModel: the 'Subject' of the Observer pattern.
    /// </summary>
    abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the backing field and notifies - but only if the value really changed.
        /// </summary>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    /// <summary>
    /// The 'Command' the View binds to. Wraps a delegate pair instead of deriving a class per action.
    /// </summary>
    class RelayCommand : ICommand
    {
        private readonly Action _Execute;
        private readonly Func<bool>? _CanExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _Execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _CanExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _CanExecute == null || _CanExecute();

        public void Execute(object? parameter)
        {
            if (CanExecute(parameter))
                _Execute();
        }

        /// <summary>
        /// The ViewModel calls this whenever a property changed that CanExecute depends on.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// The 'ViewModel' - presentation state plus commands. Contains no UI type at all.
    /// </summary>
    class CustomerViewModel : ObservableObject
    {
        private readonly Customer _Model;
        private readonly ICustomerRepository _Repository;
        private bool _IsDirty;

        public CustomerViewModel(Customer model, ICustomerRepository repository)
        {
            _Model = model;
            _Repository = repository;
            SaveCommand = new RelayCommand(Save, CanSave);
        }

        public string Name
        {
            get => _Model.Name;
            set
            {
                if (_Model.Name == value)
                    return;

                _Model.Name = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
                IsDirty = true;
            }
        }

        public decimal Balance
        {
            get => _Model.Balance;
            set
            {
                if (_Model.Balance == value)
                    return;

                _Model.Balance = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayName));
                IsDirty = true;
            }
        }

        /// <summary>
        /// Derived property: formatting for the View, but usable without one.
        /// </summary>
        public string DisplayName => string.Format("{0} ({1:0.00})", Name, Balance);

        public bool IsDirty
        {
            get => _IsDirty;
            private set
            {
                if (SetProperty(ref _IsDirty, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand SaveCommand { get; }

        private bool CanSave() => IsDirty && !string.IsNullOrWhiteSpace(Name);

        private void Save()
        {
            _Repository.Save(_Model);
            IsDirty = false;
        }
    }

    /// <summary>
    /// The 'View'. In a real application this would be XAML with data bindings; here it is a
    /// console view that binds by hand. Note it only reads the ViewModel - it never touches the Model.
    /// </summary>
    class CustomerView
    {
        private readonly CustomerViewModel _ViewModel;

        // Every property the bindings refreshed, in order. Visible to the test project,
        // because "the View is updated automatically" is the claim being made here.
        internal List<string> RenderedProperties { get; } = new List<string>();

        public CustomerView(CustomerViewModel viewModel)
        {
            _ViewModel = viewModel;

            // This is what a XAML binding does under the hood.
            _ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            _ViewModel.SaveCommand.CanExecuteChanged += OnSaveCommandCanExecuteChanged;
        }

        public bool SaveButtonEnabled { get; private set; }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RenderedProperties.Add(e.PropertyName ?? "");
            Console.WriteLine("View refreshes '{0}' -> {1}", e.PropertyName, _ViewModel.DisplayName);
        }

        private void OnSaveCommandCanExecuteChanged(object? sender, EventArgs e)
        {
            SaveButtonEnabled = _ViewModel.SaveCommand.CanExecute(null);
            Console.WriteLine("Save button enabled: {0}", SaveButtonEnabled);
        }

        /// <summary>
        /// The user clicks the button - the View forwards to the command and knows nothing else.
        /// </summary>
        public void ClickSave()
        {
            _ViewModel.SaveCommand.Execute(null);
        }
    }

    /// <summary>
    /// MainApp startup class for the MVVM architectural pattern.
    /// </summary>
    public class MvvmExample
    {
        /// <summary>
        /// Entry point into console application.
        /// </summary>
        public static void Mvvm()
        {
            InMemoryCustomerRepository repository = new InMemoryCustomerRepository();
            CustomerViewModel viewModel = new CustomerViewModel(new Customer(), repository);
            CustomerView view = new CustomerView(viewModel);

            // Nothing has been edited yet, so the button the View shows is disabled and
            // a click does nothing at all.
            view.ClickSave();

            // The user types - the ViewModel notifies, the View re-renders itself.
            viewModel.Name = "Customer1";
            viewModel.Balance = 100.5m;

            view.ClickSave();
        }
    }
}
