using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win.Misc;
using Infragistics.Win.UltraWinEditors;
using Infragistics.Win.UltraWinToolbars;

namespace DWOS.UI.Tools
{
    public interface ICommandBase: IDisposable
    {
        /// <summary>
        ///   Gets a value indicating whether this <see cref="ICommandBase" /> is enabled.
        /// </summary>
        /// <value> <c>true</c> if enabled; otherwise, <c>false</c> . </value>
        bool Enabled { get; }
        Keys KeyMapping { get; set; }

        /// <summary>
        ///   Called when [click].
        /// </summary>
        void OnClick();

        /// <summary>
        ///   Refresh the tools enabled property and return enabled state.
        /// </summary>
        bool Refresh();
    }

    internal interface IButtonAdapter
    {
        object Button { get; }
        bool IsUpdating { get; set; }
        bool Enabled { get; set; }
        bool Visible { get; set; }
        string Caption { get; set; }
        event EventHandler OnButtonClick;
    }

    internal class ToolBaseButtonAdapter: IButtonAdapter
    {
        #region Properties

        public ToolBase Tool { get; set; }

        #endregion

        #region Methods

        public ToolBaseButtonAdapter(ToolBase tool)
        {
            if(tool != null)
            {
                this.Tool = tool;
                this.Tool.ToolClick += this._tool_ToolClick;
            }
        }

        #endregion

        #region Events

        private void _tool_ToolClick(object sender, ToolClickEventArgs e)
        {
            if(this.OnButtonClick != null)
                this.OnButtonClick(this, EventArgs.Empty);
        }

        #endregion

        #region IButtonAdapter Members

        object IButtonAdapter.Button
        {
            get { return this.Tool; }
        }

        public string Caption
        {
            get { return this.Tool == null ? null : this.Tool.SharedProps.Caption; }
            set
            {
                if(this.Tool != null)
                    this.Tool.SharedProps.Caption = value;
            }
        }

        public bool IsUpdating { get; set; }

        public bool Enabled
        {
            get { return this.Tool == null || this.Tool.SharedProps.Enabled; }
            set
            {
                if(this.Tool != null)
                    this.Tool.SharedProps.Enabled = value;
            }
        }

        public bool Visible
        {
            get { return this.Tool == null || this.Tool.SharedProps.Visible; }
            set
            {
                if(this.Tool != null)
                    this.Tool.SharedProps.Visible = value;
            }
        }

        public event EventHandler OnButtonClick;

        #endregion
    }

    internal class EditorButtonAdapter : IButtonAdapter
    {
        #region Properties

        public EditorButton Tool { get; set; }

        #endregion

        #region Methods

        public EditorButtonAdapter(EditorButton tool)
        {
            if (tool != null)
            {
                this.Tool = tool;
                this.Tool.Click += this._tool_ToolClick;
            }
        }

        #endregion

        #region Events

        private void _tool_ToolClick(object sender, EditorButtonEventArgs e)
        {
            if (this.OnButtonClick != null)
                this.OnButtonClick(this, EventArgs.Empty);
        }

        #endregion

        #region IButtonAdapter Members

        object IButtonAdapter.Button
        {
            get { return this.Tool; }
        }

        public string Caption
        {
            get { return this.Tool == null ? null : this.Tool.Text; }
            set
            {
                if (this.Tool != null)
                    this.Tool.Text = value;
            }
        }

        public bool IsUpdating { get; set; }

        public bool Enabled
        {
            get { return this.Tool == null || this.Tool.Enabled; }
            set
            {
                if (this.Tool != null)
                    this.Tool.Enabled = value;
            }
        }

        public bool Visible
        {
            get { return this.Tool == null || this.Tool.Visible; }
            set
            {
                if (this.Tool != null)
                    this.Tool.Visible = value;
            }
        }

        public event EventHandler OnButtonClick;

        #endregion
    }

    internal class UltraButtonAdapter: IButtonAdapter
    {
        #region Properties

        public UltraButton Tool { get; set; }

        #endregion

        #region Methods

        public UltraButtonAdapter(UltraButton tool)
        {
            if(tool != null)
            {
                this.Tool = tool;
                this.Tool.Click += this.Tool_Click;
            }
        }

        #endregion

        #region Events

        private void Tool_Click(object sender, EventArgs e)
        {
            if(this.OnButtonClick != null)
                this.OnButtonClick(this, EventArgs.Empty);
        }

        #endregion

        #region IButtonAdapter Members

        object IButtonAdapter.Button
        {
            get { return this.Tool; }
        }

        public string Caption
        {
            get { return this.Tool.Text; }
            set { this.Tool.Text = value; }
        }

        public bool IsUpdating { get; set; }

        public bool Enabled
        {
            get { return this.Tool.Enabled; }
            set { this.Tool.Enabled = value; }
        }

        public bool Visible
        {
            get { return this.Tool.Visible; }
            set { this.Tool.Visible = value; }
        }

        public event EventHandler OnButtonClick;

        #endregion
    }

    internal class CommandManager : ICommandManager
    {
        #region Fields

        private readonly Dictionary<string, ICommandBase> _commands = new Dictionary<string, ICommandBase>(20);

        #endregion

        #region ICommandManager Members

        /// <summary>
        ///   Adds the command.
        /// </summary>
        /// <param name="key"> The key. </param>
        /// <param name="command"> The command. </param>
        /// <returns> </returns>
        public ICommandBase AddCommand(string key, ICommandBase command)
        {
            this._commands.Add(key, command);
            return command;
        }

        /// <summary>
        ///   Finds the command.
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <returns> </returns>
        public ICommandBase FindCommand<T>()
        {
            foreach(var item in this._commands)
            {
                if(item.Value is T)
                    return item.Value;
            }

            return null;
        }

        public ICommandBase FindCommandByKeyMapping(Keys key)
        {
            return (from item in this._commands where item.Value != null && item.Value.KeyMapping == key select item.Value).FirstOrDefault();
        }

        /// <summary>
        ///   Refreshes all the commands.
        /// </summary>
        public void RefreshAll()
        {
            foreach(var item in this._commands)
                item.Value.Refresh();
        }

        public void Dispose()
        {
            foreach(var commandBase in this._commands)
                commandBase.Value.Dispose();

            this._commands.Clear();
        }

        #endregion
    }
}