﻿using FintachartsApi.Database;
using FintachartsApi.Models;
using FintachartsApi.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FintachartsApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstrumentsController : Controller
    {
        private readonly ApplicationContext _context;
        private readonly GetHistoricalData _getHistoricalData;

        public InstrumentsController(ApplicationContext context, GetHistoricalData getHistoricalData)
        {
            _context = context;
            _getHistoricalData = getHistoricalData;
        }

        [HttpGet(Name = "Get historical prices for instrument")]
        async public Task<ActionResult<Models.Instrument>> Get(Guid guid, DateTime start, DateTime? end)
        {
            try
            {
                List<InstrumentValuesTimely> response = await _getHistoricalData.GetDateRangeAsync(guid, start, end);
                if(response == null)
                {
                    throw new Exception("Request returned no data");
                }
                var instrumentForReference = _context.Instruments.Where(x => x.id == guid).FirstOrDefault();
                foreach (InstrumentValuesTimely value in response)
                {
                    value.instrument = instrumentForReference;
                    _context.InstrumentsValuesTimely.Add(value);
                }

                var result = await _context.Instruments.Where(x => x.id == guid).Include(x => x.instrumentValuesTimely).ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
