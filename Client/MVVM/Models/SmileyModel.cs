using System;
using System.IO;
using System.Reactive;

using Avalonia.Media.Imaging;
using ReactiveUI;

namespace Client.MVVM.Models
{
    internal class SmileyModel
    {
        public static readonly string 
            SourcePath = AppDomain.CurrentDomain.BaseDirectory
            + Path.Combine("..", "..", "..", "Assets", "Smileys")
            + Path.DirectorySeparatorChar;

        public required string Name { get; set; }
        public required string Source { get; set; }
        public Bitmap SourceBitmap { get => new Bitmap(SourcePath + this.Source); }
        public ReactiveCommand<string, Unit>? Command { get; set; }
    }
}
