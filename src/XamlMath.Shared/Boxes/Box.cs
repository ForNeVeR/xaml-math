using System.Collections.Generic;
using System.Collections.ObjectModel;
using XamlMath.Rendering;

namespace XamlMath.Boxes;

// Represents graphical box that is part of math expression, and can itself contain child boxes.
public abstract class Box
{
    private readonly List<Box> children;
    private readonly ReadOnlyCollection<Box> childrenReadOnly;

    internal Box(TexEnvironment environment)
        : this(environment.Foreground, environment.Background)
    {
    }

    protected Box()
        : this(null, null)
    {
    }

    protected Box(IBrush? foreground, IBrush? background)
    {
        this.children = new List<Box>();
        this.childrenReadOnly = new ReadOnlyCollection<Box>(this.children);
        this.Foreground = foreground;
        this.Background = background;
    }

    public ReadOnlyCollection<Box> Children => this.childrenReadOnly;

    public SourceSpan? Source
    {
        get;
        set;
    }

    public IBrush? Foreground
    {
        get;
        set;
    }

    public IBrush? Background
    {
        get;
        set;
    }

    /// <summary>Total height of the box, including the <see cref="Depth"/>.</summary>
    public double TotalHeight => Height + Depth;

    /// <summary>Total width of the box, including the <see cref="Italic"/>.</summary>
    public double TotalWidth => Width + Italic;

    /// <summary>
    /// The additional width of the box that is taken by italic elements (e.g. letter elements that are protruding
    /// from the right of the base box.
    /// </summary>
    public double Italic { get; set; }

    /// <summary>The base width of the box.</summary>
    public double Width { get; set; }

    /// <summary>The base height of the box.</summary>
    public double Height { get; set; }

    /// <summary>
    /// The depth of the box: the height of the additional elements that are protruding from the bottom side of the
    /// box.
    /// </summary>
    public double Depth { get; set; }

    /// <summary>
    /// The shift of the box: an auxiliary value that has a box-specific meaning (e.g. a vertical shift in a
    /// <see cref="HorizontalBox"/>, or a horizontal shift in a <see cref="VerticalBox"/>).
    /// </summary>
    public double Shift { get; set; }

    public abstract void RenderTo(IElementRenderer renderer, double x, double y);

    public virtual void Add(Box box)
    {
        this.children.Add(box);
    }

    public virtual void Add(int position, Box box)
    {
        this.children.Insert(position, box);
    }

    public abstract int GetLastFontId();
}
