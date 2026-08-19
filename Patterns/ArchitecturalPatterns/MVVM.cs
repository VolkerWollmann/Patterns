using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

// Source : https://learn.microsoft.com/en-us/dotnet/architecture/maui/mvvm
//          https://en.wikipedia.org/wiki/Model-view-viewmodel
// MVVM separates the presentation state and behaviour (ViewModel) from the user interface
// (View) and from the business data (Model). The View knows the ViewModel, the ViewModel
// knows the Model - never the other way round. The only channels pointing back at the View
// are INotifyPropertyChanged (see Observer) and ICommand (see Command). That is precisely
// why a ViewModel can be exercised without any UI framework, as MvvmExample below does.

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
        // What the repository was asked to store - the effect a command execution
        // is supposed to have.
        public List<Customer> Saved { get; } = new List<Customer>();

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
                MarkDirty();
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
                MarkDirty();
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

        /// <summary>
        /// An edit happened. CanSave also looks at Name, so re-evaluating the command only
        /// when IsDirty flips would leave the View showing a stale button.
        /// </summary>
        private void MarkDirty()
        {
            IsDirty = true;
            SaveCommand.RaiseCanExecuteChanged();
        }

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

        private readonly string _Title;

        // Every property the bindings refreshed, in order - "the View is updated
        // automatically" is the claim being made here, so it is written down.
        public List<string> RenderedProperties { get; } = new List<string>();

        public CustomerView(CustomerViewModel viewModel, string title = "View")
        {
            _ViewModel = viewModel;
            _Title = title;

            // This is what a XAML binding does under the hood.
            _ViewModel.PropertyChanged += OnViewModelPropertyChanged;
            _ViewModel.SaveCommand.CanExecuteChanged += OnSaveCommandCanExecuteChanged;
        }

        public bool SaveButtonEnabled { get; private set; }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RenderedProperties.Add(e.PropertyName ?? "");
            Console.WriteLine("{0} refreshes '{1}' -> {2}", _Title, e.PropertyName, _ViewModel.DisplayName);
        }

        private void OnSaveCommandCanExecuteChanged(object? sender, EventArgs e)
        {
            SaveButtonEnabled = _ViewModel.SaveCommand.CanExecute(null);
            Console.WriteLine("{0}: save button enabled: {1}", _Title, SaveButtonEnabled);
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
            Customer model = new Customer();
            CustomerViewModel viewModel = new CustomerViewModel(model, repository);
            CustomerView view = new CustomerView(viewModel);

            // 1. Nothing has been edited yet. The command reports that it cannot run, so
            //    the View disables its button - and a click really does nothing.
            Debug.Assert(!viewModel.SaveCommand.CanExecute(null), "an untouched form has nothing to save");
            view.ClickSave();
            Debug.Assert(repository.Saved.Count == 0, "a disabled command must not reach the model");

            // 2. The user types. The ViewModel writes through to the Model and notifies;
            //    the View re-renders without ever having asked.
            viewModel.Name = "Customer1";
            viewModel.Balance = 100.5m;

            Debug.Assert(model.Name == "Customer1", "the ViewModel writes through to the Model");
            Debug.Assert(view.RenderedProperties.Contains(nameof(CustomerViewModel.Name)));

            // 3. A derived property is refreshed along with the one it is computed from -
            //    the ViewModel has to say so explicitly, nobody works that out for it.
            Debug.Assert(view.RenderedProperties.Contains(nameof(CustomerViewModel.DisplayName)));
            Debug.Assert(viewModel.DisplayName == "Customer1 (100.50)", "formatting belongs to the ViewModel");

            // 4. Writing the same value again changes nothing, so no notification is sent.
            //    Without this guard, bindings notify in circles.
            int renderCount = view.RenderedProperties.Count;
            viewModel.Name = "Customer1";
            Debug.Assert(view.RenderedProperties.Count == renderCount, "an unchanged value must stay quiet");

            // 5. The edit made the command executable, and the View learned about it
            //    through CanExecuteChanged instead of polling.
            Debug.Assert(view.SaveButtonEnabled, "an edited, valid form can be saved");

            // 6. Validation lives in the ViewModel, not in the View: an empty name
            //    disables the button again.
            viewModel.Name = "";
            Debug.Assert(!view.SaveButtonEnabled, "an invalid form cannot be saved");
            viewModel.Name = "Customer1";

            // 7. The click reaches the Model through the command - the View knows neither
            //    the repository nor the Customer.
            view.ClickSave();
            Debug.Assert(repository.Saved.Count == 1);
            Debug.Assert(ReferenceEquals(repository.Saved[0], model), "it is the very Model the ViewModel wraps");

            // 8. After saving there is nothing left to save, and the button goes back down.
            Debug.Assert(!viewModel.IsDirty);
            Debug.Assert(!view.SaveButtonEnabled);

            // 9. The ViewModel holds no reference to any View, so a second one can bind to
            //    the same instance and both are served. That is also why a test - or a
            //    console, as here - can stand in for the real user interface.
            CustomerView secondView = new CustomerView(viewModel, "Second view");
            viewModel.Balance = 250m;

            Debug.Assert(secondView.RenderedProperties.Contains(nameof(CustomerViewModel.Balance)));
            Debug.Assert(view.RenderedProperties.Contains(nameof(CustomerViewModel.Balance)));
            Debug.Assert(secondView.SaveButtonEnabled && view.SaveButtonEnabled);
        }
    }
}
