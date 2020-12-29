using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AdbKeys
{
    public class AsyncCommand<T> : ICommand
    {
        private Func<T, Task> action;
        private Func<T, bool> canExecute;

        public event EventHandler CanExecuteChanged;

        public AsyncCommand(Func<T, Task> action, Func<T, bool> canExecute = null)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute((T)parameter);
        }

        public async void Execute(object parameter)
        {
            if (CanExecute(parameter))
                await action((T)parameter);
        }
    }
}
