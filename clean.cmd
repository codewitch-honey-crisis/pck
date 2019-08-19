@echo off
@echo deleting pdb files
@del /F /S /Q *.pdb > NUL
@echo deleting object folders
@rmdir /S /Q pck\obj > NUL
@rmdir /S /Q pckw\obj > NUL
@rmdir /S /Q pckp\obj > NUL
@rmdir /S /Q pckedit\obj > NUL
@rmdir /S /Q cfg\obj > NUL
@rmdir /S /Q ll1\obj > NUL
@rmdir /S /Q lalr1\obj > NUL
@rmdir /S /Q fa\obj > NUL
@rmdir /S /Q fagui\obj > NUL
@rmdir /S /Q xbnf\obj > NUL
