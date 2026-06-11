using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TomeApp.Mobile.Models;
using TomeApp.Mobile.Services;

namespace TomeApp.Mobile.ViewModels;

public partial class TimerViewModel(LocalDatabaseService localDb) : ObservableObject
{
    private System.Timers.Timer? _timer;
    private DateTime _startTime;

    [ObservableProperty] private bool _isRunning;
    [ObservableProperty] private string _elapsedDisplay = "00:00:00";
    [ObservableProperty] private LocalBook? _currentBook;
    [ObservableProperty] private int _startPage;
    [ObservableProperty] private int _endPage;
    [ObservableProperty] private bool _showStopDialog;
    [ObservableProperty] private ObservableCollection<LocalBook> _readingBooks = new();

    public async Task LoadReadingBooksAsync()
    {
        var books = await localDb.GetBooksAsync(BookStatus.Reading);
        ReadingBooks = new ObservableCollection<LocalBook>(books);
        CurrentBook ??= books.FirstOrDefault();
        if (CurrentBook is not null) StartPage = CurrentBook.CurrentPage;
    }

    [RelayCommand]
    public void StartTimer()
    {
        if (CurrentBook is null || IsRunning) return;
        IsRunning = true;
        _startTime = DateTime.UtcNow;
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += (_, _) =>
        {
            var elapsed = DateTime.UtcNow - _startTime;
            ElapsedDisplay = elapsed.ToString(@"hh\:mm\:ss");
        };
        _timer.Start();
    }

    [RelayCommand]
    public void StopTimer()
    {
        if (!IsRunning) return;
        _timer?.Stop();
        ShowStopDialog = true;
    }

    [RelayCommand]
    public async Task ConfirmStopAsync()
    {
        if (CurrentBook is null) return;
        ShowStopDialog = false;
        IsRunning = false;
        _timer?.Dispose();

        var endTime = DateTime.UtcNow;
        var minutes = (int)(endTime - _startTime).TotalMinutes;
        if (minutes < 1) minutes = 1;

        var session = new LocalReadingSession
        {
            BookId = CurrentBook.Id,
            StartTime = _startTime,
            EndTime = endTime,
            MinutesRead = minutes,
            StartPage = StartPage,
            EndPage = EndPage > StartPage ? EndPage : StartPage,
            DeviceRecordedAt = endTime,
            IsSynced = false
        };

        await localDb.SaveSessionAsync(session);

        if (EndPage > CurrentBook.CurrentPage)
        {
            CurrentBook.CurrentPage = EndPage;
            await localDb.SaveBookAsync(CurrentBook);
        }

        ElapsedDisplay = "00:00:00";
    }

    [RelayCommand]
    public void CancelStop()
    {
        ShowStopDialog = false;
        StartTimer();
    }

    private sealed class ObservableCollection<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        public ObservableCollection() { }
        public ObservableCollection(IEnumerable<T> items) : base(items) { }
    }
}
