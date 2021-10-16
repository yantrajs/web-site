using Markdig;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YantraJS.Core;

namespace YantraJS.WebSite.Services
{
    ///<summary>
    ///    We will use this dependency in JavaScript to convert Markdown to html
    ///</summary>
    [DIRegister(ServiceLifetime.Singleton)]
    public class MarkdownService
    {
        private readonly HttpClient client;
        private readonly MarkdownPipeline pipeline;

        public MarkdownService(IHttpClientFactory httpClientFactory)
        {
            this.client = httpClientFactory.CreateClient("Default");
            this.pipeline = new MarkdownPipelineBuilder()
                .UsePipeTables()
                .UseGridTables()
                .UseAutoLinks()
                .Build();
        }

        public async Task<string> ProcessMarkdownAsync(string url)
        {
            var content = await client.GetStringAsync(url);

            return Markdown.ToHtml(content, pipeline);
        }

    }
}
