using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors.Core;

namespace Tool.TTFMetrics;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private string _content = string.Empty;
    private string _filePath = string.Empty;

    public MainViewModel()
    {
        SelectFileCommand = new ActionCommand(SelectFile);
    }

    public string Content
    {
        get => _content;
        private set => SetField(ref _content, value);
    }

    public string FilePath
    {
        get => _filePath;
        private set
        {
            if (SetField(ref _filePath, value))
                Content = BuildContent(_filePath);
        }
    }

    public ICommand SelectFileCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private static string BuildContent(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            return string.Empty;

        try
        {
            var uri = new Uri(filePath);

            var gtf = new GlyphTypeface(uri);

            var sb = new StringBuilder();

            var xHeight = gtf.XHeight;

            sb.AppendLine($@"xHeight=""{xHeight}""");

            foreach (var (code, index) in gtf.CharacterToGlyphMap)
            {
                if (code > 255)
                    continue;

                var bbh = gtf.Height;
                var bsb = gtf.BottomSideBearings[index];
                var tsb = gtf.TopSideBearings[index];
                var fullHeight = bbh - bsb - tsb;

                var depth = gtf.DistancesFromHorizontalBaselineToBlackBoxBottom[index];
                var height = fullHeight - depth;
                var width = gtf.AdvanceWidths[index];

                if (!IsValid(depth) || !IsValid(height) || !IsValid(width))
                    continue;

                sb.Append(@$"<Char code=""{code}"" width=""{width:F3}"" height=""{height:F3}"" ");
                if (depth != 0)
                    sb.Append(@$"depth=""{depth:F3}"" ");

                sb.AppendLine("/>");
            }

            return sb.ToString();
        }
        catch (Exception e)
        {
            return $"Error: {e.Message}";
        }
    }

    private static bool IsValid(double d) => !double.IsInfinity(d) && !double.IsNaN(d);

    private void SelectFile()
    {
        var ofd = new OpenFileDialog { Filter = "TTF file (*.tff)|*.ttf|All files (*.*)|*.*" };

        if (ofd.ShowDialog() is true)
            FilePath = ofd.FileName;
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
