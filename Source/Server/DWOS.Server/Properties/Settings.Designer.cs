﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DWOS.Server.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "15.9.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Brother HL-2170W series Printer")]
        public string PrinterName {
            get {
                return ((string)(this["PrinterName"]));
            }
            set {
                this["PrinterName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int ShippingNotificationsInterval {
            get {
                return ((int)(this["ShippingNotificationsInterval"]));
            }
            set {
                this["ShippingNotificationsInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0 0 8 ? * MON")]
        public string QuoteReminderCRON {
            get {
                return ((string)(this["QuoteReminderCRON"]));
            }
            set {
                this["QuoteReminderCRON"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DWOS")]
        public string ProductID {
            get {
                return ((string)(this["ProductID"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("5")]
        public int PortalNotificationsInterval {
            get {
                return ((int)(this["PortalNotificationsInterval"]));
            }
            set {
                this["PortalNotificationsInterval"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("support@ds2.com")]
        public string DWOSSupportEmail {
            get {
                return ((string)(this["DWOSSupportEmail"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("8081")]
        public int RESTServerPort {
            get {
                return ((int)(this["RESTServerPort"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int COCNotificationsInterval {
            get {
                return ((int)(this["COCNotificationsInterval"]));
            }
            set {
                this["COCNotificationsInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseInMemoryLicenseProvider {
            get {
                return ((bool)(this["UseInMemoryLicenseProvider"]));
            }
            set {
                this["UseInMemoryLicenseProvider"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int OrderApprovalNotificationsInterval {
            get {
                return ((int)(this["OrderApprovalNotificationsInterval"]));
            }
            set {
                this["OrderApprovalNotificationsInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int OrderHoldNotificationsInterval {
            get {
                return ((int)(this["OrderHoldNotificationsInterval"]));
            }
            set {
                this["OrderHoldNotificationsInterval"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("10")]
        public int LateOrderNotificationsInterval {
            get {
                return ((int)(this["LateOrderNotificationsInterval"]));
            }
            set {
                this["LateOrderNotificationsInterval"] = value;
            }
        }
    }
}
