﻿@echo off
setlocal
set PATH={gccBinPath};%PATH%
if exist out (
    ren out out.tmp
    rmdir /s /q out.tmp
)
mkdir out
gcc.exe -I {includePath} -save-temps=obj {optimizeFlag} -g -fdata-sections -ffunction-sections -Wl,--gc-sections -Wl,--enable-stdcall-fixup -o out\test.exe test.c
if %errorlevel% neq 0 exit /b
{disableObjDump}if exist out\test.exe ( objdump.exe -d out\test.exe > out\test.dmp )
