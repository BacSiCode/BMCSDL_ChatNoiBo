using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Oracle.ManagedDataAccess.Client;
namespace Nhom8.Hubs
{
    public class DataAccessService
    {
        private readonly string _connectionString =
            "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))"
            + "(CONNECT_DATA=(SERVICE_NAME=orcl)));"
            + "User Id=nhom8;Password=123;";

        public async Task SaveMessage(string groupName, string userId, string message)
        {
            string messageId = Guid.NewGuid().ToString().Substring(0, 10);

            string sql = "INSERT INTO TINNHAN (MATN, MANHOM, GUIBOI, NOIDUNG, THOIGIAN) " +
                         "VALUES (:matn, :manhom, :guiboi, NHOM8.MAHOARSA(:noidung), CURRENT_TIMESTAMP)";

            using (OracleConnection conn = new OracleConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("matn", messageId));
                    cmd.Parameters.Add(new OracleParameter("manhom", groupName));
                    cmd.Parameters.Add(new OracleParameter("guiboi", userId));
                    cmd.Parameters.Add(new OracleParameter("noidung", message));

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }

    public class ChatHub : Hub
    {
        private readonly DataAccessService _dbService;

        public ChatHub(DataAccessService dbService)
        {
            _dbService = dbService;
        }

        public async Task SendMessageToGroup(string groupName, string userId, string message)
        {
            try
            {
                await _dbService.SaveMessage(groupName, userId, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi lưu DB: " + ex.Message);
            }

            // 2. PHÂN PHỐI REAL-TIME
            await Clients.Group(groupName).SendAsync("ReceiveMessage", userId, message);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
namespace Nhom8
{
    public class ChatServer
    {
        private IHost _host;
        private const string BASE_ADDRESS = "http://localhost:5000";

        public void Start()
        {
            try
            {
                _host = Host.CreateDefaultBuilder()
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                        webBuilder.UseUrls(BASE_ADDRESS);
                    })
                    .Build();

                _host.Start();
                // Console.WriteLine($"Server started at {BASE_ADDRESS}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khởi động Server: {ex.Message}. Vui lòng kiểm tra port 5000.", "Lỗi Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Stop()
        {
            _host?.Dispose();
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Hubs.DataAccessService>();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            });

            services.AddSignalR();
            services.AddRouting();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<Hubs.ChatHub>("/chathub");
            });
        }
    }
}