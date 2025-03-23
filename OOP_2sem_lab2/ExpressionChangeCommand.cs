using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_2sem_lab2
{
    public class ExpressionChangeCommand : ICommand
    {
        private Calculator _calculator;
        private string _prevExpr;
        private string _prevHistory;
        private string _newExpr;
        private string _newHistory;

        public ExpressionChangeCommand(Calculator calculator, string newExpr, string newHistory)
        {
            _calculator = calculator;
            _prevExpr = calculator.Expression;
            _prevHistory = calculator.History;
            _newExpr = newExpr;
            _newHistory = newHistory;
        }

        public void Execute()
        {
            _calculator.UpdateExpression(_newExpr, _newHistory);
        }

        public void Unexecute()
        {
            _calculator.UpdateExpression(_prevExpr, _prevHistory);
        }
    }
}
