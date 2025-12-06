Feature: All Editions

Scenario: The first edition of the Top2000 was in 1999
Given All data scripts
When the feature is executed
Then the latest year is 1999

Scenario: Every edition is returned starting with the highest year
Given All data scripts
When the feature is executed
Then a sorted set is returned started with the highest year

Scenario: An Edition contains the datetime in local time
Given All data scripts
When the feature is executed
Then the Start- and EndDateTime is in local time

Scenario: The first editions started at the start of boxing day (CET)
Given All data scripts
When the feature is executed
Then the UTC Start date is as follow:
| Year | UTC Start date      |
| 1999 | 1999-12-25T23:00:00 |
| 2001 | 2001-12-25T23:00:00 |
| 2002 | 2002-12-25T23:00:00 |
| 2003 | 2003-12-25T23:00:00 |
| 2004 | 2004-12-25T23:00:00 |
| 2005 | 2005-12-25T23:00:00 |
| 2006 | 2006-12-25T23:00:00 |
| 2007 | 2007-12-25T23:00:00 |
| 2008 | 2008-12-25T23:00:00 |

Scenario: As from 2009 the Top2000 starts at first christmas day at noon (CET)
Given All data scripts
When the feature is executed
Then the UTC Start date is as follow:
| Year | UTC Start date       |
| 2009 | 2009-12-25T11:00:00 |
| 2010 | 2010-12-25T11:00:00 |
| 2011 | 2011-12-25T11:00:00 |
| 2012 | 2012-12-25T11:00:00 |
| 2013 | 2013-12-25T11:00:00 |
| 2014 | 2014-12-25T11:00:00 |

Scenario: As from 2015 the Top2000 starts three hours earlier
Given All data scripts
When the feature is executed
Then the UTC Start date is as follow:
| Year | UTC Start date       |
| 2015 | 2015-12-25T08:00:00 |
| 2016 | 2016-12-25T08:00:00 |
| 2017 | 2017-12-25T08:00:00 |
| 2018 | 2018-12-25T08:00:00 |

Scenario: In 2019 the Top2000 start at 08:00
Given All data scripts
When the feature is executed
Then the UTC Start date is as follow:
| Year | UTC Start date      |
| 2019 | 2019-12-25T07:00:00 |

Scenario: As from 2020 the Top2000 starts at the start of boxing day (CET)
Given All data scripts
When the feature is executed
Then the UTC Start date is as follow:
| Year | UTC Start date      |
| 2020 | 2020-12-24T23:00:00 |
| 2021 | 2021-12-24T23:00:00 |
| 2022 | 2022-12-24T23:00:00 |
| 2024 | 2024-12-24T23:00:00 |

Scenario: The List of 2023 started earlier than usual because it was the 25 edition
Given All data scripts
When the feature is executed
Then the UTC Start date is as follow:
| Year | UTC Start date      |
| 2023 | 2023-12-11T09:00:00 |