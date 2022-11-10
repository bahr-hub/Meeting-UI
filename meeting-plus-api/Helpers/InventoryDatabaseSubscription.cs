using MeetingPlus.API.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Model.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using TableDependency.SqlClient;
using TableDependency.SqlClient.Base.Enums;
using TableDependency.SqlClient.Base.EventArgs;

namespace WebApp
{
    public class InventoryDatabaseSubscription : IDatabaseSubscription
    {
        private bool disposedValue = false;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BaseDataBaseContext _context;
        private SqlTableDependency<Notification> _tableDependency;

        public InventoryDatabaseSubscription(IHubContext<NotificationHub> hubContext, IHttpContextAccessor httpContextAccessor)//, BaseDataBaseContext context)
        {
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
            //_context = context;
        }

        public void Configure(string connectionString)
        {
            _tableDependency = new SqlTableDependency<Notification>(connectionString, null, null, null, null, null, DmlTriggerType.Insert);
            _tableDependency.OnChanged += Changed;
            _tableDependency.OnError += TableDependency_OnError;
            _tableDependency.Start();

            Console.WriteLine("Waiting for receiving notifications...");
        }

        private void TableDependency_OnError(object sender, ErrorEventArgs e)
        {
            Console.WriteLine($"SqlTableDependency error: {e.Error.Message}");
        }

        private void Changed(object sender, RecordChangedEventArgs<Notification> e)
        {
            if (e.ChangeType != ChangeType.None)
            {
                // TODO: manage the changed entity
                var changedEntity = e.Entity;//_context.Notification.FirstOrDefault(x => x.Id == e.Entity.Id);
                _hubContext.Clients.User(e.Entity.UserId.ToString()).SendAsync("updatenotification", changedEntity);
            }
        }
    }
}
