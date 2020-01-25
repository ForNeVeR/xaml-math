using System;

namespace WpfMath.Utils
{
    internal static class Result
    {
        public static Result<TValue> Ok<TValue>(TValue value) => new Result<TValue>(value, null);
        public static Result<TValue> Error<TValue>(Exception error) => new Result<TValue>(default, error);
    }

    internal readonly struct Result<TValue>
    {
        private readonly TValue value;

        public TValue Value => this.Error == null ? this.value : throw this.Error;
        public Exception Error { get; }

        public bool IsSuccess => this.Error == null;

        public Result(TValue value, Exception error)
        {
            if (!Equals(value, default) && error != null)
            {
                throw new ArgumentException($"Invalid {nameof(Result)} constructor call", nameof(error));
            }

            this.value = value;
            this.Error = error;
        }

        public Result<TProduct> Map<TProduct>(Func<TValue, TProduct> mapper) => this.IsSuccess
            ? Result.Ok(mapper(this.Value))
            : Result.Error<TProduct>(this.Error);
    }
}
