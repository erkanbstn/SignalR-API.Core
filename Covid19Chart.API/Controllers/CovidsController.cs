using Covid19Chart.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Covid19Chart.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CovidsController : ControllerBase
    {
        private readonly CovidService _covidService;

        public CovidsController(CovidService covidService)
        {
            _covidService = covidService;
        }
        [HttpPost]
        public async Task<IActionResult> SaveCovid(Covid covid)
        {
            await _covidService.SaveCovid(covid);
            List<Covid> covids = _covidService.GetList().ToList();
            return Ok(covids);
        }
        [HttpGet]
        public async Task<IActionResult> InitializeCovid()
        {
            Random random = new Random();
            Enumerable.Range(1, 10).ToList().ForEach(x =>
            {
                foreach (ECity item in Enum.GetValues(typeof(ECity)))
                {
                    var newcovid = new Covid
                    {
                        City = item,
                        Count = random.Next(100, 1000),
                        CovidDate = DateTime.Now.AddDays(x)
                    };
                    _covidService.SaveCovid(newcovid).Wait();
                    Thread.Sleep(1000);
                }
            });
            return Ok("Covid Dataları Veritabanına Kaydedildi.");
        }
    }
}
