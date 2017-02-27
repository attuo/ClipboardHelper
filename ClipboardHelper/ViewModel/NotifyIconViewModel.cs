using System;
using System.Windows;
using System.Windows.Input;
using ClipboardHelper.View;

namespace ClipboardHelper.ViewModel
{
    class NotifyIconViewModel
    {
        /// <summary>
        /// Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CanExecuteFunc = () => Application.Current.MainWindow.IsVisible == false || Application.Current.MainWindow.WindowState == WindowState.Minimized,
                    CommandAction = () =>
                    {
                            //Application.Current.MainWindow = new MainWindow();
                            Application.Current.MainWindow.Show();
                        Application.Current.MainWindow.WindowState = WindowState.Normal;
                    }
                };
            }
        }

        /// <summary>
        /// Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand HideWindowCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => Application.Current.MainWindow.Hide(),
                    CanExecuteFunc = () => Application.Current.MainWindow.IsVisible == true
                };
            }
        }


        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new DelegateCommand
                {
                    CommandAction = () => Application.Current.Shutdown()
                };
            }
        }
    }

    /// <summary>
    /// Gets the previous copy from clipboard manager
    /// </summary>
    public ICommand PreviousCopyCommand
    {
        get
        {
            //TODO: Do the functionality for the previous copy
            return new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };
        }
    }


    /// <summary>
    /// Simplistic delegate command for the demo.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}