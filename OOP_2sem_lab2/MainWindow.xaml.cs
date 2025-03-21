using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Windows.Media.Animation;

namespace OOP_2sem_lab2
{
    public partial class MainWindow : Window
    {
        private Calculator _calculator = new Calculator();
        private CommandManager _commandManager = new CommandManager();
        List<string> op = new List<string>() { "/", "*", "-", "+", "√", "n^x", "ln" };
        bool isFirstSymbol = true;
        private bool isExtraVisible = false;
        private bool awaitingSqrtInput = false;
        private bool awaitingPowerBase = false;
        private bool awaitingPowerExponent = false;
        private bool awaitingLnInput = false;
        private string sqrtInput = "";
        private string powerBase = "";
        private string powerExponent = "";
        private string lnInput = "";
        public MainWindow()
        {
            InitializeComponent();

            foreach (UIElement element in MainRoot.Children)
            {
                if (element is Button )
                {
                    ((Button)element).Click += ButtonClick;
                }
            }
        }
        private void ToggleExtraColumn(object sender, RoutedEventArgs e)
        {
            isExtraVisible = !isExtraVisible;
            ExtraColumn.Width = isExtraVisible ? new GridLength(1, GridUnitType.Star) : new GridLength(0);
            ToggleExtraColumnButton.Content = isExtraVisible ? "<" : "≡";
            this.Width = isExtraVisible ? 370 : 300;
        }
        private void UndoRedo(object sender, RoutedEventArgs e)
        {
            string str = (string)((Button)e.OriginalSource).Content;

            if (str == "Undo")
            {
                _commandManager.Undo();
            }
            else if (str == "Redo")
            {
                _commandManager.Redo();
            }

            exprLabel.Text = _calculator.Expression;
            tempRes.Text = _calculator.Result;
        }
        private void Undo(object sender, EventArgs e)
        {
            _commandManager.Undo();
            exprLabel.Text = _calculator.Expression;
            tempRes.Text = _calculator.Result;
        }

        private void Redo(object sender, EventArgs e)
        {
            _commandManager.Redo();
            exprLabel.Text = _calculator.Expression;
            tempRes.Text = _calculator.Result;
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            if (exprLabel.Text.Length == 0)
            {
                isFirstSymbol = true;
            }
            else if (exprLabel.Text.Length != 0)
            {
                isFirstSymbol = false;
            }
            if (exprLabel.Text == "Error!")
            {
                exprLabel.Text = "";
            }

            string str = (string)((Button)e.OriginalSource).Content;

            if (str == "=")
            {
                EqualOp();
            }
            else if (str == "⌫")
            {
                DelOp();
            }
            else if (str == "CE")
            {
                ClearEntryOp();
            }
            else if (str == "C")
            {
                ClearOp();
            }
            else if (str == "π")
            {
                PiOp();
            }
            else if (str == "e")
            {
                EOp();
            }
            else if (str == "√x")
            {
                SqrtOp();
                Calculation();
            }
            else if (str == "n^x")
            {
                PowerOp();
                Calculation();
            }
            else if (str == "ln")
            {
                LnOp();
                Calculation();
            }
            else if (awaitingSqrtInput && double.TryParse(str, out _))
            {
                sqrtInput += str;
            }
            else if (awaitingPowerBase && double.TryParse(str, out _))
            {
                powerBase += str;
            }
            else if (awaitingPowerExponent && double.TryParse(str, out _))
            {
                powerExponent += str;
            }
            else if (awaitingLnInput && double.TryParse(str, out _))
            {
                lnInput += str;
            }
            else
            {
                OthersOp(str);
            }
        }
        private void EqualOp()
        {
            try
            {
                exprLabel.Text = tempRes.Text;
                tempRes.Text = "";
            }
            catch
            {
                exprLabel.Text = "Error!";
                tempRes.Text = "";
            }
        }
        private void DelOp()
        {
            if (exprLabel.Text == "Error!")
            {
                exprLabel.Text = "";
            }

            if (exprLabel.Text.Length > 0)
            {
                exprLabel.Text = exprLabel.Text.Remove(exprLabel.Text.Length - 1);
                if (exprLabel.Text.Length > 0)
                {
                    Calculation();
                }
                else
                {
                    exprLabel.Text = "Error!";
                    tempRes.Text = "";
                }
            }
        }
        private void ClearEntryOp()
        {
            exprLabel.Text = "";
            tempRes.Text = "";
            awaitingSqrtInput = false;
            awaitingPowerBase = false;
            awaitingPowerExponent = false;
            awaitingLnInput = false;
            sqrtInput = "";
            powerBase = "";
            powerExponent = "";
            lnInput = "";
        }
        private void ClearOp()
        {
            exprLabel.Text = "";
            tempRes.Text = "";
            awaitingSqrtInput = false;
            awaitingPowerBase = false;
            awaitingPowerExponent = false;
            awaitingLnInput = false;
            sqrtInput = "";
            powerBase = "";
            powerExponent = "";
            lnInput = "";
        }
        private void PiOp()
        {
            if (isFirstSymbol || op.Contains(exprLabel.Text[exprLabel.Text.Length - 1].ToString()))
            {
                _commandManager.ExecuteCommand(new AddTextCommand(_calculator, Math.PI.ToString("0.00000")));
                exprLabel.Text = _calculator.Expression;
                tempRes.Text = _calculator.Result;
            }
        }
        private void EOp()
        {
            if (isFirstSymbol || op.Contains(exprLabel.Text[exprLabel.Text.Length - 1].ToString()))
            {
                _commandManager.ExecuteCommand(new AddTextCommand(_calculator, Math.E.ToString("0.00000")));
                exprLabel.Text = _calculator.Expression;
                tempRes.Text = _calculator.Result;
            }
        }
        private void OthersOp(string str)
        {
            if (exprLabel.Text.Length < 30)
            {
                if (double.TryParse(str, out double d))
                {
                    exprLabel.Text += str;
                    try
                    {
                        _commandManager.ExecuteCommand(new AddTextCommand(_calculator, str));
                        exprLabel.Text = _calculator.Expression;
                        tempRes.Text = _calculator.Result;
                    }
                    catch
                    {
                        exprLabel.Text = "Error!";
                        tempRes.Text = "";
                    }
                }
                else
                {
                    if (isFirstSymbol || !op.Contains(exprLabel.Text[exprLabel.Text.Length - 1].ToString()))
                    {
                        _commandManager.ExecuteCommand(new AddTextCommand(_calculator, str));
                        exprLabel.Text = _calculator.Expression;
                        tempRes.Text = _calculator.Result;
                    }
                }
            }
        }
        private void SqrtOp()
        {
            if (!awaitingSqrtInput)
            {
                awaitingSqrtInput = true;
                sqrtInput = "";
            }
            else
            {
                if (double.TryParse(sqrtInput, out double num) && num >= 0)
                {
                    double result = Math.Sqrt(num);

                    _commandManager.ExecuteCommand(new AddTextCommand(_calculator, result.ToString("0.#####")));
                    exprLabel.Text = _calculator.Expression;
                    tempRes.Text = _calculator.Result;
                }
                else
                {
                    exprLabel.Text = "Error!";
                    tempRes.Text = "";
                }
                awaitingSqrtInput = false;
            }
        }
        private void PowerOp()
        {
            if (!awaitingPowerBase && !awaitingPowerExponent)
            {
                awaitingPowerBase = true;
                powerBase = "";
                powerExponent = "";
            }
            else if (awaitingPowerBase)
            {
                awaitingPowerBase = false;
                awaitingPowerExponent = true;
            }
            else if (awaitingPowerExponent)
            {
                if (double.TryParse(powerBase, out double baseNum) && double.TryParse(powerExponent, out double expNum))
                {
                    double result = Math.Pow(baseNum, expNum);

                    _commandManager.ExecuteCommand(new AddTextCommand(_calculator, result.ToString("0.#####")));
                    exprLabel.Text = _calculator.Expression;
                    tempRes.Text = _calculator.Result;
                }
                else
                {
                    exprLabel.Text = "Error!";
                    tempRes.Text = "";
                }
                awaitingPowerExponent = false;
            }
        }

        private void LnOp()
        {
            if (!awaitingLnInput)
            {
                awaitingLnInput = true;
                lnInput = "";
            }
            else
            {
                if (double.TryParse(lnInput, out double num) && num > 0)
                {
                    double result = Math.Log(num);

                    _commandManager.ExecuteCommand(new AddTextCommand(_calculator, result.ToString("0.#####")));
                    exprLabel.Text = _calculator.Expression;
                    tempRes.Text = _calculator.Result;
                }
                else
                {
                    exprLabel.Text = "Error!";
                    tempRes.Text = "";
                }
                awaitingLnInput = false;
            }
        }
        private void Calculation()
        {
            try
            {
                if (!op.Contains(exprLabel.Text[exprLabel.Text.Length - 1].ToString()))
                {
                    string result = new DataTable().Compute(exprLabel.Text, null).ToString();
                    tempRes.Text = result;
                }
            }
            catch { }
        }
    }
}
