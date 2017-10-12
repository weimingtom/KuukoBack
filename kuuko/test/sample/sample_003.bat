@set PATH=D:\msys_v11\msys\bin;%PATH%

@luac -p -l sample.lua | lua ../globals.lua | sort > sample_003.txt
@pause
