@echo off
setlocal

echo ===============================
echo 检查当前安装的 .NET SDK
echo ===============================
dotnet --list-sdks
echo.
echo 当前默认使用的 SDK 版本：
dotnet --version
echo.

REM 可选：检测是否包含 10.0 SDK
set "hasNet10=false"
for /f "tokens=1 delims=." %%v in ('dotnet --version') do (
    if %%v GEQ 10 (
        set hasNet10=true
    )
)

if "%hasNet10%"=="true" (
    echo ? 检测到 .NET 10 SDK，继续构建...
) else (
    echo ?? 警告：未检测到 .NET 10 SDK，构建可能失败！
)

echo.
echo ===============================
echo 清理解决方案...
echo ===============================
dotnet clean

echo.
echo ===============================
echo 删除所有 bin 和 obj 文件夹...
echo ===============================
for /r %%i in (bin,obj) do (
    if exist "%%i" (
        echo 删除：%%i
        rmdir /s /q "%%i"
    )
)

echo.
echo ===============================
echo 重新还原依赖项...
echo ===============================
dotnet restore --force

echo.
echo ===============================
echo 开始构建解决方案...
echo ===============================
dotnet build --no-restore

echo.
echo ======= 构建完成 =======
pause
