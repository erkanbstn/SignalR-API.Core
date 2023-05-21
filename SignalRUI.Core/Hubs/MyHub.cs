using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalRUI.Core.Models;

namespace SignalRUI.Core.Hubs
{
    public class MyHub : Hub
    {
        private readonly AppDbContext _appDbContext;

        public MyHub(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        private static List<string> Names { get; set; } = new List<string>();
        private static int ClientCount { get; set; } = 0;
        public static int TeamCount { get; set; } = 3;
        public async Task SendName(string name)
        {
            if (Names.Count > TeamCount)
            {
                await Clients.Caller.SendAsync("Error", $"Takım En Fazla {TeamCount} Kişi Olabilir");
            }
            else
            {
                Names.Add(name);
                await Clients.All.SendAsync("ReceiveName", name);
            }

        }
        public async Task GetNames()
        {
            await Clients.All.SendAsync("ReceiveNames", Names);
        }
        public override async Task OnConnectedAsync()
        {
            ClientCount++;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
            await base.OnConnectedAsync();
        }
        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            ClientCount--;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);
            await base.OnDisconnectedAsync(exception);
        }

        // Groups

        public async Task AddToGroup(string teamName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);
        }
        public async Task RemoveToGroup(string teamName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);
        }
        public async Task SendNameByGroup(string name, string teamName)
        {
            var team = _appDbContext.Teams.Where(b => b.Name == teamName).FirstOrDefault();
            if (team != null)
            {
                team.Users.Add(new User { Name = name });
            }
            else
            {
                var newTeam = new Team { Name = teamName };
                newTeam.Users.Add(new User { Name = name });
                _appDbContext.Teams.Add(newTeam);
            }
            await _appDbContext.SaveChangesAsync();
            await Clients.Groups(teamName).SendAsync("ReceiveMessageByGroup", name, team.Id);
        }
        public async Task GetNamesByGroup()
        {
            var team = _appDbContext.Teams.Include(b => b.Users).Select(b => new
            {
                teamId=b.Id,
                Users=b.Users.ToList()
            });
            await Clients.All.SendAsync("ReceiveNamesByGroup", team);
        }
    }
}
