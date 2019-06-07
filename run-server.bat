@ECHO OFF

REM CALL %~dp0build\build.bat

IF "%ERRORLEVEL%" NEQ "0" (
  EXIT /B %ERRORLEVEL%
)

CALL %~dp0build-docker.bat

IF "%ERRORLEVEL%" NEQ "0" (
  EXIT /B %ERRORLEVEL%
)

docker run -it -p 5000:80 -v c:\temp\files:c:/files nlundberg/arbor-file-server

EXIT /B %ERRORLEVEL%
