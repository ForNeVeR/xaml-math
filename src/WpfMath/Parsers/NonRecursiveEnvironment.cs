using System;
using System.Collections.Generic;

namespace WpfMath.Parsers
{
    internal abstract class NonRecursiveEnvironment : ICommandEnvironment
    {
        protected readonly ICommandEnvironment BaseEnvironment;

        private readonly Lazy<IReadOnlyDictionary<string,ICommandParser>> _availableCommands;

        protected NonRecursiveEnvironment(ICommandEnvironment baseEnvironment)
        {
            BaseEnvironment = baseEnvironment;
            _availableCommands = new Lazy<IReadOnlyDictionary<string, ICommandParser>>(GenerateAllCommands);
        }

        public IReadOnlyDictionary<string, ICommandParser> AvailableCommands => _availableCommands.Value;
        protected abstract IReadOnlyDictionary<string, ICommandParser> AddedCommands { get; }

        public ICommandEnvironment CreateChildEnvironment() => BaseEnvironment;

        public abstract bool ProcessUnknownCharacter(TexFormula formula, char character);

        private IReadOnlyDictionary<string, ICommandParser> GenerateAllCommands()
        {
            var addedCommands = AddedCommands;
            if (addedCommands.Count == 0) return BaseEnvironment.AvailableCommands;

            var result = new Dictionary<string, ICommandParser>(
                BaseEnvironment.AvailableCommands.Count + addedCommands.Count);
            foreach (var pair in BaseEnvironment.AvailableCommands)
                result.Add(pair.Key, pair.Value);
            foreach (var pair in addedCommands)
                result.Add(pair.Key, pair.Value);
            return result;
        }
    }
}
