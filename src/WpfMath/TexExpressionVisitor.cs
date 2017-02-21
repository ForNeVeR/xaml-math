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
            foreach (var atom in c.ToString().Select(ch => new CharAtom(ch,Formula.TextStyle)))
            {
                Formula.Add(atom);	
            }
            return base.VisitConstant(c);
        }

        protected override System.Linq.Expressions.Expression VisitParameter(ParameterExpression p)
        {
            foreach (var atom in p.Name.Select(ch => new CharAtom(ch, Formula.TextStyle)))
            {
                Formula.Add(atom);
            }
            return base.VisitParameter(p);
        }

        public TexFormula Formula { get; private set; }
    }
    
    
}
