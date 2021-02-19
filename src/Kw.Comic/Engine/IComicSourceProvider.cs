﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Engine
{
    public interface IComicSourceCondition
    {
        string EnginName { get; }

        int Order { get; }

        string HttpName { get; }

        string ImageHttpName { get; }

        EngineDescript Descript { get; }

        Type ProviderType { get; }

        bool Condition(ComicSourceContext context);
    }
    public interface IComicSourceProvider
    {

        Task<ComicEntity> GetChaptersAsync(string targetUrl);

        Task<ComicPage[]> GetPagesAsync(string targetUrl);
    }
    public static class ComicSourceProviderExtenions
    {
        public static async Task<ChapterWithPage[]> GetChapterWithPageAsync(this IComicSourceProvider provider, string targetUrl,CancellationToken token=default(CancellationToken))
        {
            var cap = await provider.GetChaptersAsync(targetUrl);
            token.ThrowIfCancellationRequested();
            var cwps = new List<ChapterWithPage>();
            for (int a = 0; a < cap.Chapters.Length; a++)
            {
                token.ThrowIfCancellationRequested();
                var c = cap.Chapters[a];
                var pages = await provider.GetPagesAsync(c.TargetUrl);
                cwps.Add(new ChapterWithPage(c, pages));
            }
            return cwps.ToArray();
        }
    }
}
