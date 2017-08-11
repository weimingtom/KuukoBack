@echo off
for %%i in (%cd%\src\kurumi\*.java) do (
	echo [process] %%~ni
	copy "csharp\%%~ni.cs" "src\kurumi\%%~ni.java"
)
@pause
@echo on

