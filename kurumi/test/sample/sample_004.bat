@set PATH=D:\msys_v11\msys\bin;%PATH%

@luac -p -l sample.lua | lua ../globals.lua | sort | lua ../table.lua > sample_004.txt
@pause
