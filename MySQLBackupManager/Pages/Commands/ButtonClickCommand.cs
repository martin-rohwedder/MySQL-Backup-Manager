using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MySQLBackupManager.Pages.Commands
{
    class ButtonClickCommand : ICommand
    {
        private readonly Action handler;

        public ButtonClickCommand(Action handler)
        {
            this.handler = handler;
        }

        public void Execute(object parameter)
        {
            handler();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
