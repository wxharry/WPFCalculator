using System;
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

using EvalWithParentheses;
namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool AddOpt = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            if (button != null)
            {
                Console.WriteLine("print" + button.Content.ToString());
            }
            
        }
        private void Enter_Operation(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            if (button != null)
            {
                if (Input.Text == "0")
                {
                    Input.Text = "";
                }
                Input.Text += button.Content.ToString();
            }
        }
        private void Click_Equal(object sender, RoutedEventArgs e)
        {
            Button? button = sender as Button;
            // convert equoation in input text to an answer
            // change ce -> ac
            // if not convertable keep it unchanged for now
            if (Program.Eval(Input.Text).ToString() != "")
            {
                double result = (double)Program.Eval(Input.Text);
                Answer.Text = Input.Text + "=";
                Input.Text = result.ToString();
                Clear_Button.Content = "AC";
            }

        }

        private void Click_Clear(object sender, RoutedEventArgs e)
        {
            if (Clear_Button.Content.ToString() == "CE")
            {
                Input.Text = Input.Text.Remove(Input.Text.Length-1);
                if (Input.Text == "")
                {
                    Input.Text = "0";
                }
            }
            if (Clear_Button.Content.ToString() == "AC")
            {
                Answer.Text = "Ans = " + Input.Text;
                Clear_Button.Content = "CE";
                Input.Text = "0";
            }
        }
    }
}

namespace EvalWithParentheses
{
    class Program
    {
        public static object Eval(string expression)
        {
            try
            {
                List<string> postfix = InfixToPostfix(expression);
                Stack<double> stack = new Stack<double>();
                Console.WriteLine(postfix.ToString());
                foreach (string token in postfix)
                {
                    Console.WriteLine(token);
                    if (double.TryParse(token, out double number))
                    {
                        stack.Push(number);
                    }
                    else if (stack.Count >= 2)
                    {
                        double right = stack.Pop();
                        double left = stack.Pop();
                        switch (token)
                        {
                            case "+":
                                stack.Push(left + right);
                                break;
                            case "-":
                                stack.Push(left - right);
                                break;
                            case "*":
                                stack.Push(left * right);
                                break;
                            case "/":
                                if (right == 0)
                                    {
                                        throw new DivideByZeroException();
                                    }
                                stack.Push(left / right);
                                break;
                            case "%":
                                stack.Push(left % right);
                                break;
                        }
                    }
                }
                // handle errors!!
                if(stack.Count != 1)
                {
                    return expression;
                }
                return stack.Pop();
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine("Error: {0}", ex.Message);
                return "";
            }
        }

        static List<string> InfixToPostfix(string expression)
        {
            List<string> postfix = new List<string>();
            Stack<string> stack = new Stack<string>();
            string[] tokens = expression.Split(' ');
            foreach (string token in tokens)
            {
                if (double.TryParse(token, out double number))
                {
                    postfix.Add(token);
                }
                else if (token == "(")
                {
                    stack.Push(token);
                }
                else if (token == ")")
                {
                    while (stack.Peek() != "(")
                    {
                        postfix.Add(stack.Pop());
                    }
                    stack.Pop();
                }
                else
                {
                    while (stack.Count > 0 && Prec(stack.Peek()) >= Prec(token))
                    {
                        postfix.Add(stack.Pop());
                    }
                    stack.Push(token);
                }
            }
            while (stack.Count > 0)
            {
                postfix.Add(stack.Pop());
            }
            return postfix;
        }

        static int Prec(string op)
        {
            switch (op)
            {
                case "*":
                case "/":
                case "%":
                    return 2;
                case "+":
                case "-":
                    return 1;
                default:
                    return 0;
            }
        }
    }
}
