Feature: TrackInformation

Background: Every track can be opened to view additional information. 

Scenario: Track information contains the static data from a track
Given the client database is created
When the track information feature is executed for TrackId 1780
Then the title is "L'Été Indien" from 'Joe Dassin' which is recorded in the year 1975

Scenario: Tracks that are recorded after the first edition are listed as Not available
Given the client database is created
When the track information feature is executed for TrackId 1267
Then the title is "Hurt" from 'Johnny Cash' which is recorded in the year 2003
And the following years are listed as 'NotAvailable'
| Edition |
| 2002    |
| 2001    |
| 2000    |
| 1999    | 

Scenario: Track that were recorded but did not made it on the list are listed as NotListed
Given the client database is created
When the track information feature is executed for TrackId 1267
Then the title is "Hurt" from 'Johnny Cash' which is recorded in the year 2003
And the following years are listed as 'NotListed'
| Edition |
| 2003    |
| 2004    |
| 2005    |
| 2006    |
| 2007    |
| 2008    |

Scenario: The first time a track is listed the status of the listing is New
Given the client database is created
When the track information feature is executed for TrackId 1267
Then the title is "Hurt" from 'Johnny Cash' which is recorded in the year 2003
And the listing 2009 is listed as 'New'

Scenario: Tracks that are listed again but was not in the previous edition the status is listed as Back
Given the client database is created
When the track information feature is executed for TrackId 2560
Then the title is "Sad But True" from 'Metallica' which is recorded in the year 1993
And the listing 2011 is listed as 'Back'

Scenario: Tracks that are higher on the list than previous edition are listed are Increased and a offset is indicating the delta
Given the client database is created
When the track information feature is executed for TrackId 1664
Then the title is "Killer Queen" from 'Queen' which is recorded in the year 1974
And the following years are listed as 'Increased'
| Edition | Offset |
| 2000    | 33     |
| 2002    | 19     |
| 2008    | 262    |
| 2011    | 85     |
| 2017    | 9      |
| 2018    | 222    | 

Scenario: Tracks that are lower on the list than previous edition are listed are Decreased and a offset is indicating the delta
Given the client database is created
When the track information feature is executed for TrackId 1664
Then the title is "Killer Queen" from 'Queen' which is recorded in the year 1974
And the following years are listed as 'Decreased'
| Edition | Offset |
| 2001    | 27     |
| 2003    | 1      |
| 2004    | 30     |
| 2005    | 45     |
| 2006    | 27     |
| 2007    | 255    |
| 2009    | 7      |
| 2010    | 76     |
| 2012    | 13     |
| 2013    | 27     |
| 2014    | 18     |
| 2015    | 10     |
| 2016    | 3      |
| 2019    | 18     |
| 2020    | 25     |
| 2020    | 38     |

Scenario: Tracks that haven't change position since the previous edition are listed as Unchanged
Given the client database is created
When the track information feature is executed for TrackId 2218
Then the title is "Nothing Else Matters" from 'Metallica' which is recorded in the year 1992
And the following years are listed as 'Unchanged'
| Edition | Offset |
| 2017    | 0      |
| 2012    | 0      |

Scenario: 'Since release' is the statistic that shows how many times the tracks could have been listed
Given the client database is created
When the track information feature is executed for TrackId 3966
Then the title is "Hello" from 'Adele' which is recorded in the year 2015
And it could have been on the Top2000 for 10 times
And is it listed for 10 times

Scenario: Record high shows the highest listing for the track
Given the client database is created
When the track information feature is executed for TrackId 1496
Then the title is "Imagine" from 'John Lennon' which is recorded in the year 1971
And the record high is number 1 on 2015

Scenario: Record low shows the lowest listing for the track
Given the client database is created
When the track information feature is executed for TrackId 1496
Then the title is "Imagine" from 'John Lennon' which is recorded in the year 1971
And the record low is number 52 in 2022

Scenario: Last postion shows the position of latest edition where the track was listed
Given the client database is created
When the track information feature is executed for TrackId 1496
Then the title is "Imagine" from 'John Lennon' which is recorded in the year 1971
And the Lastest position is number 46 in 2024

Scenario: First position shows the position of the first edition where the track was listed
Given the client database is created
When the track information feature is executed for TrackId 1496
Then the title is "Imagine" from 'John Lennon' which is recorded in the year 1971
And the first position is number 7 in 1999