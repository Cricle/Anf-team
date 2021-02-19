﻿using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Soman
{
    [EnableService(ServiceLifetime = ServiceLifetime.Scoped)]
    public class SomanSearchProvider : ISearchProvider
    {
        public const string SeachUrl = "https://api.soman.com/soman.ashx?action=getsomancomics2&pageindex={0}&pagesize={1}&keyword={2}";

        private readonly HttpClient httpClient;

        public SomanSearchProvider(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<SearchComicResult> SearchAsync(string keywork, int skip, int take)
        {
            var page = 1;
            if (skip != 0 && skip > take)
            {
                page = take / skip;
            }
            var targetUrl = string.Format(SeachUrl, page, take, keywork);
            string str = string.Empty;
            using (var rep = await httpClient.GetAsync(targetUrl))
            {
                str = await rep.Content.ReadAsStringAsync();
            }
            var jobj = JObject.Parse(str);
            var total = jobj["Total"].Value<int>();
            var items = (JArray)jobj["Items"];
            var snaps = new List<ComicSnapshot>(items.Count);
            foreach (var item in items)
            {
                var comic = (JArray)item["Comics"];
                if (comic.Count == 0)
                {
                    continue;
                }
                var sn = new ComicSnapshot();
                var sources = new List<ComicSource>();
                foreach (var c in comic)
                {
                    var host = c["Host"];
                    var part = c["Url"];
                    var name = c["Source"];
                    var source = new ComicSource
                    {
                        Name = name.ToString(),
                        TargetUrl = host.ToString() + part.ToString()
                    };
                    sources.Add(source);
                }
                var first = comic[0];
                sn.Name = first["SomanId"].ToString();
                sn.ImageUri = first["PicUrl"].ToString();
                sn.Author = first["Author"].ToString();
                sn.Descript = first["Content"].ToString();
                sn.TargetUrl = targetUrl;
                sn.Sources = sources.ToArray();
                snaps.Add(sn);
            }
            return new SearchComicResult
            {
                Snapshots = snaps.ToArray(),
                Support = true,
                Total = total
            };
        }
    }
}
