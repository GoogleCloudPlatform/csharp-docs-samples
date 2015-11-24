SETLOCAL
SET MSBUILD="MSBuild.exe"
SET NUGET="Nuget.exe"
SET MSTEST="MSTest.exe"
REM Dump the environment variables.
SET FAILED=0
SET

CD bigquery\api\BigQueryUtil
%NUGET% restore
%MSBUILD% && %MSTEST% /testcontainer:test\bin\debug\test.dll || SET FAILED=1

CD ..\GettingStarted
%NUGET% restore
%MSBUILD% && %MSTEST% /testcontainer:test\bin\debug\test.dll || SET FAILED=1

CD ..\..\..\storage\api
%NUGET% restore
%MSBUILD% && %MSTEST% /testcontainer:test\bin\debug\test.dll || SET FAILED=1

CD ..\..\pubsub\api
%NUGET% restore
%MSBUILD% && %MSTEST% /testcontainer:test\bin\debug\test.dll || SET FAILED=1

@ECHO OFF
IF %FAILED% NEQ 0 GOTO failed_case
ENDLOCAL
ECHO SUCCEEDED
EXIT /b 

:failed_case
ENDLOCAL
ECHO FAILED
EXIT /b 1
