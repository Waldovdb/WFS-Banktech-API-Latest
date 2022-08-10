using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTech_API_V1
{
    public class RequestLoggingMiddleware
    {
        static FileHelper fileHelper = new FileHelper();
        private string logDirectory = "logs\\requests\\RequestPayload.txt";

        private readonly RequestDelegate next;
        private readonly ILogger logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            var requestBody = Encoding.UTF8.GetString(buffer);
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            var builder = new StringBuilder(Environment.NewLine);

            fileHelper.EnsureDirectoryExists(logDirectory);
            using (StreamWriter sw = new StreamWriter(logDirectory, append: true))
            {
                sw.WriteLine("---------------------------------------------------------");
                sw.WriteLine("Request at: " + DateTime.Now.ToString() + "\n\nHeaders:\n");

                foreach (var header in context.Request.Headers)
                {
                    builder.AppendLine($"{header.Key}:{header.Value}");
                    sw.WriteLine($"{header.Key}:{header.Value}");
                }
                sw.WriteLine("\n");
                sw.WriteLine($"Request body:{requestBody}");

            }
           
            builder.AppendLine($"Request body:{requestBody}");

            logger.LogInformation(builder.ToString());

            await next(context);
        }
    }
}
