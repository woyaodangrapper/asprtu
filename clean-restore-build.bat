@echo off
setlocal

echo ===============================
echo ��鵱ǰ��װ�� .NET SDK
echo ===============================
dotnet --list-sdks
echo.
echo ��ǰĬ��ʹ�õ� SDK �汾��
dotnet --version
echo.

REM ��ѡ������Ƿ���� 10.0 SDK
set "hasNet10=false"
for /f "tokens=1 delims=." %%v in ('dotnet --version') do (
    if %%v GEQ 10 (
        set hasNet10=true
    )
)

if "%hasNet10%"=="true" (
    echo ? ��⵽ .NET 10 SDK����������...
) else (
    echo ?? ���棺δ��⵽ .NET 10 SDK����������ʧ�ܣ�
)

echo.
echo ===============================
echo ����������...
echo ===============================
dotnet clean

echo.
echo ===============================
echo ɾ������ bin �� obj �ļ���...
echo ===============================
for /r %%i in (bin,obj) do (
    if exist "%%i" (
        echo ɾ����%%i
        rmdir /s /q "%%i"
    )
)

echo.
echo ===============================
echo ���»�ԭ������...
echo ===============================
dotnet restore --force

echo.
echo ===============================
echo ��ʼ�����������...
echo ===============================
dotnet build --no-restore

echo.
echo ======= ������� =======
pause
