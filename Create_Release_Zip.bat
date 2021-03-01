@ECHO OFF
"%ProgramFiles%\7-Zip\7z" a ..\GreyHak-DSP_Save_Game_Sorter-9.zip icon.png LICENSE.txt manifest.json README.md
cd bin\Release
"%ProgramFiles%\7-Zip\7z" u ..\..\..\GreyHak-DSP_Save_Game_Sorter-9.zip DSPSaveGameSorter.dll
