@echo off
for %%i in (%cd%\src\kurumi\*.java) do (
	echo [process] %%~ni
	copy "src\kurumi\%%~ni.java" "csharp_\%%~ni.cs"
)
@pause
@echo on

