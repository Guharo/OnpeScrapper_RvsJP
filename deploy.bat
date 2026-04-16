@echo off
echo Ejecutando Scraper...
dotnet run --project OnpeScrapper.csproj
echo Subiendo cambios a GitHub...
git add index.html
git commit -m "Actualizacion de datos: %date% %time%"
git push origin main
echo ¡Listo!