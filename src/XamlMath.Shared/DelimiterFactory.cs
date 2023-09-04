using XamlMath.Boxes;

namespace XamlMath;

// Creates boxes containing delimeter symbol that exists in different sizes.
internal static class DelimiterFactory
{
    public static Box CreateBox(string symbol, double minHeight, TexEnvironment environment, SourceSpan? source = null)
    {
        var texFont = environment.MathFont;
        var style = environment.Style;
        var charInfo = texFont.GetCharInfo(symbol, style).Value;

        // Find first version of character that has at least minimum height.
        var metrics = charInfo.Metrics;
        var totalHeight = metrics.Height + metrics.Depth;
        while (totalHeight < minHeight && texFont.HasNextLarger(charInfo))
        {
            charInfo = texFont.GetNextLargerCharInfo(charInfo, style);
            metrics = charInfo.Metrics;
            totalHeight = metrics.Height + metrics.Depth;
        }

        if (totalHeight >= minHeight)
        {
            // Character of sufficient height was found.
            return new CharBox(environment, charInfo);
        }
        else if (texFont.IsExtensionChar(charInfo))
        {
            var resultBox = new VerticalBox() { Source = source };

            // Construct box from extension character.
            var extension = texFont.GetExtension(charInfo, style);
            if (extension.Top != null)
                resultBox.Add(new CharBox(environment, extension.Top) { Source = source });
            if (extension.Middle != null)
                resultBox.Add(new CharBox(environment, extension.Middle) { Source = source });
            if (extension.Bottom != null)
                resultBox.Add(new CharBox(environment, extension.Bottom) { Source = source });

            if (extension.Repeat != null)
            {
                // Insert repeatable part multiple times until box is high enough.
                var repeatBox = new CharBox(environment, extension.Repeat) { Source = source };
                do
                {
                    if (extension.Top != null && extension.Bottom != null)
                    {
                        resultBox.Add(1, repeatBox);
                        if (extension.Middle != null)
                            resultBox.Add(resultBox.Children.Count - 1, repeatBox);
                    }
                    else if (extension.Bottom != null)
                    {
                        resultBox.Add(0, repeatBox);
                    }
                    else
                    {
                        resultBox.Add(repeatBox);
                    }
                } while (resultBox.Height + resultBox.Depth < minHeight);
            }

            return resultBox;
        }
        else
        {
            // No extensions available, so use tallest available version of character.
            return new CharBox(environment, charInfo) { Source = source };
        }
    }
}
