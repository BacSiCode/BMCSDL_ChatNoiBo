namespace BaoCao
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {

            var chatServer = new Nhom8.ChatServer();
            Thread serverThread = new Thread(chatServer.Start);
            serverThread.IsBackground = true;
            serverThread.Start();

            ApplicationConfiguration.Initialize();
            Application.Run(new Nhom8.Dangnhaporcl());
            //Application.ApplicationExit += (s, e) => chatServer.Stop();
        }
    }
}