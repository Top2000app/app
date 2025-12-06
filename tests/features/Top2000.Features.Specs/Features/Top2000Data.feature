Feature: Top2000Data

Background: Data for the Top2000 app is stored in SQL scripts inside the `Data.SQLScripts` assembly. This assembly is then used in the `Data.ClientDatabase` project which creates the client database. 

On the offcial Top2000 website (www.top2000.nl) people can choose their top tracks for the Top2000. After the voting week the first 10 tracks are published. A few days before the show starts the full list is published. 

Scenario: All editions contains 2000 positions starting with 1 and ending with 2000
Given the client database is created
Then the listing table contains 2000 tracks for each edition ranging from 1 to 2000

Scenario: The last edition can either have 10 or 2000 tracks
Given the client database is created
When the latest edition is queried
Then the latest edition contains either 10 or 2000 items

Scenario: The playtime of each track is either the same to the last track or increment by one hour
Given the client database is created
Then for each track in the listing table the PlayDateAndTime is the same to the previous track or has incremented by one hour

Scenario: Edition 2023 has 2500 tracks
Given All data scripts
When the client database is created
Then the listing table of edition 2023 has 2500 tracks