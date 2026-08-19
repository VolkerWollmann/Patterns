using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

// Source : https://en.wikipedia.org/wiki/Model-view-presenter
//          https://martinfowler.com/eaaDev/PassiveScreen.html
// MVP splits the same three concerns as MVVM, but wires the top two together the other way
// round: the Presenter holds a reference to the View - through an interface - and pushes
// every value into it by hand. There is no INotifyPropertyChanged and no ICommand, because
// there is no binding engine to talk to. That is exactly why MVP is the pattern for UI
// toolkits without data binding (WinForms, Android views, GWT), while MVVM needs one.
//
// The flavour shown here is 'Passive View': the View knows nothing at all, it only holds
// text and raises events. The other flavour, 'Supervising Controller', lets the View bind
// simple values itself and leaves only the complex logic to the Presenter.
//
// Model and repository are the ones from MVVM.cs on purpose - the Model does not change
// when the presentation layer does.

namespace Patterns.ArchitecturalPatterns
{
    /// <summary>
    /// The 'View' contract. Note it speaks nothing but strings and bools: no Customer, no
    /// decimal, no domain type at all. The Presenter talks to this, never to a concrete
    /// control - which is what makes a console stand-in as good as a real window.
    /// </summary>
    interface ICustomerScreen
    {
        string Name { get; set; }

        // The balance as typed, not as a number. Parsing it is presentation logic and
        // therefore the Presenter's job.
        string Balance { get; set; }

        bool SaveEnabled { get; set; }

        string Status { get; set; }

        /// <summary>The user changed something.</summary>
        event EventHandler? Edited;

        /// <summary>The user pressed the save button.</summary>
        event EventHandler? SaveClicked;
    }

    /// <summary>
    /// The 'View'. A passive one: it contains not a single decision. In a real application
    /// this would be a WinForms form whose controls are wired to these events.
    /// </summary>
    class ConsoleCustomerScreen : ICustomerScreen
    {
        private string _Name = "";
        private string _Balance = "";
        private string _Status = "";
        private bool _SaveEnabled;

        // Everything the Presenter pushed into this screen, in order. "The Presenter
        // updates the View explicitly" is the claim of the pattern, so it is written down.
        public List<string> Updates { get; } = new List<string>();

        public string Name
        {
            get => _Name;
            set { _Name = value; Record(nameof(Name), value); }
        }

        public string Balance
        {
            get => _Balance;
            set { _Balance = value; Record(nameof(Balance), value); }
        }

        public string Status
        {
            get => _Status;
            set { _Status = value; Record(nameof(Status), value); }
        }

        public bool SaveEnabled
        {
            get => _SaveEnabled;
            set { _SaveEnabled = value; Record(nameof(SaveEnabled), value.ToString()); }
        }

        public event EventHandler? Edited;
        public event EventHandler? SaveClicked;

        // The user typing. The control changes by itself - no push is involved, so this
        // writes the field directly. Going through the property would make the screen
        // record its own input as a Presenter update, and in a real toolkit it is exactly
        // this distinction that keeps a programmatic update from looping back as an edit.
        public void TypeName(string text)
        {
            _Name = text;
            Console.WriteLine("User types name: '{0}'", text);
            Edited?.Invoke(this, EventArgs.Empty);
        }

        public void TypeBalance(string text)
        {
            _Balance = text;
            Console.WriteLine("User types balance: '{0}'", text);
            Edited?.Invoke(this, EventArgs.Empty);
        }

        public void ClickSave()
        {
            Console.WriteLine("User clicks save");
            SaveClicked?.Invoke(this, EventArgs.Empty);
        }

        private void Record(string property, string value)
        {
            Updates.Add(property);
            Console.WriteLine("Presenter sets {0} = '{1}'", property, value);
        }
    }

    /// <summary>
    /// The 'Presenter'. Holds the View behind its interface and drives it. All the decisions
    /// live here: what is valid, what the button does, how a number becomes text.
    /// </summary>
    class CustomerPresenter
    {
        private readonly ICustomerScreen _Screen;
        private readonly Customer _Model;
        private readonly ICustomerRepository _Repository;
        private bool _IsDirty;

        public CustomerPresenter(ICustomerScreen screen, Customer model, ICustomerRepository repository)
        {
            _Screen = screen;
            _Model = model;
            _Repository = repository;

            _Screen.Edited += OnEdited;
            _Screen.SaveClicked += OnSaveClicked;

            ShowModelOnScreen();
        }

        /// <summary>
        /// The initial push. Nobody else fills the screen - there is no binding to do it.
        /// </summary>
        private void ShowModelOnScreen()
        {
            _Screen.Name = _Model.Name;
            _Screen.Balance = _Model.Balance.ToString("0.00", CultureInfo.InvariantCulture);
            _Screen.Status = "";
            _Screen.SaveEnabled = false;
        }

        private void OnEdited(object? sender, EventArgs e)
        {
            _IsDirty = true;
            Validate();
        }

        /// <summary>
        /// Validation belongs to the Presenter, not to the View - which is why it can be
        /// checked without a user interface.
        /// </summary>
        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(_Screen.Name))
            {
                _Screen.Status = "A customer needs a name.";
                _Screen.SaveEnabled = false;
                return false;
            }

            if (!TryReadBalance(out _))
            {
                _Screen.Status = "The balance is not a number.";
                _Screen.SaveEnabled = false;
                return false;
            }

            _Screen.Status = "";
            _Screen.SaveEnabled = _IsDirty;
            return true;
        }

        private bool TryReadBalance(out decimal balance)
        {
            return decimal.TryParse(
                _Screen.Balance,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out balance);
        }

        private void OnSaveClicked(object? sender, EventArgs e)
        {
            // Re-check instead of trusting the button: a disabled control is a hint to the
            // user, never a guarantee for the Presenter.
            if (!_IsDirty || !Validate())
                return;

            // Only now does the Model learn about the edit - until here the screen held it.
            // That is a decision of this example, not a law of the pattern.
            _Model.Name = _Screen.Name;
            TryReadBalance(out decimal balance);
            _Model.Balance = balance;

            _Repository.Save(_Model);
            _IsDirty = false;

            // Redisplay from the Model: whatever the user typed now comes back the way the
            // Presenter formats it. Without a binding engine, this refresh is a method call.
            ShowModelOnScreen();
            _Screen.Status = "Saved.";
        }
    }

    /// <summary>
    /// MainApp startup class for the MVP architectural pattern.
    /// </summary>
    public class MvpExample
    {
        /// <summary>
        /// Entry point into console application.
        /// </summary>
        public static void Mvp()
        {
            InMemoryCustomerRepository repository = new InMemoryCustomerRepository();
            Customer model = new Customer();
            ConsoleCustomerScreen screen = new ConsoleCustomerScreen();

            // 1. Constructing the Presenter fills the screen. Nothing happened by itself:
            //    every one of those values was written by hand.
            //    Nothing refers to the Presenter afterwards - it stays alive because the
            //    screen's events hold on to it.
            new CustomerPresenter(screen, model, repository);

            Debug.Assert(screen.Updates.Contains(nameof(ICustomerScreen.Name)), "the Presenter pushed the initial state");
            Debug.Assert(screen.Balance == "0.00", "the Presenter formatted the number into text");
            Debug.Assert(!screen.SaveEnabled, "an untouched form has nothing to save");

            // 2. The user types. The View does not decide anything - it reports the edit and
            //    waits to be told what its own save button should look like.
            screen.TypeName("Customer1");
            Debug.Assert(screen.SaveEnabled, "an edited, valid form can be saved");
            Debug.Assert(screen.Status == "");

            // 3. Rubbish in the balance field. Parsing text is presentation logic, so the
            //    Presenter catches it and writes the complaint into the View.
            screen.TypeBalance("not a number");
            Debug.Assert(!screen.SaveEnabled);
            Debug.Assert(screen.Status == "The balance is not a number.");

            // 4. A valid number clears the complaint again. Note the sloppy input.
            screen.TypeBalance("100.5");
            Debug.Assert(screen.SaveEnabled);
            Debug.Assert(screen.Status == "");

            // 5. Validation covers every field, not just the last one touched.
            screen.TypeName("");
            Debug.Assert(!screen.SaveEnabled);
            Debug.Assert(screen.Status == "A customer needs a name.");
            screen.TypeName("Customer1");

            // 6. The click reaches the Model through the Presenter. The View knows neither
            //    the repository nor the Customer - it only ever raised an event.
            screen.ClickSave();

            Debug.Assert(repository.Saved.Count == 1);
            Debug.Assert(ReferenceEquals(repository.Saved[0], model));
            Debug.Assert(model.Name == "Customer1");
            Debug.Assert(model.Balance == 100.5m, "the Presenter parsed the text into a number");
            Debug.Assert(screen.Status == "Saved.");
            Debug.Assert(!screen.SaveEnabled, "after saving there is nothing left to save");

            // 7. A click the Presenter considers pointless does nothing, even if a broken
            //    View were to raise it anyway.
            screen.ClickSave();
            Debug.Assert(repository.Saved.Count == 1, "an unchanged form is not saved twice");

            // 8. The whole exchange ran in domain-free terms: the Model holds a decimal, the
            //    screen holds the text the Presenter made of it - the sloppy '100.5' came
            //    back as '100.50'. Swap this console screen for a WinForms form implementing
            //    ICustomerScreen and the Presenter stays exactly as it is.
            Debug.Assert(model.Balance == 100.5m);
            Debug.Assert(screen.Balance == "100.50", "formatting is the Presenter's business");
        }
    }
}
