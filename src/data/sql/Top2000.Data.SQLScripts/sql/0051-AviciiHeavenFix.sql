INSERT INTO Track ([Id] ,[Title] ,[Artist],[RecordedYear]) VALUES 
(4652, 'Heaven','Avicii ft. Chris Martin', 2019);

DELETE FROM Listing WHERE Edition = 2021 AND Position = 1803;
DELETE FROM Listing WHERE Edition = 2020 AND Position = 1383;

INSERT INTO [Listing] ([TrackId] ,[Edition] ,[Position], [PlayUtcDateAndTime]) VALUES
(4652, 2020, 1383, '2020-12-26T23:00:00'),
(4652, 2021, 1803, '2021-12-25T14:00:00');