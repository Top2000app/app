INSERT INTO Track ([Id] ,[Title] ,[Artist], [RecordedYear]) VALUES 
(5043,'Run Like Hell','Pink Floyd', 1979);

UPDATE Listing SET TrackId = 5043 WHERE Edition = 2024 AND Position = 1985;
UPDATE Listing SET TrackId = 5043 WHERE Edition = 2025 AND Position = 2000;
