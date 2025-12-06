-- Add the search columns
ALTER TABLE Track ADD SearchTitle NVARCHAR(100);
ALTER TABLE Track ADD SearchArtist NVARCHAR(100);