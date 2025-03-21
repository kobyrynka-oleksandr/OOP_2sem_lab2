using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_2sem_lab2
{
    public interface ICommand
    {
        void Execute();
        void Unexecute();
    }
}
