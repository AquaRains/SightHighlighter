using System;
using System.Windows.Input;

namespace SightHighlighter
{
    public class ButtonCommand : ICommand
    {
        // bind buttoncommand to xaml and not access from code behind(Mainwindow.xaml.cs)
        // ref why we should not access bound data in code behind: https://stackoverflow.com/a/24849106
        // ref buttoncommand: https://esound.tistory.com/12
        private Action<object?> executeAction;
        private Func<object?,bool> canExecuteAction;

        public ButtonCommand(Action<object?> executeAction, Func<object?,bool> canExecuteAction)
        {
            this.executeAction = executeAction;
            this.canExecuteAction = canExecuteAction;
        }

        event EventHandler? ICommand.CanExecuteChanged
        {
            add
            {
            }

            remove
            {
            }
        }

        bool ICommand.CanExecute(object? parameter)
        {
            return true;
        }

        void ICommand.Execute(object? parameter)
        {
            executeAction(parameter);
        }
    }
}
