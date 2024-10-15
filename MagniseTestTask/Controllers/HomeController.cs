using MagniseTestTaskFintacharts.Database;
using MagniseTestTaskFintacharts.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagniseTestTaskFintacharts.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly GetInstruments _getInstruments;


        public HomeController(ApplicationContext context, GetInstruments getInstruments)
        {
            _context = context;
            _getInstruments = getInstruments;
        }

        [HttpGet(Name = "List Assets")]
        async public Task<ActionResult<List<Models.Instrument>>> List()
        {
            _context.Instruments.RemoveRange(_context.Instruments);            
            var instruments = await _getInstruments.Get();
            _context.Instruments.AddRange(instruments);
            _context.SaveChanges();
            var result = _context.Instruments.ToList();
            return Ok(result);
        }

        [HttpGet(Name = "Get Asset")]
        async public Task<ActionResult<Models.Instrument>> Get(Guid guid)
        {
            _context.Instruments.RemoveRange(_context.Instruments);
            var instruments = await _getInstruments.Get();
            _context.Instruments.AddRange(instruments);
            _context.SaveChanges();
            var result = _context.Instruments.Where(x => x.id == guid).Include(x => x.providers).ToList().FirstOrDefault();
            return Ok(result);
        }
    }
}
