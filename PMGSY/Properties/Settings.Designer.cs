﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PMGSY.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "11.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("http://164.100.129.6/netnrega/nregapost/pmgsyplantation.asmx")]
        public string PMGSY_NREGAWebReference_pmgsyplantation {
            get {
                return ((string)(this["PMGSY_NREGAWebReference_pmgsyplantation"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("https://pmgsytenders.gov.in/nicgep_general_webservice/services/PullTenderService." +
            "PullTenderServiceHttpsSoap11Endpoint/")]
        public string PMGSY_GePNICWebReference_PullTenderService {
            get {
                return ((string)(this["PMGSY_GePNICWebReference_PullTenderService"]));
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.SpecialSettingAttribute(global::System.Configuration.SpecialSetting.WebServiceUrl)]
        [global::System.Configuration.DefaultSettingValueAttribute("https://pmgsytenders.gov.in/nicgep_general_webservice/services/PushTenderService." +
            "PushTenderServiceHttpsSoap11Endpoint/")]
        public string PMGSY_GePNICWebReferencePush_PushTenderService {
            get {
                return ((string)(this["PMGSY_GePNICWebReferencePush_PushTenderService"]));
            }
        }
    }
}