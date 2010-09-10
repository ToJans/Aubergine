@echo off
echo.
echo Cleaning up. Please wait...

for /f "delims=" %%d in ('dir obj.* /b/a:d/s 2^>NUL') do rmdir /s/q "%%d"
for /f "delims=" %%d in ('dir bin.* /b/a:d/s 2^>NUL') do rmdir /s/q "%%d"

echo.
echo Done.
echo.

