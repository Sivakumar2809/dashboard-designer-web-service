#addin nuget:?package=Cake.FileHelpers&version=4.0.1

using System.Text.RegularExpressions;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////
var target = Argument("target", "Default");
var cakeExe = MakeAbsolute(Directory("./tools/Cake/Cake.exe"));
var configuration = Argument("configuration", "Release");
var cireports = Argument("cireports","../cireports");
var nugetserverurl = Argument<string>("nugetserverurl","");
var nugetapikey = Argument<string>("nugetapikey","");
var assemblyfileversion=Argument<string>("assemblyfileversion","");
var STUDIO_VERSION=Argument<string>("studio_version","4.0.0.0");
var studio_version = Argument("studio_version", STUDIO_VERSION).Split('.'); 
var assemblyFolderPath=MakeAbsolute(Directory("../../designer-repos/assembly/")).FullPath;
var referencepath = Argument<string>("referencepath", "");
var outputpath = Argument<string>("outputpath", "");
var PreReleaseNumber = Argument("PreReleaseNumber", "1");
var publishProfilefilepath = Argument("publishProfilefilepath", "../src/Properties/PublishProfiles/local-folder.pubxml");
var password = Argument("password", "");
var SkipExtraFilesOnServer= Argument("SkipExtraFilesOnServer","true");
var site=Argument("Site","");
var domain=Argument("domain","");
// Git lab username and pasword
var GitUserName = Argument<string>("GitUserName","");
var GitPassword = Argument<string>("GitPassword","");
var DefineConstants = Argument<string>("DefineConstants","");
DefineConstants = configuration.ToLower().Contains("docker") ? "onpremiseboldbi;docker" : DefineConstants;
configuration = configuration.ToLower().Contains("docker") ? "onpremiseboldbi" : configuration;
referencepath =(configuration == "onpremiseboldbi" && referencepath == "") ? assemblyFolderPath + "/" : (referencepath != null && referencepath != "") ? referencepath : "";
////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////
var projectName = "Syncfusion.Dashboard.Designer.Web.Service";
var currentDirectory=MakeAbsolute(Directory("../"));
var currentDirectoryInfo=new DirectoryInfo(currentDirectory.FullPath);
var fxCopViolationCount=0;
var styleCopViolationCount=0;
string Copyrights="[assembly: AssemblyCopyright(\"Copyright (c) 2001-"+DateTime.Now.Year+" Syncfusion. Inc,\")]";
var fxcopFolder = cireports+"/fxcopviolation/";
var stylecopFolder = cireports+"/stylecopviolation/";
var errorlogFolder = cireports + "/errorlogs/";
var waringsFolder = cireports + "/warnings/";
var xunitFolder = cireports+"/xunit";
var codecoverageFolder = cireports+"/codecoverage";
string platform="SECURITY";
var nugetPackageFolder="";
FilePath sourceFile {get; set;}

Information("NexusServer URL is {0}",nugetserverurl);
Information("Site Parameter is {0}",site);
Information("Domain is {0}",domain);
Information("publish profile is {0}",publishProfilefilepath);

//////////////////////////////////////////////////////////////////////
// Regex
//////////////////////////////////////////////////////////////////////

var fxCopRegex = "warning CA";
var styleCopRegex = "warning SA";
var styleCopAnalyzersRegex = "warning SX";
var xUnitRegex = "warning xUnit";
var apiAnalyzerRegex = "warning API";
var asyncAnalyzerRegex = "warning AsyncFixer";
var cSharpAnalyzerRegex = "warning RS";
var mvcAnalyzerRegex = "warning MVC";
var entityFrameworkRegex = "warning EF";
var rosylnatorAnalyzerRegex = "warning RCS";
var nugetRegex = "warning NU";

var nugetSources="";
List<string> nugetSource = new List<string>();
if(site != ""&& nugetserverurl == "")
{
    if(site == "staging")
    {
        nugetserverurl="http://nexus.syncfusion.com/repository/boldbi-release/,https://api.nuget.org/v3/index.json";
    }
	else
	{
		nugetserverurl="http://nexus.syncfusion.com/repository/boldbi-development/,https://api.nuget.org/v3/index.json";
	}
   
}else if(configuration == "onpremiseboldbi" && nugetserverurl !=""){
nugetserverurl = nugetserverurl + ","+ "https://api.nuget.org/v3/index.json";
Information("nugetserverurl is {0}", nugetserverurl);
}
var buildCompileNugetServerUrl = nugetserverurl;
var nugetserverurls = nugetserverurl.Split(',');	
foreach(var nugeturl in nugetserverurls)
{
    Information(nugeturl);
    nugetSource.Add(nugeturl);
    nugetSources=nugetSources+nugeturl+';';
}
nugetserverurl =nugetserverurl.Split(',')[0];

Information("current Directory is {0}",currentDirectory);
Information("NexusServer URL is {0}",nugetserverurl);
Information("STUDIO_VERSION is {0} ",STUDIO_VERSION);
Information("studio_version is {0}",studio_version);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{

    var binDirectories=currentDirectoryInfo.GetDirectories("bin",SearchOption.AllDirectories);
    var objDirectories=currentDirectoryInfo.GetDirectories("obj",SearchOption.AllDirectories);
    
    foreach(var directory in binDirectories){
        CleanDirectories(directory.FullName);
    }
    
    foreach(var directory in objDirectories){
        CleanDirectories(directory.FullName);
    }
	CleanDirectories(currentDirectory+"/src/Plugins");
	CleanDirectories(currentDirectory+"/src/scripts");
	CleanDirectories(currentDirectory+"/src/themes");
	CleanDirectories(currentDirectory+"/src/fonts");
	CleanDirectories(currentDirectory +"/packages");
    DeleteFiles(GetFiles(currentDirectory+"/**/*FxCopAnalysis.xml"));
    DeleteFiles(GetFiles(currentDirectory+"/**/*StyleCopAnalysis.xml"));
});

Task("CopyrightandVersion")
  .IsDependentOn("Clean")
  .Does(() =>
{
  var assemblyfiles = GetFiles("../**/*AssemblyInfo.cs");
  foreach(var assemblyfile in assemblyfiles)
  {
  ReplaceRegexInFiles(assemblyfile.ToString(),@"[\d]{1,2}\.[\d]{1}\.[\d]{1}\.[\d]{1,4}",STUDIO_VERSION);
  ReplaceRegexInFiles(assemblyfile.ToString(),@"\[assembly:\s*AssemblyCopyright\s*.*?\]",Copyrights);
  ReplaceRegexInFiles(assemblyfile.ToString(),@"AssemblyCompany\s*.*","AssemblyCompany(\"Syncfusion, Inc.\")]");
  }
});


Task("DeleteLogFile")
	.Does(()=>{
		
    if(FileExists("../cireports/errorlogs/"+ projectName +".txt"))
      DeleteFile("../cireports/errorlogs/"+ projectName +".txt");
    if(FileExists("../cireports/warnings/"+ projectName +".txt") && referencepath == "")
      DeleteFile("../cireports/warnings/"+ projectName +".txt");
	  
	var getErrorLogFiles = GetFiles("../cireports/errorlogs/*.txt");
	
	foreach(var file in getErrorLogFiles)
	{
		Information("Delete Compilation Error Logs file: {0}", file);
		DeleteFile(file);
	}
});


Task("Restore-NuGet-Packages")
    .Does(() =>
{    
    var slnFiles = GetFiles("../*.sln");
    foreach(var slnFile in slnFiles){
        Information("slnFile {0}", slnFile);
       DotNetCoreRestore(slnFile.ToString(),new DotNetCoreRestoreSettings {Sources =nugetSource, EnvironmentVariables = new Dictionary<string, string>{
        { "Configuration", configuration.ToString() }
    }});
    }
});
Task("Download-Nugetexe")
  .WithCriteria( !FileExists("./tools/nuget.exe"))
  .ContinueOnError()
  .Does(() =>
{
 
     DownloadFile("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe", "./tools/nuget.exe");
     
});
Task("Create-NugetPackage-Directory")
.Does(() =>{
            var nugetContent = System.Xml.Linq.XDocument.Parse(System.IO.File.ReadAllText(nugetPackageFolder+"/NuGet.Config",System.Text.Encoding.UTF8)); //Read the text of config file
            var nugetConfigElement = (from elements in nugetContent.Descendants("config") select elements).ToList();//get the config element value
            if (nugetConfigElement.Count==0)//check the config element had value or not
            {
				EnsureDirectoryExists(sourceFile.GetDirectory().ToString()+"/packages");
            }
            else
            {
				EnsureDirectoryExists(nugetPackageFolder+"/"+nugetConfigElement[0].ToString().Split('"')[3]);
            }
});

Task("Update-Nuget-Packages")
.Does(() =>{
              
		if(FileExists(currentDirectory.ToString()+"/NuGet.Config"))
        {
            nugetPackageFolder=currentDirectory.ToString();
            RunTarget("Create-NugetPackage-Directory");
		} 
                                           
        var slnFiles = GetFiles("../*.sln");

        //update the nuget packages
        foreach(var slnFile in slnFiles)
        {
		
            sourceFile=File(slnFile.ToString());
			if (FileExists(sourceFile.GetDirectory().ToString()+"/.nuget/NuGet.Config"))
            {
				nugetPackageFolder=sourceFile.GetDirectory().ToString()+"/.nuget/";
                RunTarget("Create-NugetPackage-Directory");
				StartProcess("./tools/nuget.exe", new ProcessSettings
                { Arguments ="update "+ slnFile + " -ConfigFile " +nugetPackageFolder+"/NuGet.Config -Source "+nugetSources});
            }
            else if(FileExists(currentDirectory.ToString()+"/NuGet.Config"))
            {
				StartProcess("./tools/nuget.exe", new ProcessSettings
                 { Arguments ="update "+ slnFile + " -ConfigFile " +nugetPackageFolder+"/NuGet.Config -Source "+nugetSources});
            }
            
			//if nuget.config does not exist                 
            else
            {	EnsureDirectoryExists(sourceFile.GetDirectory().ToString()+"/packages");
                NuGetUpdate(slnFile,new NuGetUpdateSettings {Source = nugetSource});
            }
        }
});

Task("Clear-NuGet-Cache")
    .IsDependentOn("Download-Nugetexe")
    .Does(() =>
{   Information(currentDirectory.ToString() + "/packages");
    if(DirectoryExists(currentDirectory.ToString() + "/packages"))
    {
        DeleteDirectory(currentDirectory.ToString() + "/packages", true);    
    }
    
    //StartProcess("./tools/nuget.exe",new ProcessSettings{Arguments ="locals -clear all"});
    //Information("Local nuget caches cleared");
});

Task("build")
	.IsDependentOn("Download-Nugetexe")
	.IsDependentOn("CopyrightandVersion")
	//.IsDependentOn("Update-NuGet-Packages")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
	//RunTarget("DeleteLogFile");
	var flag = false;
	if(target =="Publish")
		flag = true;
	if(target =="onpremiseboldbi")
		flag = true;
    if(domain=="boldbi"){
        System.IO.File.Copy(currentDirectory+@"\src\favicons\boldbi.ico",currentDirectory+@"\src\favicon.ico",true);
    } else{
        System.IO.File.Copy(currentDirectory+@"\src\favicons\syncfusion.ico",currentDirectory+@"\src\syncfusion.ico",true);
    }
	if(!flag)
	{
	  EnsureDirectoryExists("../cireports/errorlogs");
      EnsureDirectoryExists("../cireports/warnings"); 
      MSBuild(currentDirectory + @"\"+projectName+".sln",  new MSBuildSettings()
		.AddFileLogger(new MSBuildFileLogger { LogFile="../cireports/warnings/"+projectName+".txt",MSBuildFileLoggerOutput=MSBuildFileLoggerOutput.WarningsOnly})
		.AddFileLogger(new MSBuildFileLogger { LogFile="../cireports/errorlogs/"+projectName+".txt",MSBuildFileLoggerOutput=MSBuildFileLoggerOutput.ErrorsOnly})
		.WithProperty("ReferencePath",referencepath)
		.WithProperty("OutDir",outputpath)
		.WithProperty("NexusServerUrl",nugetserverurl)
		.WithProperty("CompilationFrom","cake")
        .WithProperty("PublishDomain",domain)
		.SetConfiguration(configuration)
        .SetVerbosity(Verbosity.Minimal));
		//RunTarget("DeleteLogFile");
		
		var logFilename = projectName + ".txt";
        if(FileExists(errorlogFolder + logFilename))
        {
            if (FileSize(errorlogFolder + logFilename) == 0 )
                DeleteFile(errorlogFolder + logFilename);
        }
		}
        var errorCode=0;
        if(string.IsNullOrEmpty(referencepath)){
		 errorCode = StartProcess(currentDirectory+@"\src\copyfiles.bat");
        } else{
          errorCode = StartProcess(currentDirectory+@"\src\movedependentscripts.bat");
        }

        if(errorCode != 0 ){
            throw new Exception("Moving Script files failed...");
        }
}).OnError(exception =>
		{ 
			throw new Exception("Dashboard designer service build failed");
		});

Task("GetFxCopReports")
.Does(()=>
{
	try
	{
		var logFilename = projectName + ".txt";
		if (DirectoryExists(fxcopFolder))
		{
		 DeleteDirectory(fxcopFolder, recursive:true);
		}
		
		var fxCopAnalysisFiles=FileReadText(waringsFolder + logFilename);
		
		fxCopViolationCount = Regex.Matches(fxCopAnalysisFiles, fxCopRegex).Count;
		fxCopViolationCount += Regex.Matches(fxCopAnalysisFiles, apiAnalyzerRegex).Count;
		fxCopViolationCount += Regex.Matches(fxCopAnalysisFiles, asyncAnalyzerRegex).Count;
		fxCopViolationCount += Regex.Matches(fxCopAnalysisFiles, cSharpAnalyzerRegex).Count;
		fxCopViolationCount += Regex.Matches(fxCopAnalysisFiles, mvcAnalyzerRegex).Count;
		fxCopViolationCount += Regex.Matches(fxCopAnalysisFiles, entityFrameworkRegex).Count; 
		fxCopViolationCount += Regex.Matches(fxCopAnalysisFiles, rosylnatorAnalyzerRegex).Count; 
		
		fxCopViolationCount = 0;
		if(fxCopViolationCount != 0)
		{        
		   Information("There are {0} FXCop violations found", fxCopViolationCount);
		}

		if (!DirectoryExists(cireports))
		{
			CreateDirectory(cireports);
		}
			
		if(!DirectoryExists(fxcopFolder))
		{
			CreateDirectory(fxcopFolder);
		}
		
		FileWriteText(fxcopFolder + "FXCopViolations.txt", "FXCop Error(s) : " + fxCopViolationCount);
	}
	catch(Exception ex) {        
		throw new Exception(String.Format("Please fix Get Fx Cop Reports failures "+ ex));  
	}
		
});



Task("GetStyleCopReports")
 .Does(()=>
 {
    try
	{
		var logFilename = projectName + ".txt";
		if (DirectoryExists(stylecopFolder))
		{
		 DeleteDirectory(stylecopFolder, recursive:true);
		}
		var styleCopWarning = FileReadText(waringsFolder + logFilename);
		styleCopViolationCount += Regex.Matches(styleCopWarning, styleCopRegex).Count;
		styleCopViolationCount += Regex.Matches(styleCopWarning, styleCopAnalyzersRegex).Count;

		styleCopViolationCount=0;
		if(styleCopViolationCount != 0)
		{        
		   Information("There are {0} StyleCop violations found", styleCopViolationCount);
		}
		
		if(!DirectoryExists(cireports))
		{
			CreateDirectory(cireports);
		}

		if(!DirectoryExists(stylecopFolder))
		{
			CreateDirectory(stylecopFolder);
		}
		
		FileWriteText(stylecopFolder + "StyleCopViolations.txt", "Style Cop Error(s) : " + styleCopViolationCount);
	}
	catch(Exception ex) {        
		throw new Exception(String.Format("Please fix Get Style Cop Reports failures " + ex));  
	}

 });
 
 Task("codeviolation")
	.IsDependentOn("GetFxCopReports")
	.IsDependentOn("GetStyleCopReports")
	.Does(()=>{
		Information("Code violation");
		Information("StyleCop violations = {0}",styleCopViolationCount);
		Information("FxCop violations = {0}",fxCopViolationCount);
		if(fxCopViolationCount!=0 || styleCopViolationCount!=0)
		{
			//throw new Exception("Code violations found");
            Information("Code violations found");
		}
		else
		{
			Information("Code Analysis succees");
		}
		
});


		Task("DotCoverCover").Does(()=>{
		if(!DirectoryExists(cireports+"/xunit"))
			  {
					CreateDirectory(cireports+"/xunit");
			  }
		DotCoverCover(tool => {
						  tool.XUnit2(currentDirectory + @"\test\"+projectName+@".Test\bin\"+configuration+@"\*.Test.dll",
						  new XUnit2Settings {
						  Parallelism = ParallelismOption.All,
						  HtmlReport = true,
						  XmlReport = true,
						  OutputDirectory = xunitFolder
						  });},
						   new FilePath(cireports+"/codecoverage/UnitTestCover.dcvr"),
						 new DotCoverCoverSettings()
						  .WithScope(currentDirectory + @"\test\"+projectName+@".Test\bin\*.dll")
						.WithFilter("-:*Test")
					);
			  })
			  .OnError(exception =>
		{ 
			throw new Exception("One or more test cases failed");
		});

Task("test")
      .IsDependentOn("DotCoverCover")
      .ContinueOnError()
      .Does(()=>{
            DotCoverReport(new FilePath(codecoverageFolder+"/UnitTestCover.dcvr"),
            new FilePath(codecoverageFolder+"/UnitTestCover.html"),
            new DotCoverReportSettings {
                  ReportType = DotCoverReportType.HTML
            });  
            DotCoverReport(new FilePath(codecoverageFolder+"/UnitTestCover.dcvr"),
            new FilePath(codecoverageFolder+"/UnitTestCover.xml"),
                  new DotCoverReportSettings {
                        ReportType = DotCoverReportType.XML
            });  
            var  coveragePercent =(from elements in System.Xml.Linq.XDocument.Load(codecoverageFolder+"/UnitTestCover.xml").Descendants("Root")
                                    select (string)elements.Attribute("CoveragePercent")).FirstOrDefault();
           
            FileStream fs1 = new FileStream(codecoverageFolder+"/UnitTestCover.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            StreamWriter writer = new StreamWriter(fs1);
            writer.Write(coveragePercent);
            writer.Close();
      });
	  
//////////////////////////////////////////////////////////////////////
// compile TASK TARGETS
//////////////////////////////////////////////////////////////////////
Task("Download-Cake")
.IsDependentOn("Download-Nugetexe")
.Does(()=>{
   StartProcess("./tools/nuget.exe",new ProcessSettings{ Arguments = "install Cake -Version 0.35.0  -ExcludeVersion -OutputDirectory " +currentDirectory+ "/build/tools/" });
});

Task("CompileSource") 
.Does(()=>{
RunTarget("DeleteLogFile");
var cakeFilePath= currentDirectory + "/compile-dependencies/compile.cake";
Information("Compile Cake file path:"+cakeFilePath);
StartProcess(cakeExe.ToString(), new ProcessSettings
                { Arguments = cakeFilePath + " -target=Default -configuration=" + configuration + " -studio_version=" + STUDIO_VERSION + " -site=" +  site + " -domain=" + domain + " -GitUserName=" + GitUserName + " -GitPassword=" + GitPassword + " -nugetserverurl=" + buildCompileNugetServerUrl + " -DefineConstants=" +DefineConstants});

}).OnError(exception =>
{ 
			throw new Exception("Dashboard compilation failed");
});

Task("onpremiseboldbi")
	.IsDependentOn("Clear-NuGet-Cache")
	.IsDependentOn("Download-Cake")
	.IsDependentOn("CompileSource")
	.IsDependentOn("Publish");
	  
//////////////////////////////////////////////////////////////////////
// Publish TASK TARGETS
//////////////////////////////////////////////////////////////////////
Task("Publish")
	.IsDependentOn("Clear-NuGet-Cache")
	.IsDependentOn("build")
	.Does(()=>{
	//RunTarget("DeleteLogFile");
	Information(publishProfilefilepath);
	

	if(configuration == "onpremiseboldbi")
	{
		MSBuild(currentDirectory + @"\src\Syncfusion.Dashboard.Designer.Web.Service.csproj",  new MSBuildSettings()
			.AddFileLogger(new MSBuildFileLogger { LogFile="../cireports/warnings/"+projectName+".txt",MSBuildFileLoggerOutput=MSBuildFileLoggerOutput.WarningsOnly})
			.AddFileLogger(new MSBuildFileLogger { LogFile="../cireports/errorlogs/"+projectName+".txt",MSBuildFileLoggerOutput=MSBuildFileLoggerOutput.ErrorsOnly})
			.WithProperty("DeployOnBuild","true")
			.WithProperty("PublishProfile",publishProfilefilepath)
			.WithProperty("password",password)
			.WithProperty("SkipExtraFilesOnServer",SkipExtraFilesOnServer)
			.WithProperty("referencepath",referencepath)
			.WithProperty("NexusServerUrl",nugetserverurl)
			.WithProperty("PublishDomain",domain)
			.WithProperty("CompilationFrom","cake")
			.SetConfiguration(configuration)			
			.SetVerbosity(Verbosity.Minimal));
			
			}
			else
			{
			var settings = new DotNetCoreMSBuildSettings()
						.WithProperty("DeployOnBuild","false")
						.WithProperty("NexusServerUrl",nugetserverurl)
						.WithProperty("PublishDomain",domain)
						.WithProperty("CompilationFrom","cake")
						.AddFileLogger(new MSBuildFileLoggerSettings{LogFile="../cireports/warnings/"+projectName+".txt", SummaryOutputLevel=MSBuildLoggerOutputLevel.WarningsOnly})
						.AddFileLogger(new MSBuildFileLoggerSettings{LogFile="../cireports/errorlogs/"+projectName+".txt", SummaryOutputLevel=MSBuildLoggerOutputLevel.ErrorsOnly});

	var publishSettings = new DotNetCorePublishSettings

		{
			Configuration = configuration,
			OutputDirectory = currentDirectory +@"\output\dataservice",
			MSBuildSettings = settings,
			NoRestore = true,
			Verbosity = DotNetCoreVerbosity.Minimal
		};

		Information("==================");
		Information("Started publishing "+ "Syncfusion.Dashboard.Designer.Web.Service");
		Information(publishSettings.Configuration);
		Information(publishProfilefilepath);
		Information("==================");

		DotNetCorePublish(currentDirectory + @"\src\Syncfusion.Dashboard.Designer.Web.Service.csproj", publishSettings);
			}
		//RunTarget("DeleteLogFile");
		var logFilename = projectName + ".txt";
        if(FileExists(errorlogFolder + logFilename))
        {
            if (FileSize(errorlogFolder + logFilename) == 0 )
                DeleteFile(errorlogFolder + logFilename);
        }
		
	}).OnError(exception =>
		{ 
			throw new Exception("Dashboard designer service publish failed");
		});


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("build")
    .IsDependentOn("codeviolation");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
