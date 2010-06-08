using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Reflection;
using System.Linq.Expressions;

namespace WpfMath
{
    public class TexExpressionVisitor:ExpressionVisitor
    {
        private TexFormulaParser _tfp;
        
        public TexExpressionVisitor(System.Linq.Expressions.Expression value, TexFormulaParser tfp )
        {
            _tfp = tfp;
            Formula = new TexFormula();
            Visit(value);
        }
        protected override System.Linq.Expressions.Expression VisitConstant(ConstantExpression c)
        {
            var atom = new CharAtom(c.ToString()[0], Formula.TextStyle);
            Formula.Add(atom);
            return base.VisitConstant(c);
        }

        public TexFormula Formula { get; private set; }
    }
    
    
}
