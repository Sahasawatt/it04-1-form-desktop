namespace IT04Form;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Db.Init();
        Application.Run(new MainForm());
    }
}
