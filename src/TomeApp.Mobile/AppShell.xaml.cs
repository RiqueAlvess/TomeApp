namespace TomeApp.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("addbook", typeof(Views.AddBookPage));
        Routing.RegisterRoute("bookdetail", typeof(Views.BookDetailPage));
    }
}
