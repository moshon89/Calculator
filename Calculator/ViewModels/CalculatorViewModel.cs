using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Calculator.Commands;
using Calculator.Models;

namespace Calculator.ViewModels
{

    class CalculatorViewModel : ObservableObject
    {
        private CalculatorModel _calculator;
        private String _textBox;
        private DelegateCommand<string> _digitPressedCommand;
        private DelegateCommand<string> _operatorPressedCommand;
        private DelegateCommand _resetPressedCommand;
        private DelegateCommand _delPressedCommand;
        private DelegateCommand _resultPressedCommand;
        private bool _isAfterResult = false;

        public CalculatorViewModel()
        {
            _calculator = new CalculatorModel();
            _textBox = _calculator.Default;
        }

        public ICommand ResultPressedCommand
        {
            get
            {
                if (_resultPressedCommand == null)
                {
                    _resultPressedCommand = new DelegateCommand(() =>
                    {
                        if (TextBox.Contains(_calculator.Error)) TextBox = _calculator.Default;
                        string result = "";
                        if (TextBox.StartsWith("-"))
                        {
                            result = _calculator.Calculate("0" + TextBox);
                        }
                        else
                        {
                            result = _calculator.Calculate(TextBox);
                          
                        }

                        _isAfterResult = true;
                        TextBox = result;
                    });
                }
                return _resultPressedCommand;
            }
        }

        public ICommand DelPressedCommand
        {
            get
            {
                if (_delPressedCommand == null)
                {
                    _delPressedCommand = new DelegateCommand(() =>
                        {
                            if (TextBox.Length == 1 || TextBox.Contains(_calculator.Error))
                            {
                                TextBox = _calculator.Default;
                            }
                            else
                            {
                            TextBox = TextBox.Substring(0, TextBox.Length - 1);
                            }

                            _isAfterResult = false;
                        });
                }
                return _delPressedCommand;
            }
        }

        public ICommand OperatorPressedCommand
        {
            get
            {
                if (_operatorPressedCommand == null)
                {
                    _operatorPressedCommand = new DelegateCommand<string>(OnOperatorPressedFunc);
                }
                return _operatorPressedCommand;
            }
        }

        private void OnOperatorPressedFunc(string operation)
        {
            if (TextBox.Contains(_calculator.Error)) TextBox = _calculator.Default;

            if (_calculator.IsOperator(TextBox.Substring(TextBox.Length - 1)))
            {
                TextBox = TextBox.Substring(0, TextBox.Length - 1) + operation;
            }
            else
            {
                InsertToTextBox(operation);
            }

            _isAfterResult = false;
        }


        public ICommand ResetPressedCommand
        {
            get
            {
                if (_resetPressedCommand == null)
                {
                    _resetPressedCommand = new DelegateCommand(() =>
                    {
                        TextBox = _calculator.Default;
//                        _numOfOpenBrackets = 0;
                    });
                }
                return _resetPressedCommand;
            }
        }

        public ICommand DigitPressedCommand
        {
            get
            {
                if (_digitPressedCommand == null)
                {
                    _digitPressedCommand = new DelegateCommand<string>(OnDigitPressedFunc);
                }

                return _digitPressedCommand;
            }
        }


        private void OnDigitPressedFunc(string digit)
        {
            if (TextBox.Contains(_calculator.Error))
            {
                TextBox = _calculator.Default;
            }

            if (_isAfterResult)
            {
                TextBox = _calculator.Default;
                _isAfterResult = false;
            }

            InsertToTextBox(digit);
        }

        private void InsertToTextBox(string digit)
        {
            if (TextBox.Equals(_calculator.Default) && !digit.Equals(".")) //if it's first digit replace the 0 instead to append
            {
                TextBox = digit;
            }
            else
            {
                TextBox += digit;
            }
        }

        public String Title { get; } = "EP Calculator";

        public string TextBox
        {
            get { return _textBox; }
            set
            {
                if (_textBox != value)
                {
                    _textBox = value;
                    OnPropertyChanged();
                }

            }
        }
    }
}
