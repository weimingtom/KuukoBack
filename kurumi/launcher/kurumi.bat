::@set OPTS=-Xdebug -Xrunjdwp:transport=dt_socket,server=y,address=8000,suspend=y
@java %OPTS%   -cp "%~dp0/../bin" kurumi.Program %*
@set OPTS=
