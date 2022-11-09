using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.LogicalTree;
using System;
using System.Windows.Input;

namespace Avalonia.Extensions.Controls
{
    public sealed class PaginationView : TemplatedControl, ICommandSource
    {
        private ICommand _command;
        private bool _commandCanExecute = true;
        static PaginationView()
        {
            CommandProperty.Changed.Subscribe(CommandChanged);
        }
        /// <summary>
        /// Gets or sets an <see cref="ICommand"/> to be invoked when the PaginationView is clicked.
        /// </summary>
        public ICommand Command
        {
            get => _command;
            set => SetAndRaise(CommandProperty, ref _command, value);
        }
        /// <summary>
        /// Defines the <see cref="Command"/> property.
        /// </summary>
        public static readonly DirectProperty<PaginationView, ICommand> CommandProperty =
          AvaloniaProperty.RegisterDirect<PaginationView, ICommand>(nameof(Command),
              content => content.Command, (content, command) => content.Command = command, enableDataValidation: true);
        /// <summary>
        /// Gets or sets a parameter to be passed to the <see cref="Command"/>.
        /// </summary>
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
        /// <summary>
        /// Defines the <see cref="CommandParameter"/> property.
        /// </summary>
        public static readonly StyledProperty<object> CommandParameterProperty =
           AvaloniaProperty.Register<PaginationView, object>(nameof(CommandParameter));
        /// <summary>
        /// Called when the <see cref="ICommand.CanExecuteChanged"/> event fires.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        public void CanExecuteChanged(object sender, EventArgs e)
        {
            var canExecute = Command == null || Command.CanExecute(CommandParameter);
            if (canExecute != _commandCanExecute)
            {
                _commandCanExecute = canExecute;
                UpdateIsEffectivelyEnabled();
            }
        }
        protected override void UpdateDataValidation<T>(AvaloniaProperty<T> property, BindingValue<T> value)
        {
            base.UpdateDataValidation(property, value);
            if (property == CommandProperty && value.Type == BindingValueType.BindingError && _commandCanExecute)
            {
                _commandCanExecute = false;
                UpdateIsEffectivelyEnabled();
            }
        }
        protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnAttachedToLogicalTree(e);
            if (Command != null)
            {
                Command.CanExecuteChanged += CanExecuteChanged;
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
        protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromLogicalTree(e);
            if (Command != null)
                Command.CanExecuteChanged -= CanExecuteChanged;
        }
        /// <summary>
        /// Called when the <see cref="Command"/> property changes.
        /// </summary>
        /// <param name="e">The event args.</param>
        private static void CommandChanged(AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Sender is PaginationView content)
            {
                if (((ILogical)content).IsAttachedToLogicalTree)
                {
                    if (e.OldValue is ICommand oldCommand)
                        oldCommand.CanExecuteChanged -= content.CanExecuteChanged;
                    if (e.NewValue is ICommand newCommand)
                        newCommand.CanExecuteChanged += content.CanExecuteChanged;
                }
                content.CanExecuteChanged(content, EventArgs.Empty);
            }
        }
    }
}