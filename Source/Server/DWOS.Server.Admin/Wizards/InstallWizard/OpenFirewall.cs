using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DWOS.Shared.Wizard;
using NLog;
using NetFwTypeLib;


namespace DWOS.Server.Admin.Wizards.InstallWizard
{
    /// <summary>
    /// The 'open firewall' step of the installation wizard.
    /// </summary>
    public partial class OpenFirewall : UserControl, IWizardPanel
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private bool _started;
        
        private FirewallManager _firewall = new FirewallManager();

        private List <FirewallRule> _firewallRules;

        const string APP_NAME = "DWOS";

        #endregion

        public OpenFirewall()
        {
            InitializeComponent();

            _firewallRules = new List <FirewallRule>();
            _firewallRules.Add(new FirewallRule() {Name = APP_NAME + " License", Port = Properties.Settings.Default.FirewallLicensePort, IsTCP = true, Status = false});
            _firewallRules.Add(new FirewallRule() { Name = APP_NAME + " SQL", Port = Properties.Settings.Default.FirewallSQLPort, IsTCP = true, Status = false });
            _firewallRules.Add(new FirewallRule() { Name = APP_NAME + " Mobile", Port = Properties.Settings.Default.FirewallRESTPort, IsTCP = true, Status = false });
            _firewallRules.Add(new FirewallRule() { Name = APP_NAME + " Mobile License", Port = Properties.Settings.Default.FirewallMobileLicensePort, IsTCP = true, Status = false });
        }

        private void OpenFirewallPorts()
        {
            try
            {
                _log.Info("Opening firewall rules.");

                foreach(var rule in _firewallRules)
                    _firewall.Open(rule);

                UpdateStatus();
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error opening firewall rules.");
            }
        }

        private void UpdateStatus()
        {
            try
            {
                var enabled = _firewall.IsEnabled();
                txtStatus.Text = enabled ? "Enabled" : "Disabled";

                if (!enabled)
                {
                    this.IsValid = true;
                    lvwFirewallRules.Enabled = false;

                    lvwFirewallRules.Items.Clear();

                    //add all rules in by default
                    foreach (var rule in _firewallRules)
                        lvwFirewallRules.Items.Add(new Infragistics.Win.UltraWinListView.UltraListViewItem(rule.Name, new object[] { "True", rule.Port  }));
                }
                else
                {
                    lvwFirewallRules.Enabled = true;
                    lvwFirewallRules.Items.Clear();

                    //add rules to list after updating status
                    foreach(var rule in _firewallRules)
                    {
                        rule.Status = _firewall.IsOpen(rule);
                        lvwFirewallRules.Items.Add(new Infragistics.Win.UltraWinListView.UltraListViewItem(rule.Name, new object[] { rule.Status, rule.Port }));
                    }

                    var allOpen = _firewallRules.All(r => r.Status);
                    
                    if (allOpen)
                    {
                        this.IsValid = true;
                        btnStart.Enabled = false;
                    }
                    else
                    {
                        this.IsValid = false;
                        btnStart.Enabled = true;
                    }
                }
            }
            catch(Exception exc)
            {
                this.IsValid = true;
                _log.Error(exc, "Error determining if service is installed.");
            }
        }

        #region Events
        
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFirewallPorts();
            }
            catch(Exception exc)
            {
                _log.Error(exc, "Error opening firewall ports.");
            }
            finally
            {
                UpdateStatus();
            }
        }

        #endregion

        #region IWizardPanel

        public string Title
        {
            get { return "Firewall"; }
        }

        public string SubTitle
        {
            get { return "Allow DWOS to operate through the local Windows Firewall."; }
        }

        public Action <IWizardPanel> OnValidStateChanged { get; set; }

        public void Initialize(WizardController controller) { }

        public Control PanelControl
        {
            get { return this; }
        }

        public bool IsValid
        {
            get { return this._started; }
            set
            {
                _started = value;

                OnValidStateChanged?.Invoke(this);
            }
        }

        public void OnMoveTo()
        {
            UpdateStatus();
        }

        public void OnMoveFrom()
        {
        }

        public void OnFinished() { }

        #endregion

        #region FirewallManager

        /// <summary>
        /// Manages firewall configuration.
        /// </summary>
        public class FirewallManager
        {
            #region Fields

            private INetFwProfile _fwProfile;

            #endregion

            #region Properties

            private INetFwProfile FwProfile
            {
                get
                {
                    if (_fwProfile == null)
                    {
                        // Access INetFwMgr
                        INetFwMgr fwMgr = (INetFwMgr)getInstance("INetFwMgr");
                        INetFwPolicy fwPolicy = fwMgr.LocalPolicy;
                        _fwProfile = fwPolicy.CurrentProfile;
                    }

                    return _fwProfile;
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Gets a value indicating if the firewall is enabled.
            /// </summary>
            /// <returns>
            /// <c>true</c> if the firewall is enabled;
            /// otherwise, <c>false</c>.
            /// </returns>
            public bool IsEnabled()
            {
                return FwProfile.FirewallEnabled;
            }

            /// <summary>
            /// Gets a value indicating if the rule is open.
            /// </summary>
            /// <param name="rule"></param>
            /// <returns>
            /// <c>true</c> if the rule is already open;
            /// otherwise <c>false</c>.
            /// </returns>
            public bool IsOpen(FirewallRule rule)
            {
                return IsOpenByName(rule.Name);
            }

            /// <summary>
            /// Opens the port associated with the given rule.
            /// </summary>
            /// <param name="rule"></param>
            public void Open(FirewallRule rule)
            {
                OpenFirewall(rule.Port, rule.Name, rule.IsTCP ? NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP : NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP);
                rule.Status = true;
            }

            private bool IsOpenByName(string portName)
            {
                var openports = FwProfile.GloballyOpenPorts;
                var enumerate = openports.GetEnumerator();

                while (enumerate.MoveNext())
                {
                    var openPort = (INetFwOpenPort)enumerate.Current;
                    
                    if (openPort.Name == portName)
                        return true;
                }

                return false;
            }

            private void OpenFirewall(int portNumber, string firewallRuleName, NET_FW_IP_PROTOCOL_ protocol)
            {
                _log.Info("Opening firewall rule {1}:{0}", portNumber, firewallRuleName);
                
                if(IsOpenByName(firewallRuleName))
                    RemoveByName(firewallRuleName);
                
                var openports = FwProfile.GloballyOpenPorts;
                var openport = (INetFwOpenPort)getInstance("INetOpenPort");
                openport.Port = portNumber;
                openport.Protocol = protocol;
                openport.Name = firewallRuleName;
                openport.Enabled = true;
                openports.Add(openport);
            }

            private void RemoveByName(string firewallRuleName)
            {
                var ports = FwProfile.GloballyOpenPorts;

                var openports = FwProfile.GloballyOpenPorts;
                var enumerate = openports.GetEnumerator();

                while (enumerate.MoveNext())
                {
                    var openPort = (INetFwOpenPort)enumerate.Current;

                    if(openPort.Name == firewallRuleName)
                    {
                        ports.Remove(openPort.Port, openPort.Protocol);
                        return;
                    }
                }
            }

            private Object getInstance(String typeName)
            {
                if (typeName == "INetFwMgr")
                {
                    Type type = Type.GetTypeFromCLSID(
                    new Guid("{304CE942-6E39-40D8-943A-B913C40C9CD4}"));
                    return Activator.CreateInstance(type);
                }
                else if (typeName == "INetAuthApp")
                {
                    Type type = Type.GetTypeFromCLSID(
                    new Guid("{EC9846B3-2762-4A6B-A214-6ACB603462D2}"));
                    return Activator.CreateInstance(type);
                }
                else if (typeName == "INetOpenPort")
                {
                    Type type = Type.GetTypeFromCLSID(
                    new Guid("{0CA545C6-37AD-4A6C-BF92-9F7610067EF5}"));
                    return Activator.CreateInstance(type);
                }
                else return null;
            }

            #endregion
        }

        #endregion

        #region FirewallRule
        
        /// <summary>
        /// Represents a firewall rule.
        /// </summary>
        public class FirewallRule
        {
            #region Properties

            /// <summary>
            /// Gets or sets the name of this instance.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the port to open.
            /// </summary>
            public int Port { get; set; }

            /// <summary>
            /// Gets or sets a value indicating the status of the firewall rule.
            /// </summary>
            /// <value>
            /// <c>true</c> if the rule is in effect; otherwise, <c>false</c>.
            /// </value>
            public bool Status { get; set; }

            /// <summary>
            /// Gets or sets a value indicating if the firewall rule is for
            /// TCP communications.
            /// </summary>
            /// <value>
            /// <c>true</c> if the rule is for TCP communications;
            /// <c>false</c> if the rule is for UDP communications.
            /// </value>
            public bool IsTCP { get; set; }

            #endregion
        }

        #endregion
    }
}