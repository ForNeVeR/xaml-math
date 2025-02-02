﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     GenAPI Version: 7.0.10.26602
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace WpfMath
{
    public static partial class Extensions
    {
        public static byte[] RenderToPng(this XamlMath.TexFormula texForm, double scale, double x, double y, string systemTextFontName) { throw null; }
    }
}
namespace WpfMath.Controls
{
    public partial class FormulaControl : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector
    {
        public static readonly System.Windows.DependencyProperty ErrorsProperty;
        public static readonly System.Windows.DependencyProperty ErrorTemplateProperty;
        public static readonly System.Windows.DependencyProperty FormulaProperty;
        public static readonly System.Windows.DependencyProperty HasErrorProperty;
        public static readonly System.Windows.DependencyProperty ScaleProperty;
        public static readonly System.Windows.DependencyProperty SelectionBrushProperty;
        public static readonly System.Windows.DependencyProperty SelectionLengthProperty;
        public static readonly System.Windows.DependencyProperty SelectionStartProperty;
        public static readonly System.Windows.DependencyProperty SystemTextFontNameProperty;
        public FormulaControl() { }
        public System.Collections.ObjectModel.ObservableCollection<System.Exception> Errors { get { throw null; } }
        public System.Windows.Controls.ControlTemplate ErrorTemplate { get { throw null; } set { } }
        public string Formula { get { throw null; } set { } }
        public bool HasError { get { throw null; } }
        public double Scale { get { throw null; } set { } }
        public System.Windows.Media.Brush? SelectionBrush { get { throw null; } set { } }
        public int SelectionLength { get { throw null; } set { } }
        public int SelectionStart { get { throw null; } set { } }
        public string SystemTextFontName { get { throw null; } set { } }
        public void InitializeComponent() { }
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) { }
    }
    public partial class VisualContainerElement : System.Windows.FrameworkElement
    {
        public VisualContainerElement() { }
        public System.Windows.Media.DrawingVisual? Visual { get { throw null; } set { } }
        protected override int VisualChildrenCount { get { throw null; } }
        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize) { throw null; }
        protected override System.Windows.Media.Visual? GetVisualChild(int index) { throw null; }
        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize) { throw null; }
        protected override void OnVisualChildrenChanged(System.Windows.DependencyObject visualAdded, System.Windows.DependencyObject visualRemoved) { }
    }
}
namespace WpfMath.Converters
{
    public partial class SVGConverter
    {
        public SVGConverter() { }
        public string ConvertGeometry(System.Windows.Media.Geometry geometry) { throw null; }
    }
}
namespace WpfMath.Fonts
{
    public static partial class WpfCharInfoEx
    {
        public static System.Windows.Media.GlyphRun GetGlyphRun(this XamlMath.CharInfo info, double x, double y, double scale) { throw null; }
    }
}
namespace WpfMath.Parsers
{
    public static partial class WpfTeXFormulaParser
    {
        public static XamlMath.TexFormulaParser Instance { get { throw null; } }
    }
}
namespace WpfMath.Rendering
{
    public partial class GeometryElementRenderer : XamlMath.Rendering.IElementRenderer
    {
        public GeometryElementRenderer(System.Windows.Media.GeometryGroup geometry, double scale) { }
        public void FinishRendering() { }
        public void RenderCharacter(XamlMath.CharInfo info, double x, double y, XamlMath.Rendering.IBrush? foreground) { }
        public void RenderElement(XamlMath.Boxes.Box box, double x, double y) { }
        public void RenderLine(XamlMath.Rendering.Point point0, XamlMath.Rendering.Point point1, XamlMath.Rendering.IBrush? foreground) { }
        public void RenderRectangle(XamlMath.Rendering.Rectangle rectangle, XamlMath.Rendering.IBrush? foreground) { }
        public void RenderTransformed(XamlMath.Boxes.Box box, System.Collections.Generic.IEnumerable<XamlMath.Rendering.Transformations.Transformation> transforms, double x, double y) { }
    }
    public sealed partial class WpfBrush : XamlMath.Rendering.GenericBrush<System.Windows.Media.Brush>, System.IEquatable<WpfMath.Rendering.WpfBrush>
    {
        internal WpfBrush() { }
        protected override System.Type EqualityContract { get { throw null; } }
        public override bool Equals(object? obj) { throw null; }
        public bool Equals(WpfMath.Rendering.WpfBrush? other) { throw null; }
        public sealed override bool Equals(XamlMath.Rendering.GenericBrush<System.Windows.Media.Brush>? other) { throw null; }
        public static WpfMath.Rendering.WpfBrush FromBrush(System.Windows.Media.Brush value) { throw null; }
        public override int GetHashCode() { throw null; }
        public static bool operator ==(WpfMath.Rendering.WpfBrush? left, WpfMath.Rendering.WpfBrush? right) { throw null; }
        public static bool operator !=(WpfMath.Rendering.WpfBrush? left, WpfMath.Rendering.WpfBrush? right) { throw null; }
        protected override bool PrintMembers(System.Text.StringBuilder builder) { throw null; }
        public override string ToString() { throw null; }
        [System.Runtime.CompilerServices.PreserveBaseOverridesAttribute]
        virtual WpfMath.Rendering.WpfBrush XamlMath.Rendering.GenericBrush<System.Windows.Media.Brush>.<Clone>$() { throw null; }
    }
    public sealed partial class WpfBrushFactory : XamlMath.Rendering.IBrushFactory
    {
        internal WpfBrushFactory() { }
        public static readonly WpfMath.Rendering.WpfBrushFactory Instance;
        public XamlMath.Rendering.IBrush FromColor(XamlMath.Colors.RgbaColor color) { throw null; }
    }
    public static partial class WpfExtensions
    {
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute("brush")]
        public static XamlMath.Rendering.IBrush? ToPlatform(this System.Windows.Media.Brush? brush) { throw null; }
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute("brush")]
        public static System.Windows.Media.Brush? ToWpf(this XamlMath.Rendering.IBrush? brush) { throw null; }
        public static System.Windows.Point ToWpf(this XamlMath.Rendering.Point point) { throw null; }
    }
    public static partial class WpfTeXEnvironment
    {
        public static XamlMath.TexEnvironment Create(XamlMath.TexStyle style = 0, double scale = 20, string systemTextFontName = "Arial", System.Windows.Media.Brush? foreground = null, System.Windows.Media.Brush? background = null) { throw null; }
    }
    public static partial class WpfTeXFormulaExtensions
    {
        public static void RenderTo(this XamlMath.TexFormula formula, System.Windows.Media.DrawingContext drawingContext, XamlMath.TexEnvironment environment, double scale = 20, double x = 0, double y = 0) { }
        public static System.Windows.Media.Imaging.BitmapSource RenderToBitmap(this XamlMath.TexFormula formula, XamlMath.TexEnvironment environment, double scale = 20, double x = 0, double y = 0, double dpi = 96) { throw null; }
        public static System.Windows.Media.Geometry RenderToGeometry(this XamlMath.TexFormula formula, XamlMath.TexEnvironment environment, double scale = 20, double x = 0, double y = 0) { throw null; }
    }
}
