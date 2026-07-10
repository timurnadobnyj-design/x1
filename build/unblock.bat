@echo off
rem ============================================================
rem  Skilful: снять Mark of the Web (метку "файл из интернета")
rem  со всех DLL/EXE рядом. Запускать НА WINDOWS двойным кликом,
rem  лежать должен в одной папке со Skilful.exe и плагинами.
rem ============================================================
setlocal

echo === DO unblock: blocked status (True = помечен, .NET НЕ грузит) ===
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "Get-ChildItem '%~dp0*.dll','%~dp0*.exe' | ForEach-Object { $z = Get-Item $_.FullName -Stream Zone.Identifier -ErrorAction SilentlyContinue; '{0,-40} blocked={1}' -f $_.Name,[bool]$z }"

echo.
echo === UNBLOCK (снимаю метку) ===
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "Get-ChildItem '%~dp0*.dll','%~dp0*.exe' | Unblock-File; Write-Host 'OK'"

echo.
echo === POSLE unblock: должно стать blocked=False ===
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
  "Get-ChildItem '%~dp0*.dll','%~dp0*.exe' | ForEach-Object { $z = Get-Item $_.FullName -Stream Zone.Identifier -ErrorAction SilentlyContinue; '{0,-40} blocked={1}' -f $_.Name,[bool]$z }"

echo.
echo Gotovo. Teper zapusti Skilful.exe i otkroy Data Source - Select Data Source.
pause
