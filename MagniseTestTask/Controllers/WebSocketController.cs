using MagniseTestTaskFintacharts.Database;
using MagniseTestTaskFintacharts.Models;
using MagniseTestTaskFintacharts.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MagniseTestTaskFintacharts.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WebSocketController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly Authentication _authentication;
        private readonly ApplicationContext _context;
        public WebSocketController(IConfiguration configuration, Authentication authentication, ApplicationContext applicationContext)
        {
            _configuration = configuration;
            _authentication = authentication;
            _context = applicationContext;
        }
    
        [HttpGet(Name = "Get data collected using WebSocket")]
        public ActionResult<List<WebSocketMessage>> Get(Guid guid)
        {
            var result = _context.Instruments.Where(x => x.id == guid).Include(x => x.webSocketMessages).ToList();
            return Ok(result);
        }
        [ApiExplorerSettings(IgnoreApi = true)]
        async private Task Connect()
        {
            var ws = new ClientWebSocket();
            var token = await _authentication.Access_Token();
            string uri = _configuration["Web:URI_WSS"] + "/api/streaming/ws/v1/realtime?token=" + token;

            Console.WriteLine("Connecting to server");
            await ws.ConnectAsync(new Uri(uri), CancellationToken.None);
            Console.WriteLine("Connected");

            var receiveTask = Task.Run(async () =>
            {
                using (var ms = new MemoryStream())
                {
                    while (ws.State == WebSocketState.Open)
                    {
                        WebSocketReceiveResult result;
                        do
                        {
                            var messageBuffer = WebSocket.CreateClientBuffer(1024, 16);
                            result = await ws.ReceiveAsync(messageBuffer, CancellationToken.None);
                            ms.Write(messageBuffer.Array, messageBuffer.Offset, result.Count);
                        }
                        while (!result.EndOfMessage);

                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            var msgString = Encoding.UTF8.GetString(ms.ToArray());

                            var document = JsonConvert.DeserializeObject<WebSocketMessage>(msgString);

                            document.instrument =
                            _context.Instruments.Where(x => x.id == new Guid(document.id)).FirstOrDefault();
                            _context.WebSocketMessages.Add(document);

                            _context.SaveChanges();
                        }
                        ms.Seek(0, SeekOrigin.Begin);
                        ms.Position = 0;
                    }
                }
            });
        }
    }
}
