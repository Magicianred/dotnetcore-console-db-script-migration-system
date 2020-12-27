# dotnetcore-console-db-script-migration-system

An implementation of a Console program for [SimpleDbScriptMigrationSystem](https://github.com/Magicianred/SimpleDbScriptMigrationSystem) in dotnet core for SqlServer.  

## Instructions
1. Create a database in your SqlServer instance.

2. Edit *consoleSettings.json* or create the *consoleSettings.Development.json* file and use your settings:
	- Replace _YOUR_CONNECTION_STRING_ with your connection string to SqlServer
	- Replace *YOUR_FOLDERS* and *YOUR_FOLDERS/YOUR_FILES* with the name of your folder(s) and/or your file(s), see section [Files and Folders](files_and_folders).

	(if you create *consoleSettings.json* remember to add the property *CopyAlways* to file)

3. Configure your args for Debug
In file *ConsoleDbMigrationScript\ConsoleDbMigrationScript\Properties\launchSettings.json* configure *commandLineArgs* parameter with your db folder script.

4. If you want, use scripts in *DBScripts* folders for a sample and then remove them and use yours.  

## Command line args
- *--pathdbscripts* indicate a absolute or relative path where db scripts are.  

## Files and folders
You have to indicate where sql scripts are. You can write the name of the folder(s) or a list of files.  
The order of execution is FIFO, if it's a folder the app browse all files in alphabetic order.  
