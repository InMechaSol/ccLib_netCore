using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ccLib_netCore
{

    public class PlatformNodeStruct
    {
        [Category("Platform Configuration")]
        [Description("A name for the platform")]
        [DisplayName("platformName")]
        public string platformName { get; set; }
        [Category("Platform Configuration")]
        [Description("Indication that this platform compiles the pure C execution system and application")]
        [DisplayName("isStraightC")]
        public bool isStraightC { get; set; }
        [Category("Platform Configuration")]
        [Description("Indication that this platform compiles the ccNOos only CPP execution system and application")]
        [DisplayName("isNOosCPP")]
        public bool isNOosCPP { get; set; }
        [Category("Platform Configuration")]
        [Description("Platform API Functions use size+1 vs only size in print/parse functions")]
        [DisplayName("platformAPIfuncPlusOne")]
        public bool platformAPIfuncPlusOne { get; set; }
        [Category("Platform Configuration")]
        [Description("Console Menu and Strings have support on this platform")]
        [DisplayName("usingConsoleMenu")]
        public bool usingConsoleMenu { get; set; }
    }
    public class ModuleNodeStruct
    {
        [Category("Module Configuration")]
        [Description("Name of the module")]
        [DisplayName("modname")]
        public string modname { get; set; }
        [Category("Module Configuration")]
        [Description("Indication that this module is an API module")]
        [DisplayName("isAPImod")]
        public bool isAPImod { get; set; }
        [Category("Module Configuration")]
        [Description("Indication that this module is a Device Module")]
        [DisplayName("isDEVmod")]
        public bool isDEVmod { get; set; }
        [Category("Module Configuration")]
        [Description("Indication that this module is ccNOos c/c++ compliant")]
        [DisplayName("isNOosMod")]
        public bool isNOosMod { get; set; }
        [Category("Module Configuration")]
        [Description("Indication that this module is to be a ccOS exeThread")]
        [DisplayName("isExeThread")]
        public bool isExeThread { get; set; }
        [Category("Module Configuration")]
        [Description("Base Module extended by this module")]
        [DisplayName("extendsModule")]
        public ModuleNodeStruct extendsModule { get; set; }
    }
    public class LibsNodeStruct
    {
        [Category("ccLibs Configuration")]
        [Description("Indication to use/not use ccOS layer")]
        [DisplayName("ccOSusing")]
        public bool ccOSusing { get; set; }
    }
    public class ApplicationNodeStruct
    {
        [Category("Application Configuration")]
        [Description("Name of Application")]
        [DisplayName("applicationName")]
        public string applicationName { get; set; }
        [Category("Application Configuration")]
        [Description("List of Compute Module Configuration Structures")]
        [DisplayName("moduleNodeStructs")]
        public List<ModuleNodeStruct> moduleNodeStructs { get; set; }
        [Category("Application Configuration")]
        [Description("List of Library Configuration Structures")]
        [DisplayName("ccLibsNodeStructs")]
        public List<LibsNodeStruct> ccLibsNodeStructs { get; set; }
    }
    public class SolutionNodeStruct
    {
        [Category("Compute Solution Configuration")]
        [Description("List of Application Configuration Structures")]
        [DisplayName("applicationNodeStructs")]
        public List<ApplicationNodeStruct> applicationNodeStructs { get; set; }
        [Category("Compute Solution Configuration")]
        [Description("List of Platform Configuration Structures")]
        [DisplayName("platformNodeStructs")]
        public List<PlatformNodeStruct> platformNodeStructs { get; set; }
    }

    public class SourceCodeGenerator
    {
    }
}
