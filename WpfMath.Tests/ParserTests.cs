using DeepEqual.Syntax;
using Xunit;

namespace WpfMath.Tests
{
    public class ParserTests
    {
        private readonly TexFormulaParser _parser = new TexFormulaParser();

        static ParserTests()
        {
            TexFormulaParser.Initialize();
        }

        [Fact]
        public void SimpleAtomList()
        {
            var formula = _parser.Parse("2+2");
            var expected = new TexFormula
            {
                RootAtom = new RowAtom
                {
                    Elements =
                    {
                        new CharAtom('2'),
                        new SymbolAtom("plus", TexAtomType.BinaryOperator, false),
                        new CharAtom('2')
                    }
                }
            };

            formula.WithDeepEqual(expected).ExposeInternalsOf<TexFormula>().Assert();
        }
    }
}
