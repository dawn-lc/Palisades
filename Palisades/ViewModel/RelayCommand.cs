﻿using System;
using System.Windows.Input;

namespace Palisades.ViewModel
{
    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public partial class RelayCommand(Action execute, Func<bool>? canExecute) : ICommand
    {
        private readonly Action _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        /// <summary>
        /// Raised when RaiseCanExecuteChanged is called.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Determines whether this <see cref="RelayCommand"/> can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require 
        /// data to be passed, this object can be set to null.
        /// </param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute();
        }

        /// <summary>
        /// Executes the <see cref="RelayCommand"/> on the current command target.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require 
        /// data to be passed, this object can be set to null.
        /// </param>
        public void Execute(object? parameter)
        {
            _execute();
        }

        /// <summary>
        /// Method used to raise the <see cref="CanExecuteChanged"/> event
        /// to indicate that the return value of the <see cref="CanExecute"/>
        /// method has changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Creates a new command.
    /// </summary>
    /// <param name="execute">The execution logic.</param>
    /// <param name="canExecute">The execution status logic.</param>
    public partial class RelayCommand<T>(Action<T> execute, Func<bool>? canExecute) : ICommand
    {
        private readonly Action<T> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

        /// <summary>
        /// Raised when RaiseCanExecuteChanged is called.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Determines whether this <see cref="RelayCommand"/> can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require 
        /// data to be passed, this object can be set to null.
        /// </param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public bool CanExecute(object? parameter)
        {
            return canExecute == null || canExecute();
        }

        /// <summary>
        /// Executes the <see cref="RelayCommand"/> on the current command target.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the command. If the command does not require 
        /// data to be passed, this object can be set to null.
        /// </param>
        public void Execute(object? parameter)
        {
            if (parameter is not T)
            {
                throw new ArgumentException($"Parameter must be of type {typeof(T).Name}", nameof(parameter));
            }
            _execute((T)parameter);
        }

        /// <summary>
        /// Method used to raise the <see cref="CanExecuteChanged"/> event
        /// to indicate that the return value of the <see cref="CanExecute"/>
        /// method has changed.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
