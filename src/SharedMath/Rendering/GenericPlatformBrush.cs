namespace WpfMath.Utils
{
    public class GenericPlatformBrush<TBrush> : IPlatformBrush
    {
        private readonly TBrush _brush;
        public GenericPlatformBrush(TBrush brush)
        {
            _brush = brush;
        }

        public TBrush Get() => _brush;
    }
}