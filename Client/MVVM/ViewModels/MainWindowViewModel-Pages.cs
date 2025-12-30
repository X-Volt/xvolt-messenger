using System;
using System.Reactive;

using AvRichTextBox;
using ReactiveUI;

namespace Client.MVVM.ViewModels
{
    partial class MainWindowViewModel : ViewModelBase
    {
        private bool _settingsPageVisible = false;
        private bool _newsPageVisible = true;

        public ReactiveCommand<Unit, Unit> SettingsCommand { get; set; }
        public FlowDocument SettingsPage { get; set; }
        public bool SettingsPageVisible
        {
            get => _settingsPageVisible;
            set => this.RaiseAndSetIfChanged(ref _settingsPageVisible, value);
        }

        public ReactiveCommand<Unit, Unit> NewsCommand { get; set; }
        public FlowDocument NewsPage { get; set; }
        public bool NewsPageVisible
        {
            get => _newsPageVisible;
            set => this.RaiseAndSetIfChanged(ref _newsPageVisible, value);
        }

        public ReactiveCommand<Unit, Unit> ArtworkCommand { get; set; }

        static private FlowDocument InitPage(string title = "")
        {
            var page = new FlowDocument();
            page.IsEditable = false;
            page.ClearDocument();

            // padding must be applied after it is cleared
            page.PagePadding = new(10, 0, 0, 10);

            if (!String.IsNullOrEmpty(title))
            {
                // heading
                var pageRun = new EditableRun(title);
                pageRun.FontSize = 20;
                pageRun.FontWeight = Avalonia.Media.FontWeight.Bold;

                var pagePar = new Paragraph();
                pagePar.Inlines.Add(pageRun);

                page.Blocks.Add(pagePar);
                page.Blocks.Add(new Paragraph());
            }
            else
            {
                page.Blocks.Add(new Paragraph());
            }

            return page;
        }

        public void Settings()
        {
            ChatPageVisible = false;
            NewsPageVisible = false;
            SettingsPageVisible = true;
        }

        public void News()
        {
            ChatPageVisible = false;
            SettingsPageVisible = false;
            NewsPageVisible = true;
        }

        public void Artwork()
        {
            // TODO - Open the artist's website
        }
    }
}
