%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe ..\Pulsus.sln /t:Clean,Rebuild /p:Configuration=Release /fileLogger

copy ..\Pulsus\bin\Release\Pulsus.dll Packages\Pulsus\lib\net35\
copy ..\Pulsus\bin\Release\Pulsus.pdb Packages\Pulsus\lib\net35\
copy ..\Pulsus\bin\Release\Pulsus.xml Packages\Pulsus\lib\net35\

copy ..\Pulsus.Mvc\bin\Release\Pulsus.Mvc.dll Packages\Pulsus.Mvc\lib\net4\
copy ..\Pulsus.Mvc\bin\Release\Pulsus.Mvc.pdb Packages\Pulsus.Mvc\lib\net4\
copy ..\Pulsus.Mvc\bin\Release\Pulsus.Mvc.xml Packages\Pulsus.Mvc\lib\net4\

..\..\tools\.nuget\nuget.exe update -self
..\..\tools\.nuget\nuget.exe pack Pulsus.nuspec -Symbols -BasePath Packages\Pulsus -Output Packages
..\..\tools\.nuget\nuget.exe pack Pulsus.Mvc.nuspec -Symbols -BasePath Packages\Pulsus.Mvc -Output Packages