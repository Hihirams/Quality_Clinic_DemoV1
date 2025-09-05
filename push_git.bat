@echo off
cd /d D:\Unity\Quality_ClinicDemoV1

echo ========================================
echo   Subiendo proyecto Unity a GitHub...
echo ========================================

:: Agregar todos los cambios
git add .

:: Crear commit con fecha y hora automática
set fecha=%date% %time%
git commit -m "Auto commit: %fecha%"

:: Subir a la rama main
git push origin main

echo ========================================
echo   Proyecto subido correctamente 🚀
echo ========================================
pause
