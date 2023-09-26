using System;

namespace XamlMath;

public class SourceSpan : IEquatable<SourceSpan>
{
    public SourceSpan(string sourceName, string source, int start, int length)
    {
        SourceName = sourceName;
        this.Source = source;
        this.Start = start;
        this.Length = length;
    }

    public override string ToString() => this.Source.Substring(this.Start, this.Length);

    public int Start { get; }

    public int End => this.Start + this.Length;

    public int Length { get; }

    public string Source { get; }

    /// <summary>A name identifying the source (e.g. a user input, or some sort of macro body).</summary>
    /// <remarks>
    /// The name stored here is purely informative, and included to better distinguish various sources in case of
    /// complex formulas, when the resulting formula is composed of multiple text sources (where body of any macro
    /// or a macro-like construct is considered as a separate text source).
    /// </remarks>
    public string SourceName { get; }

    public char this[int index] => this.Source[this.Start + index];

    public SourceSpan Segment(int start) => new(SourceName, this.Source, this.Start + start, this.Length - start);
    public SourceSpan Segment(int start, int length) => new(SourceName, this.Source, this.Start + start, length);

    public bool Equals(SourceSpan? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.Start == other.Start
            && this.Length == other.Length
            && string.Equals(this.Source, other.Source, StringComparison.Ordinal)
            && string.Equals(SourceName, other.SourceName, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SourceSpan)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Start;
            hashCode = (hashCode * 397) ^ Length;
            hashCode = (hashCode * 397) ^ (Source != null ? Source.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (SourceName != null ? SourceName.GetHashCode() : 0);
            return hashCode;
        }
    }
}
