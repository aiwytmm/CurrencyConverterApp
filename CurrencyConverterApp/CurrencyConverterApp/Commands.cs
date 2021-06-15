using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;

namespace CurrencyConverterApp
{
    class Commands<T> : ICommand
    {
        private readonly Action<T> action;
        public Commands(Action<T> Action)
        {
            action = Action;

        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public void Execute(object parameter)
        {
            action((T)parameter);
        }
        public event EventHandler CanExecuteChanged;
    }
}
