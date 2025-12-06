Feature: Client database

Background: Behind the scenes the Top2000 app are retrieving the data from an SQLite database. 
This way the apps can preform completely offline. When a new year is release, the Top2000 app needs
to update an their client databases. This is done in two ways:

- First a set of SQL scripts is shipped with every release. When a new version of the app is published
to the store and the new version is downloaded on each client’s device, the app can update its database 
from those scripts.  (These scripts are also used to create the initial database). 
- Secondly the SQL are also uploaded to the Top2000.app website. The website is used as a shortcut to
provide faster updates to the client database (Since publishing an app though app stores and get the updates
to the clients can take up several hours or maybe even days).

Scenario: Application is installed with a fresh client database
Given A new install of the application
When the application starts
Then the client database is created with the scripts from the top2000 data assembly

Scenario: Client database is updated upon startup when new scripts are shipped
Given an installed application without the last 1 SQL scripts
When the application starts
Then the client database is created with the scripts from the top2000 data assembly

Scenario: Client database is updated upon startup when new scripts are shipped on the Top2000 website.
Given an installed application without the last 1 SQL scripts
When the application starts without the last SQL scripts
Then the application checks online for updates
And the client database is updated

Scenario: Client database is updated when new scripts are shipped in the assembly and the Top2000 website.
Given an installed application without the last 2 SQL scripts
When the application starts without the last SQL scripts
Then the application updates the second-to-last script from the assembly
And the application checks online for updates
And the client database is updated