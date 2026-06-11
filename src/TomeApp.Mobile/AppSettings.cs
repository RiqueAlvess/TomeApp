namespace TomeApp.Mobile;

public static class AppSettings
{
#if DEBUG
    public static string ApiBaseUrl = "http://10.0.2.2:5000/";
#else
    public static string ApiBaseUrl = "https://api.tomeapp.com.br/";
#endif
}
