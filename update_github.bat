@echo off
cd /d C:\Quality_Clinic_DemoV1

echo ================================
echo  Guardando y subiendo a GitHub...
echo ================================

:: Agrega todos los cambios
git add .

:: Crea un commit con fecha y hora
git commit -m "Actualización %date% %time%"

:: Sube al repositorio remoto
git push origin main

echo ================================
echo  ¡Proyecto subido a GitHub!
echo ================================
pause
