using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_2sem_lab2
{
    public class AddTextCommand : ICommand
    {
        private Calculator _calculator;
        private string _previousExpression;
        private string _textToAdd;

        public AddTextCommand(Calculator calculator, string text)
        {
            _calculator = calculator;
            _previousExpression = calculator.Expression;
            _textToAdd = text;
        }

        public void Execute()
        {
            _calculator.UpdateExpression(_previousExpression + _textToAdd);
        }

        public void Unexecute()
        {
            _calculator.UpdateExpression(_previousExpression);
        }
    }
}
