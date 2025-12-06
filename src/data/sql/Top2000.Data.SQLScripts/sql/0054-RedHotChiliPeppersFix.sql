-- Otherside van de Red Hot Chili Peppers staat 2x los in de lijst
UPDATE Listing SET TrackId = 3757 WHERE TrackId = 4025;
DELETE FROM Track WHERE Id = 4025;