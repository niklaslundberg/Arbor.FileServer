@ECHO OFF

SET DockerTempDir=%~dp0temp\docker
SET ArtifactSource=%~dp0Artifacts\Websites\Arbor.FileServer\AnyCPU\release\

rmdir /s /q "%DockerTempDir%"
mkdir "%DockerTempDir%"
mkdir "%DockerTempDir%\app"
copy docker\*.* "%DockerTempDir%"

copy "%ArtifactSource%\*.*" "%DockerTempDir%\app"

SET VERSION=1809

docker build -t nlundberg/arbor-file-server:%VERSION% --build-arg version=%VERSION% "%DockerTempDir%"
