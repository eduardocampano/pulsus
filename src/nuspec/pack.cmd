%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe ..\Pulsus.sln /t:Clean,Rebuild /p:Configuration=Release /fileLogger

if not exist Packages mkdir Packages\

if not exist Packages\Pulsus mkdir Packages\Pulsus\
if not exist Packages\Pulsus\lib mkdir Packages\Pulsus\lib\
if not exist Packages\Pulsus\lib\net35 mkdir Packages\Pulsus\lib\net35\
copy ..\Pulsus\bin\Release\Pulsus.dll Packages\Pulsus\lib\net35\
if not exist Packages\Pulsus\lib\net4 mkdir Packages\Pulsus\lib\net4\
copy ..\Pulsus.Net4\bin\Release\Pulsus.dll Packages\Pulsus\lib\net4\

if not exist Packages\Pulsus.Mvc mkdir Packages\Pulsus.Mvc\
if not exist Packages\Pulsus.Mvc\lib mkdir Packages\Pulsus.Mvc\lib\
if not exist Packages\Pulsus.Mvc\lib\net4 mkdir Packages\Pulsus.Mvc\lib\net4\
copy ..\Pulsus.Mvc\bin\Release\Pulsus.Mvc.dll Packages\Pulsus.Mvc\lib\net4\

if not exist Packages\Pulsus.Web mkdir Packages\Pulsus.Web\
if not exist Packages\Pulsus.Web\lib mkdir Packages\Pulsus.Web\lib\
if not exist Packages\Pulsus.Web\lib\net4 mkdir Packages\Pulsus.Web\lib\net4\
copy ..\Pulsus.Web\bin\Release\Pulsus.Web.dll Packages\Pulsus.Web\lib\net4\

if not exist Packages\Pulsus.Server mkdir Packages\Pulsus.Server\
if not exist Packages\Pulsus.Server\lib mkdir Packages\Pulsus.Server\lib\
if not exist Packages\Pulsus.Server\lib\net4 mkdir Packages\Pulsus.Server\lib\net45\
copy ..\Pulsus.Server\bin\Release\Pulsus.Server.dll Packages\Pulsus.Server\lib\net45\

nuget.exe pack Pulsus.nuspec -BasePath Packages\Pulsus -Output Packages
nuget.exe pack Pulsus.Web.nuspec -BasePath Packages\Pulsus.Web -Output Packages
nuget.exe pack Pulsus.Mvc.nuspec -BasePath Packages\Pulsus.Mvc -Output Packages
nuget.exe pack Pulsus.Server.nuspec -BasePath Packages\Pulsus.Server -Output Packages