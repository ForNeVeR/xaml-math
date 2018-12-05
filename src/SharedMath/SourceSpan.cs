using System;

namespace WpfMath
{
    public class SourceSpan : IEquatable<SourceSpan>
    {
        public SourceSpan(string source, int start, int length)
        {
            this.Source = source;
            this.Start = start;
            this.Length = length;
        }

        public override string ToString() => this.Source.Substring(this.Start, this.Length);

        public int Start { get; }

        public int End => this.Start + this.Length;

        public int Length { get; }

        public string Source { get; }

        public char this[int index] => this.Source[this.Start + index];

        public SourceSpan Segment(int start) => new SourceSpan(this.Source, this.Start + start, this.Length - start);
        public SourceSpan Segment(int start, int length) => new SourceSpan(this.Source, this.Start + start, length);

        public bool Equals(SourceSpan other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return this.Start == other.Start && this.Length == other.Length && string.Equals(this.Source, other.Source);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SourceSpan) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Start;
                hashCode = (hashCode * 397) ^ this.Length;
                hashCode = (hashCode * 397) ^ (this.Source != null ? this.Source.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
