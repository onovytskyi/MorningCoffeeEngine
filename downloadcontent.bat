@echo off

:: Replace with your file ID
set FILE_ID=1rFyRa7zgU9abUykJ8f_sbZg5Vat_y7YL

:: Replace with your desired file name
set FILE_NAME="%~dp0Content\models\IntelSponza\sponza_base.wiscene"
 
echo "Downloading base Intel Sponza"
:: Download the file with detailed progress meter
curl -# -L -o %FILE_NAME% "https://drive.usercontent.google.com/download?export=download&authuser=0&confirm=t&uuid=23d8334f-5701-4e84-a933-68d9995401cc&at=APZUnTX1lo1mmZ_amgnuGeBYvaLS%3A1719063913725&id=%FILE_ID%"

echo Download complete
pause