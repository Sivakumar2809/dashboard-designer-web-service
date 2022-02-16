Param(
  [string]$projectDir,
  [string]$nuggetUrl,
  [string]$domain,
  [string]$referencePath
)
$projectDir,
$nuggetUrl
$referencePath
$source=$projectDir+"PluginsInstaller"
$destination=$projectDir+"Plugins\connections"
$nuget = $source+"\nuget.exe"
$packageXml= $source+"\packages.xml"
$appData=$projectDir+"App_Data\"


function InstallNuget{
Invoke-WebRequest "https://www.nuget.org/nuget.exe" -OutFile $nuget
}
function MoveOAuthConfig(){
Param([string]$domain,[string]$appData)
Write-Output "Input Domain in move function:"+ $domain
[string]$destination = $appData +"OAuthConfiguration.json"
if(-not ([string]::IsNullOrEmpty($domain)) -and $domain.ToLowerInvariant() -eq "boldbi" -and ([string]::IsNullOrEmpty($referencePath)) ){
  [string]$source = $appData +"BoldBI_OAuthConfiguration.json"
  Copy-Item -Path $source -Destination $destination -Force 
} elseif(-not ([string]::IsNullOrEmpty($domain)) -and $domain.ToLowerInvariant() -eq "syncfusion" ){
[string]$source = $appData +"Syncfusion_OAuthConfiguration.json"
  Copy-Item -Path $source -Destination $destination -Force 
}
}
function CopyPlugins {
Param ([string]$source,[string]$destination)
$libDirs=Get-ChildItem -Filter "lib" -Recurse -Path $source | Select-Object FullName
ForEach($libDir in $libDirs)
{
Copy-Item -Path $libDir.FullName  -Destination $destination -Recurse -force
}
}
function CopyPluginsFromAssemblyDir {
Param ([string]$source,[string]$destination)
$libDirs=Get-ChildItem -Recurse -Path $source | Select-Object FullName
ForEach($libDir in $libDirs)
{
Copy-Item -Path $libDir.FullName  -Destination $destination -Recurse -force
}
}


function RecursiveDelete {
Param([string]$theroot)
  $children = Get-ChildItem -Path $theroot | where-object { $_.Attributes -eq "Directory"} |% {$_.FullName}
  foreach($achild in $children) {
  	if ($achild -ne $null) {
    	    RecursiveDelete $achild -ErrorAction SilentlyContinue
	}
  }
  Remove-Item $theroot -recurse -force 
}


#Methods calling 
InstallNuget
Write-Output "Input Domain from VS project:"+ $domain
MoveOAuthConfig -domain $domain -appData $appData
[xml]$XmlContent = Get-Content -Path $packageXml
Write-Output "Reference Path:" + $referencePath
[System.Xml.XmlElement] $Plugins = $xmlContent.Plugins
Foreach($plugin in $Plugins.ChildNodes)
{
$PluginDir=$destination+"\"+$Plugin.name
New-Item -ItemType Directory -Force -Path $PluginDir
if([string]:: IsNullOrEmpty($referencePath)){
Foreach($package in $plugin.ChildNodes)
{

$id=$package.id
$tempLocation=$source+"\temp"
& $nuget install $id -OutputDirectory $tempLocation -Source $nuggetUrl
}
CopyPlugins -source $source  -destination $PluginDir
RecursiveDelete -theroot $tempLocation 
}
else{
$folderPath=  $referencePath + $Plugin.name
Write-Output "Plugin Path:" + $folderPath
CopyPluginsFromAssemblyDir -source $folderPath  -destination $PluginDir
}
}