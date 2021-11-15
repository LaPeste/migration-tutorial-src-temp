# IMPORTANT!
This is just a temporary repo that holds the migration tutorial for Realm. This will be moved to the centralised repo for examples when one is defined.

# General info
This is a sample project to showcase how to execute migrations in an application using Realm .NET.
The application simulates a business that goes through 2 different migrations. So, `schema_version 1` just creates the database with some minimal models. `schema_version 2` and `schema_version 3` make some changes to the models, hence the application operates migrations to accomodate the changes.
The application should only be run with the schemas in the order 1 through 3, with the ability to skip 2. If given 1 again after any run, it is assumed that the user wants to start over so the realm file will be deleted.

## Techincal info
To build the application, define one of the follow symbol for the compiler:
* `SCHEMA_VERSION_1` or
* `SCHEMA_VERSION_2` or
* `SCHEMA_VERSION_3`

The binary will be produced either at
* `MigrationTutorial/MigrationTutorial/bin/Debug/net5.0` or
* `MigrationTutorial/MigrationTutorial/bin/Release/net5.0`

depending on the configuration that you selected.

### From command line
To build your application and set the needed preprocessor symbol, go at the root of the solution and execute the following command: `dotnet build . -p:DefineConstants=SCHEMA_VERSION_X`. Then, to run the application change directory to where the binary is outputted and run `dotnet MigrationTutorial.dll`.

### From Visual Studio
If instead you want to do it from Visual Studio for Mac, go to the properties of the MigrationTutorial project, then in `Build/General/Compiler/Define Symbols` add `SCHEMA_VERSION_X`.
In Visual Studio on Windows the naming of the setting should be really similar.