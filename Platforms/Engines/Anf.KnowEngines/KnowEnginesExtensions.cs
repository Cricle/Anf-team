﻿using System;
using System.Collections.Generic;
using System.Text;
using Anf;
using Anf.Engine;
using Anf.KnowEngines.ProposalProviders;
using Anf.KnowEngines.SearchProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Anf.KnowEngines
{
    public static class KnowEnginesExtensions
    {
        public static void AddKnowEngines(this IServiceCollection services)
        {
            services.AddScoped<Dm5ComicOperator>();
            services.AddScoped<DmzjComicOperator>();
            services.AddScoped<KuaikanComicOperator>();
            services.AddScoped<JisuComicOperator>();
            services.AddScoped<TencentComicOperator>();
            services.AddScoped<BilibiliComicOperator>();
            services.AddScoped<QimiaoComicOperator>();
            services.AddScoped<MangabzComicOperator>();
            services.AddScoped<XmanhuaComicOperator>();
            services.AddScoped<BikabikaComicOperator>();

            services.AddScoped<SomanSearchProvider>();
            services.AddScoped<Dm5SearchProvider>();
            services.AddScoped<BilibiliSearchProvider>();

            services.AddScoped<Dm5ProposalProvider>();
            services.AddScoped<BilibiliProposalProvider>();
        }
        public static void UseKnowEngines(this IServiceProvider provider)
        {
            var eng = provider.GetRequiredService<ComicEngine>();
            eng.Add(new Dm5ComicSourceCondition());
            eng.Add(new DmzjComicSourceCondition());
            eng.Add(new JisuComicSourceCondition());
            eng.Add(new KuaikanComicSourceCondition());
            eng.Add(new TencentComicSourceCondition());
            eng.Add(new BilibiliComicSourceCondition());
            eng.Add(new QimianComicSourceCondition());
            eng.Add(new MangabzComicCondition());
            eng.Add(new XmanhuaComicCondition());
            eng.Add(new BikabikaComicCondition());

            var searchEng = provider.GetRequiredService<SearchEngine>();
            searchEng.Add(typeof(SomanSearchProvider));
            searchEng.Add(typeof(Dm5SearchProvider));
            searchEng.Add(typeof(BilibiliSearchProvider));

            var proEng = provider.GetRequiredService<ProposalEngine>();
            proEng.Add(new Dm5ProposalDescrition());
            proEng.Add(new BilibiliProposalDescription());
        }
    }
}
