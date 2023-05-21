using Covid19Chart.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Runtime.InteropServices;

namespace Covid19Chart.API.Models
{
    public class CovidService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<CovidHub> _hubContext;

        public CovidService(AppDbContext context, IHubContext<CovidHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }
        public IQueryable<Covid> GetList()
        {
            return _context.Covids.AsQueryable();
        }
        public async Task SaveCovid(Covid covid)
        {
            await _context.Covids.AddAsync(covid);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveCovidList", GetCovidChartList());
        }
        public List<CovidChart> GetCovidChartList()
        {
            List<CovidChart> covidCharts = new List<CovidChart>();
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "Select tarih,[1],[2],[3],[4],[5] FROM (select [City],[Count],Cast([CovidDate] as date) as tarih from Covids) as CovidT PIVOT (Sum(Count) for City in ([1],[2],[3],[4],[5])) as ptable order by tarih asc";
                command.CommandType = CommandType.Text;
                _context.Database.OpenConnection();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CovidChart covid = new CovidChart();
                        covid.CovidDate = reader.GetDateTime(0).ToShortDateString();

                        Enumerable.Range(1, 5).ToList().ForEach(x =>
                        {
                            if (DBNull.Value.Equals(reader[x]))
                            {
                                covid.Counts.Add(0);
                            }
                            else
                            {
                                covid.Counts.Add(reader.GetInt32(x));
                            }
                        });
                        covidCharts.Add(covid);
                    }
                }
                _context.Database.CloseConnection();
                return covidCharts;
            }

        }
    }
}
