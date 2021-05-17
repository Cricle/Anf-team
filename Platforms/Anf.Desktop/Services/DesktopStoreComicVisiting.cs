﻿using Anf.CDN;
using Anf.Desktop.Settings;
using Anf.Easy.Visiting;
using Anf.Platform;
using Anf.Platform.Services;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Services
{
    internal class DesktopStoreComicVisiting : StoreComicVisiting<Bitmap>
    {
        public DesktopStoreComicVisiting(IServiceProvider host, IResourceFactoryCreator<Bitmap> resourceFactoryCreator) : base(host, resourceFactoryCreator)
        {
            anfSettings = AppEngine.GetRequiredService<AnfSettings>();
            EnableRemote = true;
        }
        private readonly AnfSettings anfSettings;
        public override bool EnableCDNCache { get => anfSettings.Performace.EnableCDN; set => anfSettings.Performace.EnableCDN = value; }
        public override bool UseStore { get => anfSettings.Performace.UseStore; set => anfSettings.Performace.UseStore = value; }
        public override bool EnableRemote { get => anfSettings.Performace.EnableRemoteFetch; set => anfSettings.Performace.EnableRemoteFetch = value; }
        
        protected override CloudflareCDNCacheFetcher GetCDNFetcher()
        {
            return CloudflareCDNController.GetChapterCDN();
        }
    }
}
