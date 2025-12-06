-- Fixing Empire State Of Mind Alicia Keys
UPDATE Listing SET TrackId = 822 WHERE TrackId = 3947;
DELETE FROM Track WHERE Id = 3947;

-- Fixing Enola Gay O.M.D.
UPDATE Listing SET TrackId = 4281 WHERE TrackId = 826;
DELETE FROM Track WHERE Id = 826;

-- Fixing Ghost Love Score Nightwish
UPDATE Listing SET TrackId = 4529 WHERE TrackId = 4480;
DELETE FROM Track WHERE Id = 4480;

-- Fixing I close my eyes and count to ten Dusty Springfield
UPDATE Listing SET TrackId = 1289 WHERE TrackId = 4132;
DELETE FROM Track WHERE Id = 4132;

-- Fixing I've Just Lost Somebody Golden Earrings
UPDATE Listing SET TrackId = 1586 WHERE TrackId = 4452;
DELETE FROM Track WHERE Id = 4452;

--Fixing Killing In The Name Rage Against The Machine
UPDATE Listing SET TrackId = 1665 WHERE TrackId = 4258;
DELETE FROM Track WHERE Id = 4258;

-- Fixing Live and let Die
UPDATE Listing SET TrackId = 4304 WHERE TrackId = 1842;
DELETE FROM Track WHERE Id = 1842;

-- Fixing Lola Montez Volbea
UPDATE Listing SET TrackId = 4341 WHERE TrackId = 3999;
DELETE FROM Track WHERE Id = 3999;

-- Fixing Magnificent U2
UPDATE Listing SET TrackId = 1954 WHERE TrackId = 4177;
DELETE FROM Track WHERE Id = 4177;

-- Fixing On The Boarder Al Stewart
UPDATE Listing SET TrackId = 2253 WHERE TrackId = 4172;
DELETE FROM Track WHERE Id = 4172;

-- Fixing Rock 'N' Roll High School Ramones
UPDATE Listing SET TrackId = 2508 WHERE TrackId = 4147;
DELETE FROM Track WHERE Id = 4147;

-- Fixing Sound Of The Screameng Day Golden Earring
UPDATE Listing SET TrackId = 4150 WHERE TrackId = 2770;
DELETE FROM Track WHERE Id = 2770;

-- Fixing Think Aretha Franklin
UPDATE Listing SET TrackId = 4532 WHERE TrackId = 3137;
DELETE FROM Track WHERE Id = 3137;

-- Fixing Waiting On A Sunny Day Bruce Springsteen
UPDATE Listing SET TrackId = 4238 WHERE TrackId = 3341;
DELETE FROM Track WHERE Id = 3341;

-- Fixing listing for Both sides Now
UPDATE listing SET TrackId = 4310 WHERE Position = 1412 AND Edition = 2016;

-- Fixing Can't Help Falling In Love
UPDATE listing SET TrackId = 477 WHERE Position = 380 AND Edition = 2020;
UPDATE listing SET TrackId = 477 WHERE Position = 375 AND Edition = 2021;
UPDATE listing SET TrackId = 477 WHERE Position = 309 AND Edition = 2022;

-- Fixing Handle With Care
UPDATE Listing SET TrackId = 1083 WHERE Position = 1565 AND Edition = 2018;

-- Fixing Joan Of Arc (Maid Of Orleans)
UPDATE Listing SET TrackId = 1618 WHERE Position = 460 AND Edition = 2016;
UPDATE Listing SET TrackId = 1618 WHERE Position = 488 AND Edition = 2017;
UPDATE Listing SET TrackId = 1618 WHERE Position = 582 AND Edition = 2018;
UPDATE Listing SET TrackId = 1618 WHERE Position = 558 AND Edition = 2019;
UPDATE Listing SET TrackId = 1618 WHERE Position = 538 AND Edition = 2020;
UPDATE Listing SET TrackId = 1618 WHERE Position = 516 AND Edition = 2021;
UPDATE Listing SET TrackId = 1618 WHERE Position = 525 AND Edition = 2022;

-- Fixing Living Doll
UPDATE Listing SET TrackId = 1849 WHERE Position = 1661 AND Edition = 2017;

-- Fixing Mister Blue
UPDATE Listing SET TrackId = 2100 WHERE TrackId = 4415;
DELETE FROM Track Where Id = 4415;

-- Fixing Het Land Van
UPDATE Track SET Title = 'Het Land Van' WHERE Id = 3970;
UPDATE Listing SET TrackId = 3970 WHERE Position = 1667 AND Edition = 2020;

-- Fixing Sovereign Light Caf�
UPDATE Listing SET TrackId = 2772 WHERE Position = 726 AND Edition = 2020;
UPDATE Listing SET TrackId = 2772 WHERE Position = 767 AND Edition = 2021;
UPDATE Listing SET TrackId = 2772 WHERE Position = 791 AND Edition = 2022;

-- Fixing Edge Of Seventeen
INSERT INTO Track ([Id] ,[Title] ,[Artist],[RecordedYear]) VALUES (4737,'Edge Of Seventeen','Stevie Nicks', 1982);
UPDATE Listing SET TrackId = 4737 WHERE Position = 1938 AND Edition = 2021;
UPDATE Listing SET TrackId = 4737 WHERE Position = 1436 AND Edition = 2022;

-- Fixing The most beautiful girl 
UPDATE Listing SET TrackId = 3054 WHERE Position = 1268 AND Edition = 2016;

-- Fixing Knockin' On Heaven's Door
UPDATE Listing SET TrackId = -1688 WHERE Position = 1102 AND Edition = 2020;
UPDATE Listing SET TrackId = -1688 WHERE Position = 1131 AND Edition = 2021;
UPDATE Listing SET TrackId = -1688 WHERE Position = 1450 AND Edition = 2022;
UPDATE Listing SET TrackId = -1687 WHERE Position = 199 AND Edition = 2020;
UPDATE Listing SET TrackId = -1687 WHERE Position = 229 AND Edition = 2021;
UPDATE Listing SET TrackId = -1687 WHERE Position = 243 AND Edition = 2022;

UPDATE Listing SET TrackId = 1688 WHERE Position = 1102 AND Edition = 2020;
UPDATE Listing SET TrackId = 1688 WHERE Position = 1131 AND Edition = 2021;
UPDATE Listing SET TrackId = 1688 WHERE Position = 1450 AND Edition = 2022;
UPDATE Listing SET TrackId = 1687 WHERE Position = 199 AND Edition = 2020;
UPDATE Listing SET TrackId = 1687 WHERE Position = 229 AND Edition = 2021;
UPDATE Listing SET TrackId = 1687 WHERE Position = 243 AND Edition = 2022;

-- Fixing Intro / Sweet Jane
UPDATE Listing SET TrackId = 1525 WHERE Position = 1838 AND Edition = 2018;

-- Fixing Sweet Jane
DELETE FROM Track WHERE Id = 4250;

-- Fixing San Francisco
UPDATE Listing SET TrackId = 2578 WHERE Position = 1496 AND Edition = 2019;
UPDATE Listing SET TrackId = 2578 WHERE Position = 1504 AND Edition = 2020;
UPDATE Listing SET TrackId = 2578 WHERE Position = 1515 AND Edition = 2021;
UPDATE Listing SET TrackId = 2578 WHERE Position = 1717 AND Edition = 2022;

-- Fixing True Colors
UPDATE Listing SET TrackId = 3230 WHERE Position = 834 AND Edition = 2016;

-- Fixing You Really Got Me - Van Halen
INSERT INTO Track ([Id] ,[Title] ,[Artist],[RecordedYear]) VALUES (4738,'You Really Got Me','Van Halen', 1978);
UPDATE Listing SET TrackId = 4738 WHERE Position = 1874 AND Edition = 2020;

-- Fixing Move On Up
UPDATE Listing SET TRackId = 2095 WHERE Position = 1076 AND Edition = 2014;
UPDATE Listing SET TRackId = 2095 WHERE Position = 1242 AND Edition = 2015;

-- Fixing Rosie - Joan Armatrading
UPDATE Track SET Artist = 'Joan Armatrading' WHERE Id = 2534;

UPDATE Track SET RecordedYear = 1982 WHERE Id = 2534;

-- Fixing Rosie - Claw Boys Claw
INSERT INTO Track ([Id] ,[Title] ,[Artist],[RecordedYear]) VALUES (4739,'Rosie', 'Claw Boys Claw', 1980);
UPDATE Listing SET TrackId = 4739 WHERE Position = 1760 AND Edition = 2016;
UPDATE Listing SET TrackId = 4739 WHERE Position = 1451 AND Edition = 2017;
UPDATE Listing SET TrackId = 4739 WHERE Position = 1648 AND Edition = 2018;
UPDATE Listing SET TrackId = 4739 WHERE Position = 1616 AND Edition = 2019;
UPDATE Listing SET TrackId = 4739 WHERE Position = 1457 AND Edition = 2020;
UPDATE Listing SET TrackId = 4739 WHERE Position = 1267 AND Edition = 2021;
UPDATE Listing SET TrackId = 4739 WHERE Position = 1231 AND Edition = 2022;

-- Fixing The Man Who Sold The World
UPDATE Listing SET TrackId = 4241 WHERE Position = 1083 AND Edition = 2018;
UPDATE Listing SET TrackId = 4254 WHERE Position = 442 AND Edition = 2015;

UPDATE Track SET Artist = 'Acda en De Munnik' WHERE Id = 2473