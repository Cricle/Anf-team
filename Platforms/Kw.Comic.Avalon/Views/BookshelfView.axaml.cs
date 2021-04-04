using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Kw.Comic.Avalon.Services;
using System;

namespace Kw.Comic.Avalon.Views
{
    public class BookshelfView : UserControl
    {
        private readonly IDisposable binder;
        public BookshelfView()
        {
            InitializeComponent();
            binder = AppEngine.GetRequiredService<MainWindow>().BindDecorationMargin(this);
        }
        protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            binder.Dispose();
            base.OnDetachedFromLogicalTree(e);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
