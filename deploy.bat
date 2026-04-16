@echo off
:: Cambia esto por la ruta real donde está tu carpeta de proyecto
cd /d "D:\ProyectosSoftware\OnpeScrapper\OnpeScrapper"

echo Ejecutando Scraper...
dotnet run --project OnpeScrapper.csproj

echo Subiendo cambios a GitHub...
git add index.html
git commit -m "Actualizacion automatica: %date% %time%"
git push origin main

echo ¡Listo!
pause