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
        List<string> op = new List<string>() { "/", "*", "-", "+", "^" };
        bool isFirstSymbol = true;
        private bool isExtraVisible = false;
        public MainWindow()
        {
            InitializeComponent();

            foreach (UIElement element in MainRoot.Children)
            {
                if (element is Button button)
                {
                    switch (button.Content.ToString())
                    {
                        case ".":
                        case "/":
                        case "*":
                        case "-":
                        case "+":
                            button.Click += BCOperations;
                            break;
                        case "√x":
                        case "n^x":
                        case "ln":
                        case "π":
                        case "e":
                            button.Click += BCExtraColumnOperations;
                            break;
                        case "C":
                        case "=":
                        case "⌫":
                            button.Click += BCControl;
                            break;
                        default:
                            button.Click += BCValue;
                            break;
                    }
                }
            }
        }
        private void BCOperations(object sender, RoutedEventArgs e)
        {
            CheckErrorOnScreen();

            string str = (string)((Button)e.OriginalSource).Content;

            if (exprLabel.Text.Length < 30)
            {
                if (!isFirstSymbol)
                {
                    if (!op.Contains(exprLabel.Text[exprLabel.Text.Length - 1].ToString()))
                    {
                        exprLabel.Text += str;
                        isFirstSymbol = exprLabel.Text.Length == 0 ? true : false;
                    }
                }
            }
        }
        private void BCExtraColumnOperations(object sender, RoutedEventArgs e)
        {
            CheckErrorOnScreen();
            string str = (string)((Button)e.OriginalSource).Content;

            switch (str)
            {
                case "π":
                    PiOp();
                    break;
                case "e":
                    EOp();
                    break;
                case "√x":
                    SqrtOp();
                    break;
                case "n^x":
                    PowerOp();
                    break;
                case "ln":
                    LnOp();
                    break;
            }
        }
        private void BCControl(object sender, RoutedEventArgs e)
        {
            CheckErrorOnScreen();
            string str = (string)((Button)e.OriginalSource).Content;

            switch (str)
            {
                case "=":
                    EqualOp(); 
                    break;
                case "⌫":
                    DelOp();
                    break;
                case "C":
                    ClearOp();
                    break;
            }
        }
        private void BCValue(object sender, RoutedEventArgs e)
        {
            CheckErrorOnScreen();
            string str = (string)((Button)e.OriginalSource).Content;

            if (exprLabel.Text.Length < 30)
            {
                if (double.TryParse(str, out double d))
                {
                    exprLabel.Text += str;
                    isFirstSymbol = exprLabel.Text.Length == 0 ? true : false;
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
        private void Undo(object sender, EventArgs e)
        {
            _commandManager.Undo();
            exprLabel.Text = _calculator.Expression;
            historyLabel.Text = _calculator.Result;
        }

        private void Redo(object sender, EventArgs e)
        {
            _commandManager.Redo();
            exprLabel.Text = _calculator.Expression;
            historyLabel.Text = _calculator.Result;
        }
        private void EqualOp()
        {
            try
            {
                if (exprLabel.Text.Contains("^"))
                {
                    string res = PowerOpCalculation();
                    historyLabel.Text = exprLabel.Text;
                    exprLabel.Text = res;
                }
                else
                {
                    string res = new DataTable().Compute(exprLabel.Text, null).ToString();
                    historyLabel.Text = exprLabel.Text;
                    exprLabel.Text = res;
                }
            }
            catch
            {
                exprLabel.Text = "Error!";
            }
        }
        private void DelOp()
        {
            if (exprLabel.Text.Length > 0)
            {
                exprLabel.Text = exprLabel.Text.Remove(exprLabel.Text.Length - 1);

                isFirstSymbol = exprLabel.Text.Length == 0 ? true : false;
            }
        }
        private void ClearOp()
        {
            _calculator.Clear();
            exprLabel.Text = "";
            historyLabel.Text = "";
            isFirstSymbol = true;
        }
        private void PiOp()
        {
            if (isFirstSymbol || op.Contains(exprLabel.Text[exprLabel.Text.Length - 1].ToString()))
            {
                exprLabel.Text += Math.PI.ToString("0.00000");
                isFirstSymbol = exprLabel.Text.Length == 0 ? true : false;
            }
        }
        private void EOp()
        {
            if (isFirstSymbol || op.Contains(exprLabel.Text[exprLabel.Text.Length - 1].ToString()))
            {
                exprLabel.Text += Math.E.ToString("0.00000");
                isFirstSymbol = exprLabel.Text.Length == 0 ? true : false;
            }
        }
        private void SqrtOp()
        {
            if (double.TryParse(exprLabel.Text, out double d))
            {
                string res = Math.Sqrt(d).ToString();
                historyLabel.Text = exprLabel.Text;
                exprLabel.Text = res;
            }
            else
            {
                exprLabel.Text = "Error!";
            }
        }
        private void PowerOp()
        {
            if (!isFirstSymbol)
            {
                if (!op.Contains(exprLabel.Text[exprLabel.Text.Length - 1].ToString()))
                {
                    exprLabel.Text += "^";
                }
            }
        }
        private string PowerOpCalculation()
        {
            Regex regex = new Regex(@"^(-?\d+(?:\.\d+)?)\^(-?\d+(?:\.\d+)?)$");

            try
            {
                Match match = regex.Match(exprLabel.Text);
                if (match.Success)
                {
                    double baseNumber = double.Parse(match.Groups[1].Value);
                    double exponent = double.Parse(match.Groups[2].Value);

                    return Math.Pow(baseNumber, exponent).ToString();
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                throw new Exception();
            }
        }

        private void LnOp()
        {
            if (double.TryParse(exprLabel.Text, out double d))
            {
                string res = Math.Log(d).ToString();
                historyLabel.Text = exprLabel.Text;
                exprLabel.Text = res;
            }
            else
            {
                exprLabel.Text = "Error!";
            }
        }
        private void CheckErrorOnScreen()
        {
            if (exprLabel.Text == "Error!")
            {
                exprLabel.Text = "";
            }
        }
    }
}
