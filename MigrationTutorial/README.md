# IMPORTANT!
This is just a temporary repo that holds the migration tutorial for Realm. This will be moved to the centralised repo for examples when one is defined.

# General info
This is a sample project to showcase how to execute migrations in Realm .NET.
To run the application, run the command line binary like `./MigrationTutorial --schema_version [1-3]`.

The application simulates a business that goes through 2 different migrations. So, `schema_version 1` just creates the database with some minimal models. `schema_version 2` and `schema_version 3` make some changes to the models, hence the application operates migrations to accomodate the changes.  
The application should only be run with the schemas in the order 1 to 3 without skipping any schema or inverting the order. If given 1 again after a run, it is assumed that the user wants to start over so the db file will be deleted.