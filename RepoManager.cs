using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;

namespace ccLib_netCore
{
    /// <summary>
    /// Repository Node Structure
    /// the Data Structure for repository nodes
    /// - name
    /// - push and fetch urls
    /// - 
    /// </summary>
    public class RepoNodeStruct
    {
        [Category("Repository Node Structure")]
        [Description("The 'Name' keystring identifying the Repository")]
        [DisplayName("Name")]
        public string name { get; set; }
        [Category("Repository Node Structure")]
        [Description("The push 'URL' keystring locating the Repository remote origin")]
        [DisplayName("URL (push)")]
        public string pushurl { get; set; }
        [Category("Repository Node Structure")]
        [Description("The fetch 'URL' keystring locating the Repository remote origin")]
        [DisplayName("URL (fetch)")]
        public string fetchurl { get; set; }
        [Category("Repository Node Structure")]
        [Description("Submodule keystrings of the Repository")]
        [DisplayName("SubModuleNames")]
        public string[] submodnames { get; set; }
        [Category("Repository Node Structure")]
        [Description("SubmoduleBranches keystrings of the Repository")]
        [DisplayName("SubModuleBranchess")]
        public string[] submodbranches { get; set; }
        [Category("Repository Node Structure")]
        [Description("Branches keystrings of the Repository")]
        [DisplayName("Branches")]
        public string[] branchnames { get; set; }
        [Category("Repository Node Structure")]
        [Description("ActiveBranch keystrings of the Repository")]
        [DisplayName("ActiveBranch")]
        public string activebranch { get; set; }
    }
    /// <summary>
    /// IMS Universe Configuration Data Structure
    /// - 3rd Party Binaries Links
    /// - The config modules list
    /// </summary>
    public class IMSConfigStruct
    {
        [Category("IMS Configuration Structure")]
        [Description("Path to git.exe, binary file")]
        [DisplayName("Path2GitBin")]
        public string Path2GitBin { get; set; }
        [Category("IMS Configuration Structure")]
        [Description("Path to doxygen.exe, binary file")]
        [DisplayName("Path2DoxygenBin")]
        public string Path2DoxygenBin { get; set; }
        [Category("IMS Configuration Structure")]
        [Description("Path to dot.exe, binary file")]
        [DisplayName("Path2GraphVizDotBin")]
        public string Path2GraphVizDotBin { get; set; }
        [Category("IMS Configuration Structure")]
        [Description("Path to Root Repository Directory")]
        [DisplayName("Path2RootRepository")]
        public string Path2RootRepository { get; set; }
        [Category("IMS Configuration Structure")]
        [Description("Flat Collection of all unique Universe Repositories")]
        [DisplayName("Repositories")]
        public List<RepoNodeStruct> Repositories { get; set; }
    }
    /// <summary>
    /// GUI TreeNode Data Structure
    /// - TreeNode elements similar to the .NET TreeNode
    /// </summary>
    public class guiTreeNode
    {
        [Category("gui Tree Node")]
        [Description("References to Parent Node Objects, [0]-rootnode [1]-immediate parent")]
        [DisplayName("ParentNodes")]
        public List<guiTreeNode> ParentNodes { set; get; }
        [Category("gui Tree Node")]
        [Description("'Name' keystring, unique identifier of node")]
        [DisplayName("Name")]
        public string Name { set; get; }
        [Category("gui Tree Node")]
        [Description("Text for Display in Tree View")]
        [DisplayName("Text")]
        public string Text { set; get; }
        [Category("gui Tree Node")]
        [Description("Text for Display in Tree View Tool Tip")]
        [DisplayName("ToolTipText")]
        public string ToolTipText { set; get; }
        [Category("gui Tree Node")]
        [Description("Tag for linking gui node with real data elements")]
        [DisplayName("Tag")]
        public object Tag { set; get; }
        [Category("gui Tree Node")]
        [Description("References to Child Nodes")]
        [DisplayName("Nodes")]
        public List<guiTreeNode> Nodes { set; get; }
    }
    /// <summary>
    /// Repository Tree Node, is a GUI Tree Node
    /// - adds Repository Node Data 
    /// </summary>
    public class repoTreeNode:guiTreeNode
    {
        [Category("repo Tree Node")]
        [Description("Configuration element from file")]
        [DisplayName("RepoConfig")]
        public RepoNodeStruct RepoConfig { set; get; }
        [Category("repo Tree Node")]
        [Description("Active Branch of the git Repository")]
        [DisplayName("ActiveBranch")]
        public string ActiveBranch { set; get; }
        [Category("repo Tree Node")]
        [Description("Indication of a 'dirty' working directory")]
        [DisplayName("hasChanges")]
        public bool hasChanges { get { if (changesString != null) { if (changesString != "") { return true; } else return false; } else return false; } }
        [Category("repo Tree Node")]
        [Description("list of changes detectected from origin/activebranch")]
        [DisplayName("changesString")]
        public string changesString { set; get; }
        [Category("repo Tree Node")]
        [Description("Indication of a working connection to Repository remote origin")]
        [DisplayName("isReachable")]
        public bool isReachable { set; get; }
        [Category("repo Tree Node")]
        [Description("Path to local working directory")]
        [DisplayName("workingDir")]
        public string workingDir { set; get; }
        [Category("repo Tree Node")]
        [Description("Sub-Level Depth (down from root repository node)")]
        [DisplayName("Depth")]
        public int Depth { get; set; }
        public repoTreeNode() {; }
        public repoTreeNode(repoTreeNode parentNode, RepoNodeStruct configStruct)
        {
            Nodes = new List<guiTreeNode>();
            ParentNodes = new List<guiTreeNode>(2);
            RepoConfig = configStruct;
            
            if(parentNode!=null)
            {
                if (parentNode.ParentNodes != null)
                {
                    if (parentNode.ParentNodes.Count > 0)
                    {
                        ParentNodes.Add(parentNode.ParentNodes[0]);
                    }
                }
                if (ParentNodes.Count == 0)
                    ParentNodes.Add(parentNode);
                ParentNodes.Add(parentNode);
            }
            else
            {
                ;
            }
            

            Name = RepoConfig.name;
            Text = RepoConfig.name;
            ActiveBranch = RepoConfig.activebranch;
            Tag = this;
            ToolTipText = RepoConfig.fetchurl;
        }        
    }
    public class RepoManager : ComputeModule
    {
        /// <summary>
        ///  The Active Configuration structure, from file
        /// </summary>
        public IMSConfigStruct IMSConfiguration { set; get; }
        /// <summary>
        /// The guiTreeNode for Configuration
        /// </summary>
        public guiTreeNode IMSConfigNode { set; get; }
        /// <summary>
        /// The guiTreeNode for Reopsitory Nodes
        /// </summary>
        public repoTreeNode RepositoryTreeRootNode { set; get; }
        /// <summary>
        /// Inidication of new config loaded
        /// </summary>
        public bool newConfigLoaded { get; set; } = false;
        /// <summary>
        /// Indication that active configuration should be updated
        /// </summary>
        public bool updateConfigflag = true;
        /// <summary>
        /// Trigger for the loop function to run "BuildRepos"
        /// </summary>
        public bool buildReposFromRemotes { get; set; } = false;
        /// <summary>
        /// Trigger for the loop function to run "CreateNewSolution"
        /// </summary>
        public bool createNewSolutionfromConfig { get; set; } = false;
        /// <summary>
        /// Trigger for the loop function to run "pushTemp2Remotes)
        /// </summary>
        public bool pushTempRepos2Remotes { get; set; } = false;
        /// <summary>
        /// Trigger for the git universe status function to run in loop()
        /// </summary>
        public bool UniverseGitStatusTrigger { get; set; } = false; 
        /// <summary>
        /// Temporary Directory Root for the repomanager
        /// </summary>
        string tempDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\RepoManager";
        string ConfigReposDir;
        string ReposDir;
        string ReposDirUverseRoot;
        string repoDirString;
        public List<repoTreeNode> repoTreeNodesfromUverseGitStatua = new List<repoTreeNode>();
        List<List<repoTreeNode>> RepositoryTreeLevelLists = new List<List<repoTreeNode>>();
        /// <summary>
        /// Constructor from Universe Repository Root Directory
        /// </summary>
        /// <param name="repoDirStringIn"></param>
        public RepoManager(string repoDirStringIn)
        {
            // create a new config struct
            IMSConfiguration = new IMSConfigStruct();
            // if the input path is not null
            if (repoDirStringIn != null)
            {
                if (Directory.Exists(Path.GetFullPath(repoDirStringIn)))
                {
                    if(repoDirStringIn.EndsWith("\\"))
                    {
                        int rIndex = repoDirStringIn.LastIndexOf("\\");
                        repoDirString = repoDirStringIn.Remove(rIndex);
                    }
                    else
                        repoDirString = repoDirStringIn;
                    
                    IMSConfiguration.Path2RootRepository = Path.GetFullPath(repoDirString);
                }
                else
                    IMSConfiguration.Path2RootRepository = "C:\\IMS";
            }
            else// if the input path was null, default to C:\\IMS
                IMSConfiguration.Path2RootRepository = "C:\\IMS";

            // set temp directory paths on the repo manager
            ConfigReposDir = tempDir + "\\ConfigurationRepos";
            ReposDir = tempDir + "\\UniverseRepos";
        }
        /// <summary>
        /// The Loop function of the Repository Manager Module
        /// </summary>
        protected override void Loop()
        {
            // if a new config has been loaded, parse and build nodes
            if(newConfigLoaded)
            {
                newConfigLoaded = false;
                BuildNodes();                
                updateConfigflag = true;
            }

            // if triggered, build repositories in temp directory using git and
            if(buildReposFromRemotes)
            {
                buildReposFromRemotes = false;
                BuildRepos();
            }

            // if triggered, create new solution from configuration file using git and code generator
            if(createNewSolutionfromConfig)
            {
                createNewSolutionfromConfig = false;
                CreateSolutionfromConfig();
            }
            
            // if triggered, check git status for universe submodules
            if (UniverseGitStatusTrigger)
            {
                UniverseGitStatusTrigger = false;
                CheckGitStatus(IMSConfiguration.Path2GitBin, IMSConfiguration.Path2RootRepository);
                updateConfigflag = true;
            }
            if (pushTempRepos2Remotes)
            {
                pushTempRepos2Remotes = false;
                PushTemp2Remotes();
            }
        }
        protected override void Setup()
        {
            NoAlarmsNoWarnings = true;
        }
        public override void SysTick()
        {
            ;
        }
        public static void DeleteFilesAndFoldersRecursively(string target_dir)
        {
            foreach (string file in Directory.GetFiles(target_dir))
            {
                File.SetAttributes(file,FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string subDir in Directory.GetDirectories(target_dir))
            {
                DeleteFilesAndFoldersRecursively(subDir);
            }

            System.Threading.Thread.Sleep(1); // This makes the difference between whether it works or not. Sleep(0) is not enough.
            File.SetAttributes(target_dir, FileAttributes.Directory);
            Directory.Delete(target_dir);
        }
        public static void copyFilesNFolders(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }
            foreach (string fstring in Directory.GetFiles(sourceDir))
            {
                //File.SetAttributes(fstring, FileAttributes.Normal);
                File.Copy(fstring, targetDir + $"\\{Path.GetFileName(fstring)}", true);
            }
        }
        public static void copyFilesNFoldersRecurrsive(string sourceDir, string targetDir)
        {
            copyFilesNFolders(sourceDir, targetDir);
            foreach (string dstring in Directory.GetDirectories(sourceDir))
                copyFilesNFoldersRecurrsive(dstring, targetDir+$"\\{dstring.Replace(sourceDir,"")}");
        }
        /// <summary>
        /// UniversefromConfig creates ??
        /// </summary>
        /// <param name="ParentDirPath"></param>
        /// <param name="Node2Transfer"></param>
        void UniversefromConfig(string ParentDirPath, repoTreeNode Node2Transfer)
        {
            string configDirPath = ConfigReposDir+$"\\{Node2Transfer.Name}";
            
            foreach(string f in Directory.GetFiles(configDirPath))
            {
                File.SetAttributes(f, FileAttributes.Normal);
                string frepo = ParentDirPath + $"\\{Node2Transfer.Name}\\{Path.GetFileName(f)}";                
                if (!Directory.Exists(Path.GetDirectoryName(frepo)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(frepo));
                }
                File.SetAttributes(Path.GetDirectoryName(frepo), FileAttributes.Directory);                  
                File.Copy(f, frepo);                
            }
            foreach (string dstring in Directory.GetDirectories(configDirPath))
            {
                string destName = ParentDirPath + $"\\{Node2Transfer.Name}\\{Path.GetFileName(dstring)}";
                if (dstring.EndsWith(".git"))
                {
                    exeSysLink.uComms.EnqueMsgString($"Universe Building: {destName}");
                    string strippedString = destName.Replace(ReposDir,"").Replace("\\.git","");
                    if (strippedString.LastIndexOf("\\")==0)
                    {
                        ReposDirUverseRoot = strippedString;//this is the root universe repository
                        copyFilesNFoldersRecurrsive(dstring, destName);
                    }
                    else
                    {
                        string submodulegitpath = destName.Replace(ReposDirUverseRoot, $"{ReposDirUverseRoot}\\.git\\modules");
                        copyFilesNFoldersRecurrsive(dstring, submodulegitpath);
                        string gitText = "gitdir: ";
                        gitText += Path.GetRelativePath(Path.GetDirectoryName(destName), submodulegitpath);
                        File.WriteAllText(destName,gitText);                        
                    }
                    if(Node2Transfer.Nodes.Count>0)
                    {
                        string moduleFname = destName.Replace(".git", ".gitmodules");
                        if (File.Exists(moduleFname))
                            File.Delete(moduleFname);
                        string modText = "";
                        foreach (repoTreeNode rNode in Node2Transfer.Nodes)
                        {
                            modText += $"[submodule \"{rNode.RepoConfig.name}\"]\r\n";
                            modText += $"\tpath = {rNode.RepoConfig.name}\r\n";
                            modText += $"\turl = {rNode.RepoConfig.fetchurl}\r\n";
                        }
                        File.WriteAllText(moduleFname, modText);
                    }
                    
                }
                else
                {
                    copyFilesNFoldersRecurrsive(dstring, destName);
                }
            }
                
        }
        void RecursiveUniversefromConfig(string ParentDirPath, repoTreeNode Node2Transfer)
        {
            if (!Directory.Exists(ParentDirPath))
            {
                Directory.CreateDirectory(ParentDirPath);
            }
            File.SetAttributes(ParentDirPath, FileAttributes.Directory);            
            UniversefromConfig(ParentDirPath, Node2Transfer);            
            foreach (repoTreeNode rNode in Node2Transfer.Nodes)
            {
                RecursiveUniversefromConfig(ParentDirPath+$"\\{Node2Transfer.Name}", rNode);
            }

        }
        /// <summary>
        /// Build Repository Tree nodes from Universe Configuration
        /// </summary>
        public void BuildRepos()
        {
            List<ExtProcCmdStruct> Cmds = new List<ExtProcCmdStruct>();
            ExtProcCmdStruct thisCmd;
            if (IMSConfigNode!=null)
            {
                if(IMSConfigNode.Nodes.Count>0)
                {

                    if (Directory.Exists(ConfigReposDir))
                    {
                        exeSysLink.uComms.EnqueMsgString($"Deleting: Begins {ConfigReposDir}");
                        DeleteFilesAndFoldersRecursively(ConfigReposDir);
                        exeSysLink.uComms.EnqueMsgString($"Deleting: Completed {ConfigReposDir}");
                    }
                    Directory.CreateDirectory(ConfigReposDir);

                    // Clone all remotes from config file to flat config directory
                    Cmds.Clear();
                    foreach (guiTreeNode tNode in IMSConfigNode.Nodes.Find(n => n.Name == "Repositories").Nodes)
                    {
                        // build commands to clone into config repos dir
                        thisCmd = new ExtProcCmdStruct();
                        thisCmd.cmdString = IMSConfiguration.Path2GitBin;
                        thisCmd.cmdArguments = $"clone {((RepoNodeStruct)tNode.Tag).fetchurl} {Path.GetFullPath(ConfigReposDir + "\\" + ((RepoNodeStruct)tNode.Tag).name)}";
                        thisCmd.workingDirString = ConfigReposDir;
                        Cmds.Add(thisCmd);
                    }

                    //Execute Clone Commands
                    exeSysLink.ThirdPartyTools.executeCMDS(Cmds);


                    // fully remove, unlink, and delete subs from config
                    // then create submodules according to config file
                    string[] rDirs = Directory.GetDirectories(ConfigReposDir);
                    List<bool> rDone = new List<bool>(rDirs.Length);
                    List<RepoNodeStruct> lastDoneList = new List<RepoNodeStruct>(rDirs.Length);
                    List<RepoNodeStruct> nextToDoList = new List<RepoNodeStruct>(rDirs.Length);
                    RepoNodeStruct thisRnode;
                    foreach (string rName in rDirs)
                    {
                        // get the list of subs from the config
                        thisRnode = IMSConfiguration.Repositories.Find(x => x.name == rName.Substring(rName.LastIndexOf("\\")+1));
                        if(thisRnode.submodnames!=null)
                            foreach(string sName in thisRnode.submodnames)
                            {
                                Cmds.Clear();

                                // build commands to rm 
                                thisCmd = new ExtProcCmdStruct();
                                thisCmd.cmdString = IMSConfiguration.Path2GitBin;
                                thisCmd.cmdArguments = $"rm {sName}";
                                thisCmd.workingDirString = ConfigReposDir + $"\\{thisRnode.name}";
                                Cmds.Add(thisCmd);

                                // build commands to commit 
                                thisCmd = new ExtProcCmdStruct();
                                thisCmd.cmdString = IMSConfiguration.Path2GitBin;
                                thisCmd.cmdArguments = $"commit -am \"*RepoManager: Removing subs from config\" ";
                                thisCmd.workingDirString = ConfigReposDir + $"\\{thisRnode.name}";
                                Cmds.Add(thisCmd);

                                //Execute Commands
                                exeSysLink.ThirdPartyTools.executeCMDS(Cmds);
                                Cmds.Clear();

                                // remove entry from config file
                                string confText = File.ReadAllText(ConfigReposDir + $"\\{thisRnode.name}\\.git\\config");
                                if (confText.Contains($"[submodule {sName}]") || confText.Contains($"[submodule \"{sName}\"]"))
                                    ;

                                // build commands to rmdir 
                                if(Directory.Exists(ConfigReposDir + $"\\{thisRnode.name}\\.git\\modules\\{sName}"))
                                    DeleteFilesAndFoldersRecursively(ConfigReposDir + $"\\{thisRnode.name}\\.git\\modules\\{sName}");


                            }

                        if (thisRnode.submodnames != null)
                            foreach (string sName in thisRnode.submodnames)
                            {
                                Cmds.Clear();

                                RepoNodeStruct thatRnode = IMSConfiguration.Repositories.Find(x => x.name == sName);
                                // build commands to submodule add 
                                thisCmd = new ExtProcCmdStruct();
                                thisCmd.cmdString = IMSConfiguration.Path2GitBin;
                                thisCmd.cmdArguments = $"submodule add {thatRnode.fetchurl} {thatRnode.name}";
                                thisCmd.workingDirString = ConfigReposDir + $"\\{thisRnode.name}";
                                Cmds.Add(thisCmd);

                                // build commands to commit 
                                thisCmd = new ExtProcCmdStruct();
                                thisCmd.cmdString = IMSConfiguration.Path2GitBin;
                                thisCmd.cmdArguments = $"commit -am \"*RepoManager: Adding subs from config\" ";
                                thisCmd.workingDirString = ConfigReposDir + $"\\{thisRnode.name}";
                                Cmds.Add(thisCmd);

                                //Execute Commands
                                exeSysLink.ThirdPartyTools.executeCMDS(Cmds);

                            }

                        if (thisRnode.submodnames == null)
                        { rDone.Add(true); lastDoneList.Add(thisRnode); nextToDoList.Add(thisRnode); }
                        else if (thisRnode.submodnames.Length == 0)
                        { rDone.Add(true); lastDoneList.Add(thisRnode); nextToDoList.Add(thisRnode); }
                        else
                        { rDone.Add(false); }
                    }
                                        
                    bool keepGoing = false;
                    do
                    {
                        // loop through the process list and mark as done
                        foreach (RepoNodeStruct r in nextToDoList)
                        {
                            Cmds.Clear();// process the repo
                            if (!(nextToDoList.FindIndex(x => x == r) == lastDoneList.FindIndex(x => x == r)))                            
                            {
                                // submodule update recursive remote

                                
                            }

                            ;// commit and push
                             // build commands to commit 
                            thisCmd = new ExtProcCmdStruct();
                            thisCmd.cmdString = IMSConfiguration.Path2GitBin;
                            thisCmd.cmdArguments = $"commit -am \"*RepoManager: Building sub module tree\" ";
                            thisCmd.workingDirString = ConfigReposDir + $"\\{r.name}";
                            Cmds.Add(thisCmd);

                            //Execute Commands
                            exeSysLink.ThirdPartyTools.executeCMDS(Cmds);

                            int asdfas=0;
                            foreach(string st in rDirs)
                            {
                                if (st.Substring(st.LastIndexOf("\\")) == r.name)
                                {
                                    rDone[asdfas] = true;// mark as done
                                }
                                asdfas++;
                            }
                                
                                    

                        }

                        keepGoing = false;
                        foreach (bool b in rDone)
                            keepGoing |= !b;

                        if(keepGoing)
                        {
                            // transfer the next-to-process list over to the lastDoneList
                            lastDoneList.Clear();
                            foreach (RepoNodeStruct r in nextToDoList)
                                lastDoneList.Add(r);

                            // loop through to determine next to process list
                            nextToDoList.Clear();
                            foreach (RepoNodeStruct r in IMSConfiguration.Repositories)
                            {
                                List<string> subModNames = new List<string>(r.submodnames);
                                foreach (RepoNodeStruct q in lastDoneList)
                                    if (subModNames.Contains(q.name))
                                        if (!nextToDoList.Contains(q))
                                            nextToDoList.Add(q);
                            }
                        }
                        


                    } while (keepGoing);
                }
            }




            if(RepositoryTreeRootNode!=null)
            {
                if(RepositoryTreeRootNode.Nodes.Count>0)
                {
                    
                    if (Directory.Exists(ReposDir))
                    {
                        exeSysLink.uComms.EnqueMsgString($"Deleting: Begins {ReposDir}");
                        DeleteFilesAndFoldersRecursively(ReposDir);
                        exeSysLink.uComms.EnqueMsgString($"Deleting: Completed {ReposDir}");
                    }
                    Directory.CreateDirectory(ReposDir);



                    ///
                    exeSysLink.uComms.EnqueMsgString($"Universe Building: Begins {ReposDir}");
                    //RecursiveUniversefromConfig(ReposDir, (repoTreeNode)RepositoryTreeRootNode.Nodes[0]);
                    exeSysLink.uComms.EnqueMsgString($"Universe Building: Completed {ReposDir}");
                    ///


                    thisCmd = new ExtProcCmdStruct();
                    thisCmd.cmdString = "explorer";
                    thisCmd.cmdArguments = ReposDir;
                    thisCmd.workingDirString = ReposDir;
                    Cmds.Add(thisCmd);
                    exeSysLink.ThirdPartyTools.executeCMDS(Cmds);

                }
            }
        }
        /// <summary>
        /// Create a new compute solution from config
        /// - Using ccOS / ccNOos
        /// - create solution directory for
        ///  - desired platform(s)
        ///  - desired application
        /// </summary>
        public void CreateSolutionfromConfig()
        {
            // create the default solution config structure for testing
            SolutionNodeStruct confSol = new SolutionNodeStruct();
            confSol.applicationNodeStructs = new List<ApplicationNodeStruct>();
            confSol.platformNodeStructs = new List<PlatformNodeStruct>();

            // the platform specification
            confSol.platformNodeStructs.Add(new PlatformNodeStruct());
            confSol.platformNodeStructs[0].platformName = "Teensy";
            confSol.platformNodeStructs[0].isStraightC = false;
            confSol.platformNodeStructs[0].platformAPIfuncPlusOne = true;
            confSol.platformNodeStructs[0].usingConsoleMenu = true;
            confSol.platformNodeStructs[0].isNOosCPP = true;

            // the application definition
            confSol.applicationNodeStructs.Add(new ApplicationNodeStruct());
            confSol.applicationNodeStructs[0].applicationName = "Lib_GripperFW";
            confSol.applicationNodeStructs[0].ccLibsNodeStructs = new List<LibsNodeStruct>();
            confSol.applicationNodeStructs[0].ccLibsNodeStructs.Add(new LibsNodeStruct());
            confSol.applicationNodeStructs[0].ccLibsNodeStructs[0].ccOSusing = true;

            // the application compute module
            confSol.applicationNodeStructs[0].moduleNodeStructs = new List<ModuleNodeStruct>();
            confSol.applicationNodeStructs[0].moduleNodeStructs.Add( new ModuleNodeStruct());
            confSol.applicationNodeStructs[0].moduleNodeStructs[0].modname = "iGripperFW";
            confSol.applicationNodeStructs[0].moduleNodeStructs[0].isAPImod = false;
            confSol.applicationNodeStructs[0].moduleNodeStructs[0].isDEVmod = false;
            confSol.applicationNodeStructs[0].moduleNodeStructs[0].isNOosMod = true;
            // the console menu api module
            confSol.applicationNodeStructs[0].moduleNodeStructs.Add(new ModuleNodeStruct());
            confSol.applicationNodeStructs[0].moduleNodeStructs[1].modname = "MenuAPI_iGripperFW";
            confSol.applicationNodeStructs[0].moduleNodeStructs[1].isAPImod = true;
            confSol.applicationNodeStructs[0].moduleNodeStructs[1].isDEVmod = false;
            confSol.applicationNodeStructs[0].moduleNodeStructs[1].isNOosMod = true;
            // the packets interface api module
            confSol.applicationNodeStructs[0].moduleNodeStructs.Add(new ModuleNodeStruct());
            confSol.applicationNodeStructs[0].moduleNodeStructs[2].modname = "PacksAPI_iGripperFW";
            confSol.applicationNodeStructs[0].moduleNodeStructs[2].isAPImod = true;
            confSol.applicationNodeStructs[0].moduleNodeStructs[2].isDEVmod = false;
            confSol.applicationNodeStructs[0].moduleNodeStructs[2].isNOosMod = true;
            // the suction device module
            confSol.applicationNodeStructs[0].moduleNodeStructs.Add(new ModuleNodeStruct());
            confSol.applicationNodeStructs[0].moduleNodeStructs[3].modname = "SuctionDEV_iGripperFW";
            confSol.applicationNodeStructs[0].moduleNodeStructs[3].isAPImod = false;
            confSol.applicationNodeStructs[0].moduleNodeStructs[3].isDEVmod = true;
            confSol.applicationNodeStructs[0].moduleNodeStructs[3].isNOosMod = true;
            // the smart motors device module
            confSol.applicationNodeStructs[0].moduleNodeStructs.Add(new ModuleNodeStruct());
            confSol.applicationNodeStructs[0].moduleNodeStructs[4].modname = "SmartMotorsDEV_iGripperFW";
            confSol.applicationNodeStructs[0].moduleNodeStructs[4].isAPImod = false;
            confSol.applicationNodeStructs[0].moduleNodeStructs[4].isDEVmod = true;
            confSol.applicationNodeStructs[0].moduleNodeStructs[4].isNOosMod = true;
            // the libraries configuration for the application
            confSol.applicationNodeStructs[0].ccLibsNodeStructs = new List<LibsNodeStruct>();
            confSol.applicationNodeStructs[0].ccLibsNodeStructs.Add(new LibsNodeStruct());
            confSol.applicationNodeStructs[0].ccLibsNodeStructs[0].ccOSusing = true;


            // Create Temp Directory for New Solution if it does'nt exist
            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }
            if (Directory.Exists(tempDir+"\\newSolution"))
            {                
                DeleteFilesAndFoldersRecursively(tempDir+"\\newSolution");
            }
            Directory.CreateDirectory(tempDir + "\\newSolution");

            // Populate Temp Directory with Libs

            // libs - pull / clone ??
            bool gitccOS = false;
            bool gitccNOos = false;
            ExtProcCmdStruct thisCmd = new ExtProcCmdStruct();
            if (confSol.applicationNodeStructs[0].ccLibsNodeStructs!=null)
            {
                if(confSol.applicationNodeStructs[0].ccLibsNodeStructs.Count>0)
                {
                    gitccNOos = true;
                    foreach (LibsNodeStruct ls in confSol.applicationNodeStructs[0].ccLibsNodeStructs)
                        if (ls.ccOSusing)
                            gitccOS = true;
                }
            }
            if (gitccNOos)
            {                
                thisCmd.cmdArguments = "clone -b main https://github.com/InMechaSol/ccNOos.git";
                thisCmd.timeOutms = 10000;
                thisCmd.cmdString = IMSConfiguration.Path2GitBin;
                thisCmd.workingDirString = tempDir + "\\newSolution";

                List<ExtProcCmdStruct> cmdsIn = new List<ExtProcCmdStruct>();
                cmdsIn.Add(thisCmd);
                exeSysLink.ThirdPartyTools.executeCMDS(cmdsIn);
                string ccNOosCloneString = thisCmd.outANDerrorResults;
            }
            if (gitccOS)
            {
                thisCmd.cmdArguments = "clone -b main https://github.com/InMechaSol/ccOS.git";
                thisCmd.timeOutms = 10000;
                thisCmd.cmdString = IMSConfiguration.Path2GitBin;
                thisCmd.workingDirString = tempDir + "\\newSolution";

                List<ExtProcCmdStruct> cmdsIn = new List<ExtProcCmdStruct>();
                cmdsIn.Add(thisCmd);
                exeSysLink.ThirdPartyTools.executeCMDS(cmdsIn);
                string ccNOosCloneString = thisCmd.outANDerrorResults;
            }

            // just for now, lets copy the template dirs
            copyFilesNFoldersRecurrsive(tempDir + "\\newSolution\\ccNOos\\templates\\Lib_GripperFW_Tests", tempDir + "\\newSolution");

            // Create Sub Directory(ies) for Application(s)
            if(confSol.applicationNodeStructs!=null)
            {
                if(confSol.applicationNodeStructs.Count>0)
                {
                    foreach(ApplicationNodeStruct an in confSol.applicationNodeStructs)
                    {
                        ;
                    }
                }
            }

            // Populate Application Directoies

            // application
            // app - api modules             
            // app - device modules

            // Create Sub Directory(ies) for Platform(s)

            // Populate Platform Directories

            // main files            
            // platform(s)




        }
        public void PushTemp2Remotes()
        {
            for(int i = RepositoryTreeLevelLists.Count - 1; i>=0; i--)
            {
                List<repoTreeNode> ChangesList = new List<repoTreeNode>();
                List<repoTreeNode> NoChangeList = new List<repoTreeNode>();
                List<ExtProcCmdStruct> cmds = new List<ExtProcCmdStruct>();
                ExtProcCmdStruct thisCmd = new ExtProcCmdStruct();
                thisCmd.cmdString = IMSConfiguration.Path2GitBin;
                thisCmd.cmdArguments = "status --short";
                string tempV = IMSConfiguration.Path2RootRepository;
                foreach (repoTreeNode rNode in RepositoryTreeLevelLists[i])
                {
                    // check for local changes
                    cmds.Clear();// one at a time...
                    IMSConfiguration.Path2RootRepository = Directory.GetDirectories(ReposDir)[0];
                    thisCmd.workingDirString = expectedWorkingDirectoryPath(rNode);                    
                    cmds.Add(thisCmd);
                    exeSysLink.ThirdPartyTools.executeCMDS(cmds);
                    if (cmds[0].outANDerrorResults.Contains("up to date") && cmds[0].outANDerrorResults.Contains("Untracked files:  ("))
                    {
                        NoChangeList.Add(rNode);
                    }
                    else
                    {
                        ChangesList.Add(rNode);
                    }
                        
                }
                IMSConfiguration.Path2RootRepository = tempV;
                


                foreach (repoTreeNode rNode in ChangesList)
                {

                    // commit local
                    //thisCmd.cmdArguments = "commit"
                    //cmds.Clear();// one at a time...
                    //IMSConfiguration.Path2RootRepository = Directory.GetDirectories(ReposDir)[0];
                    //thisCmd.workingDirString = expectedWorkingDirectoryPath(rNode);
                    //cmds.Add(thisCmd);
                    //exeSysLink.ThirdPartyTools.executeCMDS(cmds);
                    //if (cmds[0].outANDerrorResults.Contains("up to date") && cmds[0].outANDerrorResults.Contains("Untracked files:  ("))
                    //{
                    //    NoChangeList.Add(rNode);
                    //}
                    //else
                    //{
                    //    ChangesList.Add(rNode);
                    //}

                    // pull from remote


                    // push remote

                    //
                    ;

                }

                foreach (repoTreeNode rNode in NoChangeList)
                {

                    // pull from remote no conflict
                    ;
                    // 
                }
            }
        }
        public string expectedWorkingDirectoryPath(repoTreeNode rNode)
        {
            string outstrig = "";

            List<string> parentNames = new List<string>();

            bool keepGoing = true;
            repoTreeNode thisNode = rNode;
            do
            {
                parentNames.Add(thisNode.Name);
                if (thisNode.ParentNodes != null)
                {
                    if (thisNode.ParentNodes[0] == thisNode.ParentNodes[1])
                    {
                        keepGoing = false;
                    }
                    else
                        thisNode = (repoTreeNode)thisNode.ParentNodes[1];
                }
                else
                    keepGoing = false;

            } while (keepGoing);

            outstrig += IMSConfiguration.Path2RootRepository;
            for (int i = parentNames.Count - 2; i >= 0; i--)
            {
                outstrig += "\\" + parentNames[i];
            }
            return outstrig;
        }
        /// <summary>
        /// Univers git Status
        /// - call from universe repo to determine status of all submodules
        /// </summary>
        /// <param name="cmdStringIn">the path to the git.exe application</param>
        /// <param name="wrkNDir"> the working directory of the "IMS" folder</param>
        private void CheckGitStatus(string cmdStringIn, string wrkNDir)
        {
            // What does git think about it?
            ExtProcCmdStruct thisCmd = new ExtProcCmdStruct();
            thisCmd.cmdArguments = "submodule foreach --recursive git remote -v";
            thisCmd.timeOutms = 5000;
            thisCmd.cmdString = cmdStringIn;
            thisCmd.workingDirString = wrkNDir;

            List<ExtProcCmdStruct> cmdsIn = new List<ExtProcCmdStruct>();
            cmdsIn.Add(thisCmd);
            exeSysLink.ThirdPartyTools.executeCMDS(cmdsIn);
            string remoteString = thisCmd.outANDerrorResults;

            thisCmd.cmdArguments = "submodule foreach --recursive git branch";
            exeSysLink.ThirdPartyTools.executeCMDS(cmdsIn);
            string branchString = thisCmd.outANDerrorResults;

            thisCmd.cmdArguments = "submodule foreach --recursive git status -b --porcelain";
            exeSysLink.ThirdPartyTools.executeCMDS(cmdsIn);
            string statusString = thisCmd.outANDerrorResults;

            ParseUniverseGitStrings(remoteString, branchString, statusString);
        }
        private void ParseGitStringRemote(string remoteStringIn)
        {
            int parseIndex = remoteStringIn.IndexOf("out:");
            string remoteSubString = remoteStringIn.Substring(parseIndex+ "out:".Length);
            // out:
            if (parseIndex==0)
            { 
                string[] tokens = remoteSubString.Split("Entering ");
                
                repoTreeNodesfromUverseGitStatua = new List<repoTreeNode>(tokens.Length);


                foreach (string tok in tokens)
                {
                    string RepoDirNameString = null;
                    string FecthURLString = null;
                    string PushURLString = null;
                    parseIndex = 0;

                    if (tok != "\n")
                    {
                        //Regex rx = new Regex(@"^('(?<repoDirName>\S+)'origin (?<fetchURL>\S+) (fetch)origin (?<pushURL>\S+) (push))");
                        //MatchCollection matches = rx.Matches(tok);
                        RepoDirNameString = (tok.Remove(tok.LastIndexOf('\''))).Substring(1);
                        parseIndex += (2 + RepoDirNameString.Length);
                        remoteSubString = tok.Substring(parseIndex);
                        FecthURLString = (remoteSubString.Substring(remoteSubString.IndexOf('\t') + 1)).Remove(remoteSubString.IndexOf(".git") - 3);
                        parseIndex += (15 + FecthURLString.Length);
                        remoteSubString = tok.Substring(parseIndex);
                        PushURLString = (remoteSubString.Substring(remoteSubString.IndexOf('\t') + 1)).Remove(remoteSubString.IndexOf(".git") - 3);
                        repoTreeNodesfromUverseGitStatua.Add(new repoTreeNode(null,new RepoNodeStruct()));
                        repoTreeNodesfromUverseGitStatua[repoTreeNodesfromUverseGitStatua.Count - 1].RepoConfig.fetchurl = FecthURLString;
                        repoTreeNodesfromUverseGitStatua[repoTreeNodesfromUverseGitStatua.Count - 1].RepoConfig.name = RepoDirNameString;
                        repoTreeNodesfromUverseGitStatua[repoTreeNodesfromUverseGitStatua.Count - 1].Name = RepoDirNameString;
                        repoTreeNodesfromUverseGitStatua[repoTreeNodesfromUverseGitStatua.Count - 1].Text = RepoDirNameString;
                        repoTreeNodesfromUverseGitStatua[repoTreeNodesfromUverseGitStatua.Count - 1].RepoConfig.pushurl = PushURLString;
                    }
                    else
                        ;

                }
                // Entering '<RepoDirName>'origin <fetchurl> (fetch)origin <pushurl> (push)

                //Regex rx = new Regex(@"^(Entering '(?<repoDirName>[a-z1-9]+{1})'origin (?<fetchURL>*{1}) (fetch)origin (?<pushURL>*{1}) (push)){1}");
                //MatchCollection matches = rx.Matches(remoteSubString);
                //;
            }            
        }
        private void ParseGitStringBranch(string branchStringIn)
        {
            int parseIndex = branchStringIn.IndexOf("out:");
            string remoteSubString = branchStringIn.Substring(parseIndex + "out:".Length);
            // out:
            if (parseIndex == 0)
            {
                string[] tokens = remoteSubString.Split("Entering");

                foreach (string tok in tokens)
                {
                    
                    string RepoBranchesString = null;
                    

                    if (tok != "\n")
                    {
                        //Regex rx = new Regex(@"^('(?<repoDirName>\S+)'origin (?<fetchURL>\S+) (fetch)origin (?<pushURL>\S+) (push))");
                        //MatchCollection matches = rx.Matches(tok);
                        RepoBranchesString = tok.Substring(tok.IndexOf("* ")+2);
                        repoTreeNodesfromUverseGitStatua[parseIndex++].RepoConfig.branchnames = RepoBranchesString.Split(' ', StringSplitOptions.RemoveEmptyEntries);                        
                    }
                }
            }
        }
        private void ParseGitStringStatus(string statusStringIn)
        {
            int parseIndex = statusStringIn.IndexOf("out:");
            string remoteSubString = statusStringIn.Substring(parseIndex + "out:".Length);
            // out:
            if (parseIndex == 0)
            {
                string[] tokens = remoteSubString.Split("Entering");

                foreach (string tok in tokens)
                {
                    string ActiveBranchString = null;
                    string ChangesString = null;

                    if (tok != "\n")
                    {
                        ActiveBranchString = (tok.Substring(tok.IndexOf("## ") + 3));
                        ActiveBranchString = ActiveBranchString.Remove(ActiveBranchString.IndexOf("..."));
                        repoTreeNodesfromUverseGitStatua[parseIndex].RepoConfig.activebranch = ActiveBranchString;
                        repoTreeNodesfromUverseGitStatua[parseIndex].ActiveBranch = repoTreeNodesfromUverseGitStatua[parseIndex].RepoConfig.activebranch;
                        ChangesString = (tok.Substring(tok.LastIndexOf(ActiveBranchString) + ActiveBranchString.Length));
                        repoTreeNodesfromUverseGitStatua[parseIndex].changesString = ChangesString;
                        parseIndex++;
                    }
                    
                }
            }
        }
        private void ParseUniverseGitStrings(string remoteStringIn, string branchStringIn, string statusStringIn)
        {
            // parse the remote string
            ParseGitStringRemote(remoteStringIn);

            // parse the branch string
            ParseGitStringBranch(branchStringIn);

            // parse the status string
            ParseGitStringStatus(statusStringIn);
        }
        public IMSConfigStruct CreateIMSConfigStructfromUverseStatus(string inputString)
        {
            if (!Directory.Exists(Path.GetFullPath(inputString)))
            {
                Directory.CreateDirectory(Path.GetFullPath(inputString));
                return CreateDefaultIMSConfigStruct(inputString);
            }
            IMSConfigStruct outStruct =     new IMSConfigStruct();
            outStruct.Path2RootRepository = Path.GetFullPath(inputString);
            outStruct.Path2DoxygenBin =     "C:\\Program Files\\doxygen\\bin\\doxygen.exe";
            outStruct.Path2GitBin =         "C:\\Program Files\\Git\\bin\\git.exe";
            outStruct.Path2GraphVizDotBin = "C:\\Program Files\\Graphviz\\bin\\dot.exe";

            CheckGitStatus(outStruct.Path2GitBin, outStruct.Path2RootRepository);

            outStruct.Repositories = new List<RepoNodeStruct>();

            RepoNodeStruct tempNode = new RepoNodeStruct();
            tempNode.name = "IMI";
            tempNode.fetchurl = "https://github.com/NORGREN-AUTOMATION/IMS.git";
            tempNode.pushurl = "https://github.com/NORGREN-AUTOMATION/IMS.git";
            tempNode.submodnames = new string[] { "CR", "CS", "P" };
            outStruct.Repositories.Add(tempNode);

            string tempString = "";
            // loop through the list of nodes generated by git status checks
            foreach(repoTreeNode rTN in repoTreeNodesfromUverseGitStatua)
            {
                // if an entry with mathcing fetch url does not already exists in the lists
                if(!outStruct.Repositories.Exists(x=>x.fetchurl==rTN.RepoConfig.fetchurl))
                {
                    // Create and add the configuration repository
                    tempNode = new RepoNodeStruct();

                    // if this is a sub repo
                    if (rTN.RepoConfig.name.Contains('/'))
                    {
                        // capture name of repo
                        tempNode.name = rTN.RepoConfig.name.Substring(rTN.RepoConfig.name.LastIndexOf('/') + 1);
                    }
                    else
                        tempNode.name = rTN.RepoConfig.name;
                    tempNode.fetchurl = rTN.RepoConfig.fetchurl;
                    tempNode.pushurl = rTN.RepoConfig.pushurl;
                    tempNode.branchnames = rTN.RepoConfig.branchnames;
                    //tempNode.activebranch = rTN.RepoConfig.activebranch;
                    outStruct.Repositories.Add(tempNode);
                }
            }

            // loop through the list of nodes generated by git status checks
            foreach (repoTreeNode rTN in repoTreeNodesfromUverseGitStatua)
            {
                // Create and add the configuration repository
                tempNode = new RepoNodeStruct();

                // if this is a sub repo
                if (rTN.RepoConfig.name.Contains('/'))
                {
                    // capture name of repo
                    tempNode.name = rTN.RepoConfig.name.Substring(rTN.RepoConfig.name.LastIndexOf('/') + 1);
                    tempString = rTN.RepoConfig.name.Remove(rTN.RepoConfig.name.LastIndexOf('/'));//.Substring(rTN.RepoConfig.name.LastIndexOf('/') + 1);
                                                                                                  // capture name of parent repo
                    tempString = tempString.Substring(tempString.LastIndexOf('/') + 1);
                    // check if parent repo is already in config repos (it should be)
                    if (outStruct.Repositories.Find(x => x.name == tempString) != null)
                    {
                        if (outStruct.Repositories.Find(x => x.name == tempString).submodnames != null)
                        {
                            if (!outStruct.Repositories.Find(x => x.name == tempString).submodnames.Contains(tempNode.name))
                            {
                                List<string> stletp = new List<string>(outStruct.Repositories.Find(x => x.name == tempString).submodnames);
                                stletp.Add(tempNode.name);
                                outStruct.Repositories.Find(x => x.name == tempString).submodnames = stletp.ToArray();
                            }
                        }
                        else
                        {
                            outStruct.Repositories.Find(x => x.name == tempString).submodnames = new string[] { tempNode.name };
                        }

                    }

                }

            }

            return outStruct;
        }
        /// <summary>
        /// Build TreeNodes when configuration is loaded/changed
        /// </summary>
        private void BuildNodes()
        {
            // Determine if default config
            if (IMSConfiguration.Repositories.Count == 8 &&
                IMSConfiguration.Repositories[1].fetchurl == "https://github.com/InMechaSol/CR.git" &&
                IMSConfiguration.Repositories[2].fetchurl == "https://github.com/InMechaSol/CS.git" &&
                IMSConfiguration.Repositories[3].fetchurl == "https://github.com/InMechaSol/P.git" &&
                IMSConfiguration.Repositories[4].fetchurl == "https://github.com/InMechaSol/ccNOos.git" &&
                IMSConfiguration.Repositories[5].fetchurl == "https://github.com/InMechaSol/ccNOos_Tests.git" &&
                IMSConfiguration.Repositories[6].fetchurl == "https://github.com/InMechaSol/ccOS.git" &&
                IMSConfiguration.Repositories[7].fetchurl == "https://github.com/InMechaSol/ccOS_Tests.git"
                )
            {
                // Run git status if so
                IMSConfiguration = CreateIMSConfigStructfromUverseStatus(IMSConfiguration.Path2RootRepository);
            }
                

            // Create a Config Node from the configuration settings
            IMSConfigNode = createGUItreeNodefromConfig(IMSConfiguration);
            // Create a Repo Node from 
            RepositoryTreeRootNode = createREPOtreeNodefromRepoList(IMSConfiguration.Repositories);



            ((repoTreeNode)RepositoryTreeRootNode.Nodes[0]).Depth = 0;  
            RecursiveDetectWorkingDirectoryStatus((repoTreeNode)RepositoryTreeRootNode.Nodes[0]);            
        }
        /// <summary>
        /// Detect Status of Working Directory
        /// - called recursively
        /// </summary>
        /// <param name="rootRepoNode"></param>
        public void DetectWorkingDirectoryStatus(repoTreeNode rootRepoNode)
        {
            rootRepoNode.isReachable = false;

            string sName = expectedWorkingDirectoryPath(rootRepoNode);

            // First things first, does this directory exist in the file system?
            if (Directory.Exists(sName))
            {
                rootRepoNode.workingDir = sName;

                          

            }
            else
                rootRepoNode.workingDir = "Not Detected";


            




            // Categorize Repository Tree Node by level/depth
            if (RepositoryTreeLevelLists.Count <= rootRepoNode.Depth)
            {
                RepositoryTreeLevelLists.Add(new List<repoTreeNode>());
            }
            if (!RepositoryTreeLevelLists[rootRepoNode.Depth].Contains(rootRepoNode))
                RepositoryTreeLevelLists[rootRepoNode.Depth].Add(rootRepoNode);


        }
        /// <summary>
        /// The Recursive Detection Working Directory Status Function
        /// </summary>
        /// <param name="rootRepoNode"></param>
        public void RecursiveDetectWorkingDirectoryStatus(repoTreeNode rootRepoNode)
        {
            // detect status for self
            DetectWorkingDirectoryStatus(rootRepoNode);

            // detect status of branches recursively
            foreach (repoTreeNode rNode in rootRepoNode.Nodes)
            {
                rNode.Depth = rootRepoNode.Depth + 1;                
                RecursiveDetectWorkingDirectoryStatus(rNode);
            }
        }
        public static guiTreeNode createGUItreeNodefromConfig(IMSConfigStruct refConfig)
        {
            guiTreeNode tempNode = new guiTreeNode();
            tempNode.Nodes = new List<guiTreeNode>();
            
            tempNode.Name = "RepoConfigList";
            tempNode.Text = "IMS Configuration List";
            tempNode.ToolTipText = refConfig.Path2RootRepository + "\\imsConf.json";
            tempNode.Tag = refConfig;

            guiTreeNode tempSubNode = new guiTreeNode();            
            tempSubNode.ParentNodes = new List<guiTreeNode>(2);
            tempSubNode.ParentNodes.Add(tempNode);
            tempSubNode.ParentNodes.Add(tempNode);
            tempSubNode.Name = "RootRepoPath";
            tempSubNode.Text = "Path to Repository Root";
            tempSubNode.ToolTipText = refConfig.Path2RootRepository;
            tempSubNode.Tag = refConfig.Path2RootRepository;
            
            tempNode.Nodes.Add(tempSubNode);

            tempSubNode = new guiTreeNode();
            tempSubNode.ParentNodes = new List<guiTreeNode>(2);
            tempSubNode.ParentNodes.Add(tempNode);
            tempSubNode.ParentNodes.Add(tempNode);
            tempSubNode.Name = "DoxygenPath";
            tempSubNode.Text = "Path to doxygen.exe";
            tempSubNode.ToolTipText = refConfig.Path2DoxygenBin;
            tempSubNode.Tag = refConfig.Path2DoxygenBin;
            tempNode.Nodes.Add(tempSubNode);

            tempSubNode = new guiTreeNode();
            tempSubNode.ParentNodes = new List<guiTreeNode>(2);
            tempSubNode.ParentNodes.Add(tempNode);
            tempSubNode.ParentNodes.Add(tempNode);
            tempSubNode.Name = "GitPath";
            tempSubNode.Text = "Path to git.exe";
            tempSubNode.ToolTipText = refConfig.Path2GitBin;
            tempSubNode.Tag = refConfig.Path2GitBin;
            tempNode.Nodes.Add(tempSubNode);

            tempSubNode = new guiTreeNode();
            tempSubNode.ParentNodes = new List<guiTreeNode>(2);
            tempSubNode.ParentNodes.Add(tempNode);
            tempSubNode.ParentNodes.Add(tempNode);
            tempSubNode.Name = "GraphvizDotPath";
            tempSubNode.Text = "Path to dot.exe";
            tempSubNode.ToolTipText = refConfig.Path2GraphVizDotBin;
            tempSubNode.Tag = refConfig.Path2GraphVizDotBin;
            tempNode.Nodes.Add(tempSubNode);

            tempSubNode = new guiTreeNode();
            tempSubNode.Nodes = new List<guiTreeNode>();
            tempSubNode.ParentNodes = new List<guiTreeNode>(2);
            tempSubNode.ParentNodes.Add(tempNode);
            tempSubNode.ParentNodes.Add(tempNode);
            tempSubNode.Name = "Repositories";
            tempSubNode.Text = "Configuration Repositories";
            tempSubNode.ToolTipText = $"{refConfig.Repositories.Count} Repository List";
            tempSubNode.Tag = refConfig.Repositories;
            

            int counter = 0;            
            foreach (RepoNodeStruct rStruct in refConfig.Repositories)
            {
                guiTreeNode tempSubSubNode = new guiTreeNode();
                tempSubSubNode.ParentNodes = new List<guiTreeNode>(2);
                tempSubSubNode.ParentNodes.Add(tempNode);
                tempSubSubNode.ParentNodes.Add(tempSubNode);
                tempSubSubNode.Name = $"RepoNode{counter}";
                tempSubSubNode.Text = rStruct.name;
                tempSubSubNode.ToolTipText = "";
                if (refConfig.Repositories[counter].submodnames != null)
                {
                    if (refConfig.Repositories[counter].submodnames.Length > 0)
                    {
                        tempSubSubNode.ToolTipText += "Subs: ";
                        foreach (string s in refConfig.Repositories[counter].submodnames)
                            tempSubSubNode.ToolTipText += $"{s}, ";
                    }
                    else
                        tempSubSubNode.ToolTipText += "No Subs ";
                }
                else
                    tempSubSubNode.ToolTipText += "No Subs ";
                tempSubSubNode.Tag = refConfig.Repositories[counter];                
                tempSubNode.Nodes.Add(tempSubSubNode);
                counter++;
            }
            tempNode.Nodes.Add(tempSubNode);
            return tempNode;
        }

        public repoTreeNode createREPOtreeNodefromRepoList(List<RepoNodeStruct> refList)
        {
            
            repoTreeNode tempNode = new repoTreeNode(); 
            tempNode.Nodes = new List<guiTreeNode>();
            tempNode.Name = "RepoDirectoryTree";
            tempNode.Text = "Repository Directories";


            List<string> Names = new List<string>();
            List<string> SubNames = new List<string>();
            List<bool> NamesMatched = new List<bool>();
            foreach(RepoNodeStruct rStruct in refList)
            {
                Names.Add(rStruct.name);
                NamesMatched.Add(false);
                if(rStruct.submodnames!=null)
                    foreach (string s in rStruct.submodnames)
                        SubNames.Add(s);
            }
            int counter = 0;
            foreach(string s in Names)
            {                
                foreach(string S in SubNames)
                {
                    if(s == S)
                    {
                        NamesMatched[counter] = true;
                        break;
                    }    
                }
                if (!NamesMatched[counter])
                    break;
                counter++;
            }

            repoTreeNode tempSubNode = new repoTreeNode(tempNode, refList[counter]);

            ///////////////
            //// Recursive Function Call - Build all Repo Nodes
            //////////////
            BuildallRepoNodes(tempSubNode, refList);
            
            tempNode.Nodes.Add(tempSubNode);
            return tempNode;
        }
        public void BuildallRepoNodes(repoTreeNode tempSubNode, List<RepoNodeStruct> refList)
        {
            // Loop through all child nodes of current node
            repoTreeNode currentNode = tempSubNode;
            

            if (currentNode.RepoConfig.submodnames != null)
            {
                int counter = 0;
                foreach (string s in currentNode.RepoConfig.submodnames)
                {
                    RepoNodeStruct childConfig = refList.Find(r => r.name == s);
                    repoTreeNode nextChildNode = new repoTreeNode(currentNode, childConfig);
                    

                    if(childConfig.submodnames!=null)
                    {
                        if (childConfig.submodnames.Length > 0)
                        {
                            BuildallRepoNodes(nextChildNode, refList);
                            
                        }
                    }

                    currentNode.Nodes.Add(nextChildNode);
                    counter++;
                }
            }
        }
        public static IMSConfigStruct CreateDefaultIMSConfigStruct(string inputString)
        {
            IMSConfigStruct outStruct = new IMSConfigStruct();
            if(Directory.Exists(Path.GetFullPath(inputString)))
                outStruct.Path2RootRepository = Path.GetFullPath(inputString);
            else
                outStruct.Path2RootRepository = "C:\\IMS";

            outStruct.Path2DoxygenBin = "C:\\Program Files\\doxygen\\bin\\doxygen.exe";
            outStruct.Path2GitBin = "C:\\Program Files\\Git\\bin\\git.exe";
            outStruct.Path2GraphVizDotBin = "C:\\Program Files\\Graphviz\\bin\\dot.exe";
            outStruct.Repositories = new List<RepoNodeStruct>();            

            RepoNodeStruct tempNode = new RepoNodeStruct();
            tempNode.name = "IMS";
            tempNode.fetchurl = "https://github.com/InMechaSol/IMS.git";
            tempNode.submodnames = new string[] { "CR", "CS", "P" };
            outStruct.Repositories.Add(tempNode);

            tempNode = new RepoNodeStruct();
            tempNode.name = "CR";
            tempNode.fetchurl = "https://github.com/InMechaSol/CR.git";
            tempNode.submodnames = new string[] { "ccOS", "ccOS_Tests", "ccNOos", "ccNOos_Tests" };
            outStruct.Repositories.Add(tempNode);

            tempNode = new RepoNodeStruct();
            tempNode.name = "CS";
            tempNode.fetchurl = "https://github.com/InMechaSol/CS.git";
            outStruct.Repositories.Add(tempNode);

            tempNode = new RepoNodeStruct();
            tempNode.name = "P";
            tempNode.fetchurl = "https://github.com/InMechaSol/P.git";
            outStruct.Repositories.Add(tempNode);            

            tempNode = new RepoNodeStruct();
            tempNode.name = "ccNOos";
            tempNode.fetchurl = "https://github.com/InMechaSol/ccNOos.git";
            outStruct.Repositories.Add(tempNode);

            tempNode = new RepoNodeStruct();
            tempNode.name = "ccNOos_Tests";
            tempNode.fetchurl = "https://github.com/InMechaSol/ccNOos_Tests.git";
            tempNode.submodnames = new string[] { "ccNOos" };
            outStruct.Repositories.Add(tempNode);

            tempNode = new RepoNodeStruct();
            tempNode.name = "ccOS";
            tempNode.fetchurl = "https://github.com/InMechaSol/ccOS.git";
            outStruct.Repositories.Add(tempNode);

            tempNode = new RepoNodeStruct();
            tempNode.name = "ccOS_Tests";
            tempNode.fetchurl = "https://github.com/InMechaSol/ccOS_Tests.git";
            tempNode.submodnames = new string[] { "ccOS" };
            outStruct.Repositories.Add(tempNode);

            return outStruct;
        }
        public static RepoNodeStruct CreateRepoNodeStructJSON(ref byte[] jsonString)
        {            
            return Platform_Serialization.tryParseRepoNodeStruct(ref jsonString);
        }
        public static byte[] SerializeRepoNodeStructJSON(RepoNodeStruct outStruct)
        {            
            return Platform_Serialization.packageRepoNodeStruct(ref outStruct);
        }
        public static IMSConfigStruct CreateIMSConfigStructJSON(ref byte[] jsonString)
        {
            return Platform_Serialization.tryParseIMSConfigStruct(ref jsonString);
        }
        public static byte[] SerializeIMSConfigStructJSON(IMSConfigStruct outStruct)
        {
            return Platform_Serialization.packageIMSConfigStruct(ref outStruct);
        }







        //public static void DeleteFilesAndFoldersRecursively(string target_dir)
        //{
        //    foreach (string file in Directory.GetFiles(target_dir))
        //    {
        //        File.Delete(file);
        //    }

        //    foreach (string subDir in Directory.GetDirectories(target_dir))
        //    {
        //        DeleteFilesAndFoldersRecursively(subDir);
        //    }

        //    System.Threading.Thread.Sleep(1); // This makes the difference between whether it works or not. Sleep(0) is not enough.
        //    Directory.Delete(target_dir);
        //}
        public static string LicenseText()
        {
            return @"
    Copyright 2021 <a href=""https://www.inmechasol.org/"" target=""_blank"">InMechaSol, Inc</a>    

    Licensed under the Apache License, Version 2.0(the ""License"");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an ""AS IS"" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.";
        }
        public static void FixFileBriefNLicense(string fstring)
        {
            bool fileNeedsNewHeaderText = true;
            string ftypeText = "HowUnlikelyWouldItBe@if thisEx@ctTextWasN8";
            if (Path.GetExtension(fstring) == ".h")
            {
                if (Path.GetFileName(fstring).Contains("Platform_"))
                    ftypeText = "Platform Specification, " + Path.GetFileNameWithoutExtension(fstring).Replace("Platform_", "");
                else
                    ftypeText = "Declarations for straight C and C++";
            }
            else if (Path.GetExtension(fstring) == ".cpp" || Path.GetExtension(fstring) == ".ino")
            {
                ftypeText = "Implementation for C++ wrappers";
            }
            else if (Path.GetExtension(fstring) == ".c")
            {
                ftypeText = "Implementation for straight C";
            }
            else
                return;


            Console.WriteLine("Processing: " + fstring);
            string fileText = File.ReadAllText(fstring);


            if (fileText.StartsWith("/** \\file " + Path.GetFileName(fstring)))
            {
                if (fileText.Contains("\n*   \\brief <a href=\"https://www.inmechasol.org/\" target=\"_blank\">IMS</a>:\r\n\t\t<a href=\"https://github.com/InMechaSol/ccNOos\" target=\"_blank\">ccNOos</a>,\r\n\t\t"))
                {
                    if (fileText.Contains(ftypeText))
                    {
                        if (fileText.Contains("\r\n\r\nNotes:"))
                        {
                            if (fileText.Contains("(.c includes .h) - for straight C") && fileText.Contains("(.cpp includes .c which includes .h) - for C++ wrapped straight C") && fileText.Contains("Always compiled to a single compilation unit, either C or CPP, not both"))
                            {
                                if (fileText.Contains(@"Copyright 2021 <a href=""https://www.inmechasol.org/"" target=""_blank"">InMechaSol, Inc</a>"))
                                {
                                    if (fileText.Contains("Licensed under the Apache License, Version 2.0(the \"License\");") && fileText.Contains("you may not use this file except in compliance with the License.") && fileText.Contains("You may obtain a copy of the License at"))
                                    {
                                        if (fileText.Contains("http://www.apache.org/licenses/LICENSE-2.0"))
                                        {
                                            if (fileText.Contains("Unless required by applicable law or agreed to in writing, software") &&
                                                fileText.Contains("distributed under the License is distributed on an \"AS IS\" BASIS,") &&
                                                fileText.Contains("WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.") &&
                                                fileText.Contains("See the License for the specific language governing permissions and") &&
                                                fileText.Contains("limitations under the License.")
                                                )
                                            {
                                                fileNeedsNewHeaderText = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (fileNeedsNewHeaderText)
            {
                int j = fileText.IndexOf("\n#");
                int k = fileText.IndexOf("\n/*");
                int i = fileText.IndexOf("*/");
                int h = 0;
                if (j > -1)// first compiler statement (#ifndef, #include, ... ) 
                {
                    if (k > -1)// opened comment
                    {
                        if (i > k && i < j)// closed comment before first compiler statement
                        {
                            h = j - 1; // clear from compiler statement to top of file
                        }
                    }
                    else// never opened comment
                    {
                        h = j - 1; // clear from compiler statement to top of file
                    }
                }
                fileText = fileText.Substring(h);
                fileText = fileText.Substring(fileText.IndexOf("\n#"));
                fileText = "/** \\file " + Path.GetFileName(fstring) + "\r\n*   \\brief <a href=\"https://www.inmechasol.org/\" target=\"_blank\">IMS</a>:\r\n\t\t<a href=\"https://github.com/InMechaSol/ccNOos\" target=\"_blank\">ccNOos</a>,\r\n\t\t" + ftypeText + " \r\n" + LicenseText() + "\r\n\r\nNotes:\r\n\t(.c includes .h) - for straight C or\r\n\t(.cpp includes .c which includes .h) - for C++ wrapped straight C\r\n\t*Always compiled to a single compilation unit, either C or CPP, not both\r\n\r\n*/\r\n" + fileText;

                File.WriteAllText(fstring, fileText);

            }
        }
        
        // this is called for each directory in the ccNOos directory
        public static void CopyDirectoryArduinoMod(string sourceDir, string destinationDir)
        {
            // process each file
            foreach (string fstring in Directory.GetFiles(sourceDir))
            {
                // fstring - path to source file
                
                // filestring - path to destination file
                string filestring = destinationDir + "\\" + fstring.Substring(fstring.LastIndexOf("\\") + 1);

                if (Path.GetExtension(fstring).Equals(".c"))
                {
                    // copy and change extension to hpp
                    File.Copy(fstring, filestring.Replace(".c", ".hpp"), true);
                }
                else if (Path.GetExtension(fstring).Equals(".cpp"))
                {

                    // copy and modify #include line within
                    string ftext = File.ReadAllText(fstring);

                    Regex rx = new Regex(@"#include .*\.([c])");
                    MatchCollection matches = rx.Matches(ftext);
                    foreach (Match m in matches)
                    {
                        string[] toks = m.Value.Split('.');
                        ftext = rx.Replace(ftext, toks[0] + ".hpp");
                    }
                    File.WriteAllText(filestring, ftext);
                    //}

                }
                else if (Path.GetExtension(fstring).Equals(".hpp") || Path.GetExtension(fstring).Equals(".h") || Path.GetExtension(fstring).Equals(".ino"))
                {
                    if(Path.GetFileName(fstring).Equals("Application_Solution.h"))
                    {
                        // copy and modify #include line within
                        string ftext = File.ReadAllText(fstring);

                        Regex rx = new Regex(@"#define ccGripper_BuildNumber [0-9]+ ");
                        MatchCollection matches = rx.Matches(ftext);
                        
                        if(matches.Count > 0)
                        {
                            string[] toks = matches[0].Value.Split(' ');
                            if(toks.Length > 2)
                            {
                                uint buildNum;
                                if (UInt32.TryParse(toks[2], out buildNum))
                                {
                                    ftext = rx.Replace(ftext, toks[0] + " " + toks[1] + " " + (buildNum + 1) + " ");
                                    File.WriteAllText(fstring, ftext);
                                }
                            }
                        }
                        
                        
                    }
                    
                    File.Copy(fstring, filestring, true);

                }
            }
        }
    }
}
