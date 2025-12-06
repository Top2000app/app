Feature: AllListingsOfEdition

Scenario: In 1998 the Top2000 did not exist
Background: the system won't throw an exception when the year since found. 
It is the responsibilty of the caller to make sure the year exist. 
Given All data scripts
When the AllListingOfEdition feature is executed with year 1998
Then an empty set is returned

Scenario: In 2023 the Top2000 has 2500 tracks
Given All data scripts
When the AllListingOfEdition feature is executed with year 2023
Then the start of the list is 1
And the end of the list is 2500