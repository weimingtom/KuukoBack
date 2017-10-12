@set LUA_ENABLE_LUAC=1
@java -cp "%~dp0/../bin" kurumi.Program %*
@set LUA_ENABLE_LUAC=
@pause
