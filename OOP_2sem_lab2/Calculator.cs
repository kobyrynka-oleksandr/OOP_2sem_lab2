using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_2sem_lab2
{
    public class Calculator
    {
        public string Expression { get; private set; } = "";
        public string History { get; private set; } = "";

        public void UpdateExpression(string newExpr, string newHistory)
        {
            Expression = newExpr;
            History = newHistory;
        }
    }
}
