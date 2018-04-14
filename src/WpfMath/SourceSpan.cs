namespace WpfMath
{
    public class SourceSpan
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
    }
}
