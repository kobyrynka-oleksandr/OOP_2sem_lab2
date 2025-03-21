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
        public string Result { get; private set; } = "";

        public void UpdateExpression(string newExpr)
        {
            Expression = newExpr;
            Recalculate();
        }

        private void Recalculate()
        {
            try
            {
                Result = new DataTable().Compute(Expression, null).ToString();
            }
            catch { }
        }
        public void Clear()
        {
            Expression = "";
            Result = "";
        }
    }
}
