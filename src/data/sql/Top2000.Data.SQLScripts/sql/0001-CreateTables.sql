CREATE TABLE Track
(
    Id INT NOT NULL,
    Title NVARCHAR(100) NOT NULL,
    Artist NVARCHAR(100) NOT NULL,
    RecordedYear INT NOT NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE Edition
(
    Year int NOT NULL,
    StartUtcDateAndTime DATETIME NOT NULL,
    EndUtcDateAndTime DATETIME NOT NULL,
    HasPlayDateAndTime BIT NOT NULL,
    PRIMARY KEY (Year)
);

CREATE TABLE Listing
(
    TrackId int NOT NULL,
    Edition int NOT NULL,
    Position int NOT NULL,
    PlayUtcDateAndTime DATETIME NULL,
    PRIMARY KEY (TrackId, Edition),
    FOREIGN KEY (Edition) REFERENCES Edition(Year)
)