namespace WpfMath
{
    public class StringSpan
    {
        private readonly string _source;

        private readonly int _start;

        private readonly int _length;

        public StringSpan(string source, int start, int length)
        {
            _source = source;
            _start = start;
            _length = length;
        }

        public StringSpan(StringSpan source, int start, int length)
        {
            _source = source._source;
            _start = source._start + start;
            _length = length;
        }

        public override string ToString() => _source.Substring(_start, _length);

        public int Start => _start;

        public int Length => _length;

        public string Source => _source;

        public char this[int index] => _source[_start + index];

        public StringSpan Substring(int start) => new StringSpan(this, start, Length - start);
    }
}
