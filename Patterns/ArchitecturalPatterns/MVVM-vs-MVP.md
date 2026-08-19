# MVVM vs. MVP

Both split the same three concerns. The difference is how the top two are wired together.

```
+------------------+----------------------------------------------+-----------------------------------------------------------+
| Aspect           | MVVM                                         | MVP                                                       |
+------------------+----------------------------------------------+-----------------------------------------------------------+
| Direction        | The ViewModel does NOT know the View         | The Presenter holds the View, as an interface, and pushes |
+------------------+----------------------------------------------+-----------------------------------------------------------+
| Mechanics        | INotifyPropertyChanged + ICommand            | Ordinary events upwards, property assignments downwards   |
+------------------+----------------------------------------------+-----------------------------------------------------------+
| Prerequisite     | Needs a binding engine                       | Needs none - hence the pattern for WinForms / Android     |
+------------------+----------------------------------------------+-----------------------------------------------------------+
| View knows about | Nothing; the binding reads the ViewModel     | Nothing; it only raises events and holds text             |
+------------------+----------------------------------------------+-----------------------------------------------------------+
| Refresh          | Happens by itself once a property notifies   | A method call the Presenter has to remember to make       |
+------------------+----------------------------------------------+-----------------------------------------------------------+
| Formatting       | Derived property on the ViewModel            | The Presenter formats into the View's strings             |
+------------------+----------------------------------------------+-----------------------------------------------------------+
| Validation       | ViewModel (property setter, IDataErrorInfo)  | Presenter                                                 |
+------------------+----------------------------------------------+-----------------------------------------------------------+
| Testable because | The ViewModel contains no UI type            | The View is an interface a stub can implement             |
+------------------+----------------------------------------------+-----------------------------------------------------------+
| Costs            | Notifications are easy to forget and to loop | Every update is written by hand, which gets chatty        |
+------------------+----------------------------------------------+-----------------------------------------------------------+
```

Neither is a GoF pattern. Both are compositions of ones that are:
Observer (change notification) and Command (the invokable action).


## Examples in this folder

- [MVVM.cs](MVVM.cs) - `MvvmExample.Mvvm()` walks the binding cycle in nine steps.

- [MVP.cs](MVP.cs) - `MvpExample.Mvp()` walks the same screen in eight, as a Passive View.

Both use the same `Customer` and `ICustomerRepository`, defined in `MVVM.cs`. That is
deliberate: the Model does not change when the presentation layer does.


## Two flavours of MVP

- **Passive View** (implemented here): the View contains no decision at all.

- **Supervising Controller**: the View binds simple values itself, the Presenter handles
  only the complex logic.


## What to pick

- No data binding in the toolkit -> MVP. That is why WinForms projects end up there.

- WPF, MAUI, Avalonia, or anything else with real bindings -> MVVM; the binding engine
  removes exactly the manual pushing that makes MVP tedious.
