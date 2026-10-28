using XamlMath.Boxes;
using XamlMath.Parsers;

namespace XamlMath.Atoms;

/// <summary>An atom representing an extensible arrow with a label above, e.g. \xrightarrow{label}.</summary>
internal sealed record ExtensibleArrowAtom : Atom
{
    private const double MinArrowWidth = 1.0; // Minimum arrow width in em-like units
    private const double LabelPadding = 0.2; // Padding between label and arrow

    public ExtensibleArrowAtom(
        SourceSpan? source,
        Atom? labelAtom,
        ExtensibleArrowDirection direction)
        : base(source)
    {
        LabelAtom = labelAtom;
        Direction = direction;
    }

    public Atom? LabelAtom { get; }

    public ExtensibleArrowDirection Direction { get; }

    protected override Box CreateBoxCore(TexEnvironment environment)
    {
        // Create boxes for label and arrow
        var labelBox = LabelAtom?.CreateBox(environment);
        var labelWidth = labelBox?.Width ?? 0;

        // Determine the arrow width: at least min width, and wide enough to cover the label
        var arrowWidth = System.Math.Max(MinArrowWidth, labelWidth + 2 * LabelPadding);

        // Find the appropriate arrow character
        var arrowName = Direction == ExtensibleArrowDirection.Right ? "rightarrow" : "leftarrow";
        var arrowChar = environment.MathFont.GetCharInfo(arrowName, environment.Style);
        if (!arrowChar.IsSuccess)
        {
            // Fallback: use a fixed-width arrow box
            return CreateFallbackBox(environment, labelBox, arrowWidth);
        }

        var arrowBox = new CharBox(environment, arrowChar.Value);
        var arrowCharWidth = arrowBox.Width;

        // We need to extend the arrow to match arrowWidth. 
        // Use a repeated approach: if the character has a next-larger variant, use it.
        // Otherwise, create a horizontal box with the arrow and extenders.
        var resultBox = new VerticalBox();

        // Place label above the arrow
        if (labelBox != null)
        {
            // Center the label over the arrow
            if (labelWidth < arrowWidth)
            {
                var centeredLabel = new HorizontalBox(labelBox, arrowWidth, TexAlignment.Center);
                resultBox.Add(centeredLabel);
            }
            else
            {
                resultBox.Add(labelBox);
                arrowWidth = labelWidth + 2 * LabelPadding;
            }

            // Add small space between label and arrow
            resultBox.Add(new StrutBox(0, LabelPadding, 0, 0));
        }

        // Create the arrow line with arrowhead at the end
        var arrowLineWidth = arrowWidth - arrowCharWidth;
        if (arrowLineWidth < 0) arrowLineWidth = 0;

        var arrowRow = new HorizontalBox();
        if (Direction == ExtensibleArrowDirection.Right)
        {
            // Draw the arrow shaft then the arrowhead
            if (arrowLineWidth > 0)
            {
                arrowRow.Add(CreateArrowShaft(environment, arrowLineWidth));
            }
            arrowRow.Add(arrowBox);
        }
        else
        {
            // Draw the arrowhead then the shaft
            arrowRow.Add(arrowBox);
            if (arrowLineWidth > 0)
            {
                arrowRow.Add(CreateArrowShaft(environment, arrowLineWidth));
            }
        }

        arrowRow.Height = arrowBox.Height;
        arrowRow.Depth = arrowBox.Depth;
        arrowRow.Width = arrowWidth;
        resultBox.Add(arrowRow);

        return resultBox;
    }

    private static Box CreateArrowShaft(TexEnvironment environment, double width)
    {
        // Use a horizontal rule as the arrow shaft
        var ruleThickness = environment.MathFont.GetDefaultLineThickness(environment.Style);
        var shaftBox = new HorizontalRule(environment, ruleThickness, width, 0);
        shaftBox.Height = ruleThickness;
        shaftBox.Depth = 0;
        return shaftBox;
    }

    private static Box CreateFallbackBox(TexEnvironment environment, Box? labelBox, double arrowWidth)
    {
        var resultBox = new VerticalBox();
        var lineThickness = environment.MathFont.GetDefaultLineThickness(environment.Style);

        if (labelBox != null)
        {
            var centeredLabel = new HorizontalBox(labelBox, arrowWidth, TexAlignment.Center);
            resultBox.Add(centeredLabel);
            resultBox.Add(new StrutBox(0, LabelPadding, 0, 0));
        }

        // Simple line with a small arrowhead approximation
        var arrowLine = new HorizontalRule(environment, lineThickness, arrowWidth, 0);
        resultBox.Add(arrowLine);
        resultBox.Height = (labelBox?.Height ?? 0) + (labelBox != null ? LabelPadding : 0) + lineThickness;
        resultBox.Depth = labelBox?.Depth ?? 0;
        resultBox.Width = arrowWidth;

        return resultBox;
    }
}
