
#addin nuget:?package=Cake.Git&version=1.0.1
#addin nuget:?package=Cake.FileHelpers&version=4.0.1

using Cake.Core.IO;
using Cake.Common.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Net;
using Cake.Common.Tools.WiX;

var target = Argument("target", "Default");
var configuration = Argument("configuration","Release");
var currentDirectory=MakeAbsolute(Directory("./"));
var serviceDirectory=MakeAbsolute(Directory("../"));
var currentDirectoryInfo=new DirectoryInfo(currentDirectory.FullPath);
var studio_version=Argument<string>("studio_version","");
var nugetserverurl = Argument<string>("nugetserverurl","");
var assemblyFolderPath=MakeAbsolute(Directory("../../designer-repos/assembly/")).FullPath;
var assemblyfileversion=Argument<string>("assemblyfileversion","");
var outputpath=Argument<string>("outputpath",assemblyFolderPath);
var referencepath=Argument<string>("referencepath",assemblyFolderPath);
var productname=Argument<string>("build_type","");
var optionalbranch=Argument<string>("optionalbranch","");
var build_mode=Argument<string>("build_mode","");
var BuildUrl=Argument<string>("buildurl","");
var checkOutPath = Argument<string>("checkoutpath", MakeAbsolute(Directory("../../designer-repos")).FullPath);
string build_type=productname.Replace(" ","");
// Git lab username and pasword
var UserName = Argument<string>("GitUserName","");
var Password = Argument<string>("GitPassword","");
var branchName = Argument<string>("branchname","");
var site = Argument<string>("Site","");
var domain = Argument<string>("domain","");
var isParallel = Argument<bool>("isParallel",true);
var isCdn=Argument<bool>("iscdn",false);
var DefineConstants = Argument<string>("DefineConstants","");
referencepath = assemblyFolderPath;
Information("Service Path = {0}",serviceDirectory);
public class RepositoryDetails
{
	public string RepoUrl { get; set; }
	public string BranchName { get; set; }
	public string FolderName { get; set; }
	public bool IgnoreBuild { get; set;}
}
public class RepositoryGroup
{
	public int GroupId {get; set;}
	public bool IsDependent {get; set;}
	public int DependentGroup {get; set;}
	public List<RepositoryDetails> RepositoryList {get; set;}
}

public enum ActionMode{
	CheckOut = 0,
	CreateTag = 1,
	CreateBranch = 2,
	PushToTargetBranch= 3
}
public class GitAction{
	public GitAction(){
		SourceRepositories=new List<RepositoryDetails>();
		SourceGroupRepositories = new List<RepositoryGroup>();
	}
	public ActionMode Mode { get; set; }
	public string TargetBranch { get; set; }
	public string TagName { get; set; }
	public List<RepositoryDetails> SourceRepositories {get; set;}
	public List<RepositoryGroup> SourceGroupRepositories {get; set;}
}
public GitAction action=new GitAction();
public Dictionary<int, bool> dic = new Dictionary<int, bool>();
//public bool passBuild = true;

Task("Initialize")
.Does(()=>{
 	XmlDocument doc = new XmlDocument();
	Information(currentDirectory);	
	doc.Load(currentDirectory + "/RepositoryDetails.xml");
	XDocument xdoc = XDocument.Load(currentDirectory + "/RepositoryDetails.xml");
	Information("document loaded");
  // XmlNodeList element = doc.GetElementsByTagName("GitAction");
  // action.Mode=Enum.Parse(element.Attributes["mode"].Value,typeof(ActionMode),true);
  // action.TargetBranch=element.Attributes["targetBranch"].Value;
	// action.TagName=element.Attributes["tagName"].Value;
	
	//GetRepositories(doc);
	NewGetRepositories(xdoc);
	});

	Task("Clean")
	.Does(()=>{
    if(DirectoryExists(assemblyFolderPath)){
			CleanDirectories(assemblyFolderPath);
		}
		var cdnFolder=MakeAbsolute(Directory(assemblyFolderPath+"/../cdn/")).FullPath;
		Information(cdnFolder);
		if(DirectoryExists(cdnFolder)){
			CleanDirectories(cdnFolder);
		}
		if(DirectoryExists(currentDirectory+"/Cake")){
			CleanDirectories(currentDirectory+"/Cake");
		}
	});
Task("Git-Clone-Checkout")
.IsDependentOn("Initialize")
.Does(() =>
{
	Information("Parallel Build mode: " + isParallel);
	if(isParallel)
	{
		Parallel.ForEach(action.SourceRepositories, (repoDetails) => {
			DownloadRepositories(repoDetails);
			
		});
	}
	else
	{
		foreach(var repoDetails in action.SourceRepositories) {
			DownloadRepositories(repoDetails);
			GitCleanProductTeamSource(checkOutPath +"/" + repoDetails.FolderName,string.IsNullOrEmpty(branchName)? repoDetails.BranchName:branchName);
		}
		
	}
});

public void DownloadRepositories(RepositoryDetails repoDetails)
{
	if(DirectoryExists(checkOutPath + "/" + repoDetails.FolderName))
	{		
		var exitCode = 0;
		
		Information("Repository Name " + repoDetails.FolderName);
		
		System.IO.Directory.SetCurrentDirectory(checkOutPath+ "/" + repoDetails.FolderName);
		
		Information("Clean repository " + repoDetails.FolderName);
		
		exitCode = StartProcess(@"git",new ProcessSettings{ Arguments = "clean -dxf"});	
		
		exitCode = StartProcess(@"git",new ProcessSettings{ Arguments = "fetch --prune"});	
		
		Information("Resetting repository " + repoDetails.FolderName);
		
		exitCode = StartProcess(@"git",new ProcessSettings{ Arguments = "reset --hard"});	

		if(exitCode == 0)
		{
			Information("Pull...");
			
			exitCode = StartProcess(@"git",new ProcessSettings{ Arguments = "pull"});
			
			//git-gc - Cleanup unnecessary files and optimize the local repository
			
			if(exitCode != 0){
				StartProcess(@"git",new ProcessSettings{ Arguments = "gc"});
			}
			
			if(exitCode == 0)
			{
				
				Information("Checkout " + (string.IsNullOrEmpty(branchName)? repoDetails.BranchName:branchName));	
		
				exitCode = StartProcess(@"git",new ProcessSettings{ Arguments = "checkout " + (string.IsNullOrEmpty(branchName)? repoDetails.BranchName:branchName)});
			
				if(exitCode == 0)
				{
					
						Information("Pull...");
					
						exitCode = StartProcess(@"git",new ProcessSettings{ Arguments = "pull"});
						
						if(exitCode !=0) {
							StartProcess(@"git",new ProcessSettings{ Arguments = "gc"});	
							exitCode = StartProcess(@"git",new ProcessSettings{ Arguments = "pull"});
						}
										
				}	
			}
		}	
		
		// Cake Build - Git Pull API implementation
		/*
		if(!DirectoryExists(currentDirectory + "/GitLogs"))
		{
			EnsureDirectoryExists(currentDirectory + "/GitLogs");
		}
		
		Information("Repository Name " + repoDetails.FolderName);
		var repoPath = checkOutPath + "/" + repoDetails.FolderName;
		Information("Clean repository "+ repoDetails.FolderName);
		GitClean(repoPath);
		Information("Resetting repository " + repoDetails.FolderName);
		GitReset(repoPath, GitResetMode.Hard);
		
		var beforePullCommit = GitLogTip(repoPath);

		var pullResult = GitPull(repoPath, 
				"BuildAutomation",
				"buildautomation@syncfusion.com", 
				UserName, 
				Password, 
				"origin");

		if (pullResult.Status==GitMergeStatus.UpToDate)
		{
			var afterPullCommit = GitLogTip(repoPath);

			var diff = GitDiff(repoPath, beforePullCommit.Sha, afterPullCommit.Sha);

			
			foreach(var file in diff)
			{
				System.IO.File.AppendAllText(currentDirectory + "/GitLogs/" + repoDetails.FolderName + ".txt", file.ToString()+ Environment.NewLine);
			}
		}
		
		Information(repoDetails.FolderName + " is " + pullResult.Status); 
		*/
		
	}
	else {
		EnsureDirectoryExists(checkOutPath +"/" + repoDetails.FolderName);
		
		Information("cloning repository " + repoDetails.FolderName);
		
		GitClone(repoDetails.RepoUrl.Trim() + ".git", 
			checkOutPath + "/" + repoDetails.FolderName, 
			UserName, 
			Password, 
			new GitCloneSettings { BranchName = string.IsNullOrEmpty(branchName)? repoDetails.BranchName:branchName });
	}
}

public void GitCleanProductTeamSource(string dir, string branchname)
{
  try
  {
      IEnumerable < string > redirectedOutput;
      System.IO.Directory.SetCurrentDirectory(dir);
      string FolderName =  System.IO.Path.GetFileName(dir);
      StartProcess("git", new ProcessSettings{
        Arguments = "rev-parse --abbrev-ref HEAD",
        RedirectStandardOutput = true
      },
        out redirectedOutput
      );
      var clonedbranch = string.Join("", redirectedOutput.ToArray());
      if (clonedbranch.ToString().Contains(branchname))
      {
        var exitCode = StartProcess(@"git", new ProcessSettings{ Arguments = "rev-parse --is-inside-work-tree" });
        if (exitCode == 0)
        {
          StartProcess(@"C:\Program Files\Git\usr\bin\rm.exe", new ProcessSettings{ Arguments = "-rf node_modules" });//remove node_modules
          exitCode = StartProcess(@"git", new ProcessSettings{ Arguments = "clean -xdf" });
          if (exitCode == 0)
          {
            exitCode = StartProcess(@"git", new ProcessSettings{ Arguments = "checkout ." });
          }
          if (exitCode == 0)
          {
            StartProcess("git", new ProcessSettings{
              Arguments = "pull --stat",
              RedirectStandardOutput = true
            },
              out redirectedOutput
            );
            // write the pull log into text file
            System.IO.File.WriteAllLines(currentDirectory + "GitLogs/" + FolderName + ".txt",
              redirectedOutput, Encoding.UTF8
            );

 

          }
        }
      }
    
  }
  catch
  { }
}

public void GetRepositories(XmlDocument doc)
{
	
	XmlNodeList elemList = doc.GetElementsByTagName("Repository");
	var filteredList = elemList.Cast<XmlNode>().ToList();	
    for (int i = 0; i < filteredList.Count; i++)
    {
			if(isCdn){
        if( filteredList[i].Attributes["cdn"] !=null && bool.Parse(filteredList[i].Attributes["cdn"].Value.ToString())){
						 	action.SourceRepositories.Add(new RepositoryDetails(){
							BranchName = filteredList[i].Attributes["branchName"].Value,
							RepoUrl = filteredList[i].Attributes["repoUrl"].Value,
							FolderName = filteredList[i].Attributes["folderName"].Value,
							IgnoreBuild = filteredList[i].Attributes["ignoreBuild"] !=null? bool.Parse(filteredList[i].Attributes["ignoreBuild"].Value.ToString()) : false
					});
				}
			} else {
				action.SourceRepositories.Add(new RepositoryDetails(){
					BranchName = filteredList[i].Attributes["branchName"].Value,
					RepoUrl = filteredList[i].Attributes["repoUrl"].Value,
					FolderName = filteredList[i].Attributes["folderName"].Value,
					IgnoreBuild = filteredList[i].Attributes["ignoreBuild"] !=null? bool.Parse(filteredList[i].Attributes["ignoreBuild"].Value.ToString()) : false
				});
			}
		}  
}

public void NewGetRepositories(XDocument doc)
{
	var repoGroups = from repoGroup in doc.Descendants("SourceRepositories").Descendants("RepositoryGroup")
                       select new
                       {
                           GroupId = int.Parse(repoGroup.Attribute("groupId").Value.ToString()),
						   IsDependent = bool.Parse(repoGroup.Attribute("isDependent").Value.ToString()),
						   DependentGroup = int.Parse(repoGroup.Attribute("dependentGroupId").Value.ToString()),
                           RepositoryGroup = repoGroup.Descendants("Repository")
                       };
	
	//Add non dependent repository id into dictionary with true value
	dic.Add(0,true);
	foreach(var repoList in repoGroups)
	{
		
		var groupId = repoList.GroupId;
		var isDependent = repoList.IsDependent;
		var dependentGroup = repoList.DependentGroup;
		dic.Add(groupId,false);
	
		GitAction act = new GitAction();
		foreach(var repos in repoList.RepositoryGroup){
			
			RepositoryDetails repo = new RepositoryDetails();
			if(isCdn){
				if( repos.Attribute("cdn") !=null && bool.Parse(repos.Attribute("cdn").Value.ToString())){
					repo = new RepositoryDetails(){
					BranchName = repos.Attribute("branchName").Value,
					RepoUrl = repos.Attribute("repoUrl").Value,
					FolderName = repos.Attribute("folderName").Value,
					IgnoreBuild = repos.Attribute("ignoreBuild") !=null? bool.Parse(repos.Attribute("ignoreBuild").Value.ToString()) : false
					};
				}
			} else {
				repo = new RepositoryDetails(){
					BranchName = repos.Attribute("branchName").Value,
					RepoUrl = repos.Attribute("repoUrl").Value,
					FolderName = repos.Attribute("folderName").Value,
					IgnoreBuild = repos.Attribute("ignoreBuild") !=null? bool.Parse(repos.Attribute("ignoreBuild").Value.ToString()) : false
				};
			}
			act.SourceRepositories.Add(repo);
			action.SourceRepositories.Add(repo);
		}
		
		action.SourceGroupRepositories.Add(new RepositoryGroup(){
			GroupId = groupId,
			IsDependent = isDependent,
			DependentGroup = dependentGroup,
			RepositoryList = act.SourceRepositories
		});
		
		
	}
}
public void CreateTags() {
	if(string.IsNullOrEmpty(action.TagName))
	foreach(var repoDetails in action.SourceRepositories){
		Information("Repository Name " + repoDetails.FolderName);
		if(DirectoryExists("./"+repoDetails.FolderName)){
		  var exitCode = 0;
			System.IO.Directory.SetCurrentDirectory("./" + repoDetails.FolderName);
		  exitCode = StartProcess(@"git",new ProcessSettings{ Arguments = "tag"});
			if(exitCode==0) {
			 exitCode = StartProcess(@"git",new ProcessSettings{ Arguments = "tag -a tags/"+action.TagName +" -m" + repoDetails.BranchName});
			 if(exitCode !=0){
				//  throw new Exception("Tag Craetion failed for the repo "+ repoDetails.RepoUrl);
			 }
			}
		}
		// System.IO.Directory.SetCurrentDirectory(currentDirectory);
	}
}
private void RemoveSyncfusionPackages(RepositoryDetails repository,string rootFolder)
{
	List<string> sycfusionexcludedpackages = new List<string>();
            string[] boldBi = {
        "Compression.Base",
		"Server.Base.Encryption",
        "Cloud.Base.Encryption",
        "PhantomJS",
        "Dashboard.XlsIO.Base",
        "Server.Base.Logger",
        "Dashboard.Xml.Base",
        "JsonShapes",
        "ExportWrapper",
        "KBCsv",
        "FxCop",
        "StyleCop",
        "Interope.Excel",
        "CefSharp",
        "Dashboard.Signalr.Core",
        "Dashboard.Web.MongoDB.Driver",
        "KeyPackage",
        "Vertica",
        "Dashboard.ThriftHive.Base",
        "Dashboard.Thrift",
		"Thrift",
		"ThriftHive.Base"
        };
            string[] onPremiseBoldBi = {
        "Compression.Base",
		"Server.Base.Encryption",
		"Server.Base.logger",
        "Cloud.Base.Encryption",
		"XlsIO.Net.Core",
        "Dashboard.XlsIO.Base",
        "Dashboard.Xml.Base",
        "JsonShapes",
        "ExportWrapper",
        "KBCsv",
        "FxCop",
        "StyleCop",
        "Interope.Excel",
        "CefSharp",
        "Dashboard.Signalr.Core",
        "Dashboard.Web.MongoDB.Driver",
        "KeyPackage",
        "Vertica",
        "Dashboard.ThriftHive.Base",
        "Dashboard.Thrift",
		"Thrift",
		"ThriftHive.Base"
        };
       sycfusionexcludedpackages.AddRange((configuration != "onpremiseboldbi") ? boldBi : onPremiseBoldBi);
	var configfiles = GetFiles(rootFolder+ (string.IsNullOrEmpty(repository.FolderName)?"":( "/" + repository.FolderName))+ "/**/*.csproj");
	foreach(var configpath in configfiles)
	{
		Information("config file path :"+configpath);
		string tempFile = System.IO.Path.GetTempFileName();
		using (var sr = new System.IO.StreamReader(configpath.ToString()))
		using (var sw = new System.IO.StreamWriter(tempFile))
		{
			string line;
			while ((line = sr.ReadLine()) != null)
			{
				bool excludevalue = false;
				foreach(string nugetname in sycfusionexcludedpackages)
				{
					if (!line.ToLower().Contains("packagereference") || !line.ToLower().Contains("syncfusion") || line.Contains(nugetname))
					{
						excludevalue = true;
						break;
					}
				}
				if (excludevalue)
				{
					sw.WriteLine(line);
				}
			}
		}
		System.IO.File.Delete(configpath.ToString());
		System.IO.File.Move(tempFile, configpath.ToString());
	}
}
private int BuildSource(RepositoryDetails repo) {
try{
//var cakeExe = @"D:\Tst\GitCheckout\cake-develop\src\Cake\bin\Debug\net461\Cake.exe";
var cakeExe =  currentDirectory + "/Cake/Cake.exe";
Information("Cake EXE path:"+cakeExe);
  var cakeFilePath= checkOutPath+ "/" + repo.FolderName + "/build/build.cake";
Information("Build Cake file path:"+cakeFilePath);
   var errorCode= StartProcess(cakeExe.ToString(),
	 new ProcessSettings {
		  Arguments = cakeFilePath +
			 " -target=build -configuration=" + configuration +
			 " -referencepath=" + referencepath + 
			 " -outputpath=" + outputpath +
			 " -studio_version="+ studio_version +
			 " -site=" +  site +
			 " -domain="+ domain+
			 " -assemblyfileversion="+ assemblyfileversion +
			 " -nugetserverurl="+nugetserverurl +
			 " -DefineConstants=" + DefineConstants
				}); 

	if(errorCode!=0){
		Error("Build Failed for repo" + repo.FolderName);
	}
	return errorCode;
	} 
	catch(Exception ex){
		Error(ex.ToString());
	}
	return 1;
} 

private void UpdateCurrentRepoPackageConfig(){
	Information("Current Path = {0}",serviceDirectory);
    var currentRepo=new RepositoryDetails{
       FolderName=null
	};
	RemoveSyncfusionPackages(currentRepo,serviceDirectory.ToString());
}

private void CopyErrorLogFiles(RepositoryDetails repoDetails){
	try{
		Information("Copy Build Error Log Report to Service Cireports/errorlog Location");
		Information("Repository Error Log Path: " + repoDetails.FolderName + "/cireport/errorlogs/");
		var errorLogPath = currentDirectory + "/../cireports/errorlogs";
		if(!DirectoryExists(errorLogPath))
		{
			CreateDirectory(errorLogPath);
		}
		
		var readErrorFile=GetFiles(checkOutPath +"/" + repoDetails.FolderName + "/cireports/errorlogs"+"/**/*.txt");
		CopyFiles(readErrorFile, errorLogPath);
		
	}
	catch(Exception ex)
	{
		throw new Exception("Exception in Coping error logs files to service Cireport location: ", ex);
	}
}

private async void WaitingToProceed()
{
	await System.Threading.Tasks.Task.Delay(10000);
}

Task("LoadlongPath")
.Does(() =>
{

	System.IO.Directory.SetCurrentDirectory(MakeAbsolute(Directory("./")) + "/");

	var fileSystemType = Context.FileSystem.GetType();

	Information(fileSystemType.ToString());

	if (fileSystemType.ToString()=="Cake.LongPath.Module.LongPathFileSystem")
	{
		Information("Successfully loaded {0}", fileSystemType.Assembly.Location);
	}
	else
	{
		Error("Failed to load Cake.LongPath.Module");
	}
});
Task("Download-Nugetexe")
  .Does(() =>
{
 
    DownloadFile("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe", "./tools/nuget.exe");
     
});

Task("Download-Cake")
.IsDependentOn("Download-Nugetexe")
.Does(()=>{
   StartProcess("./tools/nuget.exe",new ProcessSettings{ Arguments = "install Cake -Version 0.35.0  -ExcludeVersion -OutputDirectory " +currentDirectory });
});
Task("Compile")
.IsDependentOn("Clean")
.IsDependentOn("Initialize")
.IsDependentOn("Download-Cake")
.IsDependentOn("Git-Clone-Checkout")
.Does(()=>{
	try{
		Parallel.ForEach(action.SourceRepositories,(repoDetails)=>
		{	
			Information("Update package config for Repository: " + repoDetails.FolderName);
			if(!string.IsNullOrEmpty(referencepath)){
				RemoveSyncfusionPackages(repoDetails,checkOutPath);

			}
		});
		UpdateCurrentRepoPackageConfig();
		if(!isParallel)
		{
			foreach(var repoDetails in action.SourceRepositories)
			{
				Information("Compiling Repository: " + repoDetails.FolderName);
				if(!repoDetails.IgnoreBuild){
				var errorCode = BuildSource(repoDetails);
				CopyErrorLogFiles(repoDetails);	
				if(errorCode!=0) {
					break;
				}
				}
			}
		}
		else
		{
			var passBuild = true;
			Parallel.ForEach(action.SourceGroupRepositories, repoGroup =>
			{
				
				while(passBuild == true && !dic[repoGroup.DependentGroup])
				{
					WaitingToProceed();
				}
				
				if(passBuild){
					foreach(var repoDetails in repoGroup.RepositoryList)
					{
						Information("Compiling Repository : " + repoDetails.FolderName);
							
							if(!repoDetails.IgnoreBuild){
								Information("Compiling Repository: " + repoDetails.FolderName);
								var errorCode = BuildSource(repoDetails);
								CopyErrorLogFiles(repoDetails);
								
								if(errorCode!=0) {
									passBuild = false;
									throw new OperationCanceledException();
									//break;
								}
							}
						
					}
					if(passBuild){
						dic[repoGroup.GroupId] = true;
						Information("Dictionary of Group " + repoGroup.GroupId + " is value changed to " +dic[repoGroup.GroupId]);
					}
				}
			});
		}
		
	}
	catch(Exception ex)
	{
		throw new Exception("Please fix compile task failures: ", ex);
	}
});

Task("Default")	
.IsDependentOn("Compile");

RunTarget(target);