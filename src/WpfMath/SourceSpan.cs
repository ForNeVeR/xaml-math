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

        public SourceSpan(SourceSpan source, int start, int length)
        {
            this.Source = source.Source;
            this.Start = source.Start + start;
            this.Length = length;
        }

        public override string ToString() => this.Source.Substring(this.Start, this.Length);

        public int Start { get; }

        public int Length { get; }

        public string Source { get; }

        public char this[int index] => this.Source[this.Start + index];

        public SourceSpan Substring(int start) => new SourceSpan(this, start, this.Length - start);
    }
}
