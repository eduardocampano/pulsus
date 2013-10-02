@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe Pulsus.sln /t:Clean,Rebuild /p:Configuration="%config%" /m /nr:false /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal

if not "%errorlevel%"=="0" goto failure

rd Build /s /q 

if not exist Build mkdir Build\

REM Pulsus
if not exist Build\Pulsus mkdir Build\Pulsus\
if not exist Build\Pulsus\lib mkdir Build\Pulsus\lib\
if not exist Build\Pulsus\lib\net35 mkdir Build\Pulsus\lib\net35\
if not exist Build\Pulsus\lib\net4 mkdir Build\Pulsus\lib\net4\
copy LICENSE.md Build\Pulsus\
copy Pulsus\bin\%config%\Pulsus.dll Build\Pulsus\lib\net35\
copy Pulsus\bin\%config%\Pulsus.pdb Build\Pulsus\lib\net35\
copy Pulsus\bin\%config%\Pulsus.xml Build\Pulsus\lib\net35\
copy Pulsus.Net4\bin\%config%\Pulsus.dll Build\Pulsus\lib\net4\
copy Pulsus.Net4\bin\%config%\Pulsus.pdb Build\Pulsus\lib\net4\
copy Pulsus.Net4\bin\%config%\Pulsus.xml Build\Pulsus\lib\net4\

REM Pulsus.Mvc
if not exist Build\Pulsus.Mvc mkdir Build\Pulsus.Mvc\
if not exist Build\Pulsus.Mvc\lib mkdir Build\Pulsus.Mvc\lib\
if not exist Build\Pulsus.Mvc\lib\net4 mkdir Build\Pulsus.Mvc\lib\net4\
copy LICENSE.md Build\Pulsus.Mvc\
copy Pulsus.Mvc\bin\%config%\Pulsus.Mvc.dll Build\Pulsus.Mvc\lib\net4\
copy Pulsus.Mvc\bin\%config%\Pulsus.Mvc.pdb Build\Pulsus.Mvc\lib\net4\
copy Pulsus.Mvc\bin\%config%\Pulsus.Mvc.xml Build\Pulsus.Mvc\lib\net4\

REM Pulsus.Web
if not exist Build\Pulsus.Web mkdir Build\Pulsus.Web\
if not exist Build\Pulsus.Web\lib mkdir Build\Pulsus.Web\lib\
if not exist Build\Pulsus.Web\lib\net4 mkdir Build\Pulsus.Web\lib\net4\
copy LICENSE.md Build\Pulsus.Web\
copy Pulsus.Web\bin\%config%\Pulsus.Web.dll Build\Pulsus.Web\lib\net4\
copy Pulsus.Web\bin\%config%\Pulsus.Web.pdb Build\Pulsus.Web\lib\net4\
copy Pulsus.Web\bin\%config%\Pulsus.Web.xml Build\Pulsus.Web\lib\net4\

REM Pulsus.Server
if not exist Build\Pulsus.Server mkdir Build\Pulsus.Server\
if not exist Build\Pulsus.Server\lib mkdir Build\Pulsus.Server\lib\
if not exist Build\Pulsus.Server\lib\net4 mkdir Build\Pulsus.Server\lib\net45\
copy LICENSE.md Build\Pulsus.Server\
copy Pulsus.Server\bin\%config%\Pulsus.Server.dll Build\Pulsus.Server\lib\net45\
copy Pulsus.Server\bin\%config%\Pulsus.Server.pdb Build\Pulsus.Server\lib\net45\
copy Pulsus.Server\bin\%config%\Pulsus.Server.xml Build\Pulsus.Server\lib\net45\

nuget.exe pack Pulsus.nuspec -Symbols -BasePath Build\Pulsus -Output Build
nuget.exe pack Pulsus.Web.nuspec -Symbols -BasePath Build\Pulsus.Web -Output Build
nuget.exe pack Pulsus.Mvc.nuspec -Symbols -BasePath Build\Pulsus.Mvc -Output Build
nuget.exe pack Pulsus.Server.nuspec -Symbols -BasePath Build\Pulsus.Server -Output Build

if not "%errorlevel%"=="0" goto failure

:success

REM success

rem exit 0

:failure

REM failure

rem exit -1