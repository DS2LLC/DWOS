using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.UI.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DWOS.UI.Admin.Time
{
    /// <summary>
    /// Interaction logic for MoveTimersDialog.xaml
    /// </summary>
    /// <remarks>
    /// Moves orders and batches to a user.
    /// </remarks>
    public partial class MoveTimersDialog : Window
    {
        #region Methods

        public MoveTimersDialog()
        {
            InitializeComponent();
            Icon = Properties.Resources.Clock_32.ToWpfImage();
        }

        public void LoadData(IEnumerable<IOperatorEntry> operatorEntries)
        {
            var context = new MoveTimersContext(operatorEntries);
            context.OnCompleted += Context_OnCompleted;
            DataContext = context;
        }

        #endregion

        #region Events

        private void Context_OnCompleted(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region MoveTimersContext

        /// <summary>
        /// Context for <see cref="MoveTimersDialog"/>.
        /// </summary>
        private sealed class MoveTimersContext : INotifyPropertyChanged
        {
            #region Fields

            /// <summary>
            /// Occurs when the context is finished moving orders.
            /// </summary>
            public event EventHandler OnCompleted;

            private UserEntry _selectedUser;

            #endregion

            #region Properties

            /// <summary>
            /// Gets a collection of operator entries to move.
            /// </summary>
            public IEnumerable<IOperatorEntry> OperatorEntries
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a collection of potential users to move orders/batches to.
            /// </summary>
            public IEnumerable<UserEntry> UserEntries
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets or sets the selected user.
            /// </summary>
            public UserEntry SelectedUser
            {
                get
                {
                    return _selectedUser;
                }
                set
                {
                    if (_selectedUser != value)
                    {
                        _selectedUser = value;
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedUser)));
                    }
                }
            }

            /// <summary>
            /// Gets the 'move items' command.
            /// </summary>
            public ICommand MoveItems
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Initializes a new instance of the <see cref="MoveTimersContext"/> class.
            /// </summary>
            /// <param name="operatorEntries">
            /// Operator entries to move to another operator.
            /// </param>
            public MoveTimersContext(IEnumerable<IOperatorEntry> operatorEntries)
            {
                if (operatorEntries == null)
                {
                    throw new ArgumentNullException(nameof(operatorEntries));
                }

                OperatorEntries = operatorEntries;

                var userEntries = new List<UserEntry>();

                using (var taUsers = new UsersTableAdapter())
                {
                    var users = taUsers.GetActiveOperators();

                    foreach (var user in users)
                    {
                        userEntries.Add(new UserEntry()
                        {
                            Name = user.Name,
                            UserId = user.UserID
                        });
                    }
                }

                UserEntries = userEntries.OrderBy(userRow => userRow.Name);
                SelectedUser = UserEntries.FirstOrDefault();
                MoveItems = new MoveCommand(this);
            }

            /// <summary>
            /// Completes the dialog; call after moving orders.
            /// </summary>
            public void Complete()
            {
                OnCompleted?.Invoke(this, EventArgs.Empty);
            }

            #endregion

            #region INotifyPropertyChanged Members

            public event PropertyChangedEventHandler PropertyChanged;

            #endregion

        }

        #endregion

        #region UserEntry

        /// <summary>
        /// Represents a user.
        /// </summary>
        private sealed class UserEntry
        {
            #region Properties

            public int UserId
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }

            #endregion
        }

        #endregion

        #region MoveCommand

        private sealed class MoveCommand : ICommand
        {
            #region Properties

            public MoveTimersContext Context
            {
                get;
                private set;
            }

            #endregion

            #region Methods

            public MoveCommand(MoveTimersContext context)
            {
                Context = context;
            }

            #endregion

            #region ICommand Members

            public event EventHandler CanExecuteChanged
            {
                add
                {
                    CommandManager.RequerySuggested += value;
                }
                remove
                {
                    CommandManager.RequerySuggested -= value;
                }
            }

            public bool CanExecute(object parameter)
            {
                return Context.SelectedUser != null;
            }

            public void Execute(object parameter)
            {
                if (!CanExecute(parameter))
                {
                    return;
                }

                var newUserId = Context.SelectedUser.UserId;
                foreach (var item in Context.OperatorEntries)
                {
                    if (item.UserId == newUserId)
                    {
                        continue;
                    }

                    item.MoveToUser(newUserId);
                }

                Context.Complete();
            }

            #endregion
        }

        #endregion
    }
}
