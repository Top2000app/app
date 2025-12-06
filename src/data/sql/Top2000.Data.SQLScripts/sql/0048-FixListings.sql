-- Merge duplicate listing: Biko - Peter Gabriel.
UPDATE Listing 
SET TrackId = 347 
WHERE TrackId = 3916;

DELETE FROM Track WHERE Id = 3916;

-- School Supertramp is in the wrong play time.
UPDATE Listing 
SET [PlayUtcDateAndTime] = '2020-12-31T17:00:00' 
WHERE TrackId = 2601 AND Edition = 2020;

-- Het nummer "More than a woman" in de Top2000 lijst van 2019 en 2020 is van de Bee Gees, niet van Tavares.
INSERT INTO Track(Id, Title, Artist, RecordedYear) VALUES (4588, 'More than a woman', 'Bee Gees', 1977);

UPDATE Listing 
SET TrackId = 4588 
WHERE TrackId = 2081 AND Edition = 2019;

UPDATE Listing 
SET TrackId = 4588 
WHERE TrackId = 2081 AND Edition = 2020;
