Feature: Search

Scenario: Tracks can be searched by the counter part of a special character
Given All data scripts
When searching for S10 & BLOF
Then the following tracks are found:
| Id   | Title       | Artist     | Recorded Year |
| 4902 | Laat Me Los | S10 & BLØF | 2022          |
When searching for /\
Then the following tracks are found:
| Id   | Title          | Artist            | Recorded Year |
| 4056 | Sun Is shining | Axwell Λ Ingrosso | 2015          |

Scenario: Track can be searching by title
Given All data scripts
When searching for Piano
Then the following tracks are found:
| Id   | Title           | Artist            | Recorded Year |
| 2134 | My old piano    | Diana Ross        | 1980          |
| 2358 | Piano Man       | Billy Joel        | 1974          |
| 3558 | Worn Down Piano | Mark & Clark Band | 1977          |

Scenario: Tracks can be searching by artist
Given All data scripts
When searching for Soleil
Then the following tracks are found:
| Id  | Title   | Artist           | Recorded Year |
| 119 | Alegria | Cirque du Soleil | 1997          |

Scenario: Track can be searched by their full RecordedYear
Given All data scripts
When searching for 1954
Then the following tracks are found:
| Id   | Title           | Artist      | Recorded Year |
| 3468 | White Christmas | Bing Crosby | 1954          |

Scenario: Searching track by their RecordedYear are only found by searching the full year
Given All data scripts
When searching for 54
Then the track White Christmas is not found

Scenario: Searching will have a result cap of 100
Given All data scripts
When searching for a
Then the results contain 100 items

Scenario: An empty search querty give zero results
Given All data scripts
When searching without a query
Then the results contain 0 items