INSERT INTO [Edition] ([Year], [StartUtcDateAndTime], [EndUtcDateAndTime], [HasPlayDateAndTime]) VALUES
(2024, '2024-12-24T23:00:00', '2024-12-31T23:00:00', 1);

INSERT INTO Track ([Id] ,[Title] ,[Artist], [RecordedYear]) VALUES
(4932,'Better Days','Dermot Kennedy', 2021),
(4933,'Europapa','Joost',2024),
(4934,'Die With A Smile', 'Lady Gaga & Bruno Mars', 2024),
(4935,'Terug In De Tijd','Yves Berendse',2023),
(4936,'Waterdicht','Hannah Mae',2023),
(4937,'The Emptiness Machine','Linkin Park',2024),
(4938,'The Door','Teddy Swims',2023),
(4939,'BIRDS OF A FEATHER','Billie Eilish',2024),
(4940,'Als Ik God Was','Froukje',2023),
(4941,'HIND''S HALL','MACKLEMORE',2024),
(4942,'Silver Springs','Fleetwood Mac',1976),
(4943,'Hells Bells','AC/DC',1980),
(4944,'One Step Closer','Linkin Park',2000),
(4945,'Karma','Taylor Swift',2023),
(4946,'Heaven','Niall Horan',2023),
(4947,'Vampire','Olivia Rodrigo',2023),
(4948,'Jigsaw Falling Into Place','Radiohead',2008),
(4949,'Chump Change','Quincy Jones',1973),
(4950,'OMG It''s Happening','DI-RECT',2023),
(4951,'Sugardaddy','Roxy Dekker',2024),
(4952,'This World Is Our Home','Douwe Bob',2024),
(4953,'Welcome To The DCC','Nothing But Thieves',2023),
(4954,'Vas-y (Ga Maar)','Suzan & Freek & Claude',2023),
(4955,'Coney Island','Taylor Swift ft. The National',2021),
(4956,'Bad Blood (Taylor''s Version)','Taylor Swift',2023),
(4957,'Espresso','Sabrina Carpenter',2024),
(4958,'Beautiful Things','Benson Boone',2024),
(4959,'Good Luck, Babe!','Chappell Roan',2024),
(4960,'Murder On The Dancefloor','Sophie Ellis Bextor',2001),
(4961,'Radio','Lana Del Rey',2012),
(4962,'Candy','Paolo Nutini',2009),
(4963,'Houdini','Dua Lipa',2024),
(4964,'On Melancholy Hill','Gorillaz',2010),
(4965,'Just Like Heaven','The Cure',1987),
(4966,'White Horse','Chris Stapleton',2023),
(4967,'Unstoppable','Sia',2016),
(4968,'Speed Of Light','Chef''Special',2023),
(4969,'Night Changes','One Direction',2014),
(4970,'Too Sweet','Hozier',2024),
(4971,'Wish You The Best','Lewis Capaldi',2023),
(4972,'Fast Car','Luke Combs',2023),
(4973,'Chemical','Post Malone',2023),
(4974,'Head Held High','Sera',2023);

UPDATE Track SET Title = 'Ob-La-Di, Ob-La-Da' WHERE Id = 2229;
UPDATE Track SET Title = 'Ob-La-Di, Ob-La-Da' WHERE Id = 2230;
UPDATE Track SET RecordedYear = 2022 WHERE Id = 4831;
UPDATE Track SET Artist = 'Daryl Hall & John Oates' WHERE Artist = 'Hall & Oates';
UPDATE Track SET Artist = 'U.S.A. For Africa' WHERE Id = 3382;
UPDATE Track SET Artist = 'Prince & The New Power Generation' WHERE Id = 4229;