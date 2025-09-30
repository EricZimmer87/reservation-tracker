IF DB_ID('ReservationTracker') IS NULL
    CREATE DATABASE ReservationTracker;
GO
USE ReservationTracker;
GO

-- USERS TABLE
CREATE TABLE Users (
    UserId BIGINT IDENTITY(1,1) PRIMARY KEY,
    GoogleId VARCHAR(255) UNIQUE NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    DisplayName VARCHAR(100) NULL,
    Picture VARCHAR(MAX) NULL,
    IsAdmin BIT DEFAULT 0,
    IsBanned BIT DEFAULT 0
);

-- GUESTS TABLE
CREATE TABLE Guests (
    GuestId BIGINT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    PhoneNumber VARCHAR(20) NOT NULL,
    Address VARCHAR(MAX) NOT NULL,
    Email VARCHAR(100),
    Notes VARCHAR(MAX),
    Company VARCHAR(100)
);

-- ROOMS TABLE
CREATE TABLE Rooms (
    RoomId BIGINT IDENTITY(1,1) PRIMARY KEY,
    RoomNumber VARCHAR(10) NOT NULL UNIQUE,
    RoomType VARCHAR(50) NOT NULL,
    Notes VARCHAR(MAX)
);

-- RESERVATIONS TABLE
CREATE TABLE Reservations (
    ReservationId BIGINT IDENTITY(1,1) PRIMARY KEY,
    GuestId BIGINT,
    UserId BIGINT,
    RoomId BIGINT NOT NULL,
    DateReserved DATETIME2 DEFAULT SYSDATETIME(),
    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NOT NULL,
    NumberOfGuests INT NULL,
    Notes VARCHAR(MAX) NULL,
    Status VARCHAR(20) NOT NULL 
        CHECK (Status IN ('booked', 'checked_in', 'canceled', 'blocked', 'past')),
    CardLastFour VARCHAR(4) NULL,

    CONSTRAINT FK_Guest FOREIGN KEY (GuestId) REFERENCES Guests(GuestId) ON DELETE CASCADE,
    CONSTRAINT FK_User FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE SET NULL,
    CONSTRAINT FK_Room FOREIGN KEY (RoomId) REFERENCES Rooms(RoomId) ON DELETE NO ACTION
);

-- USERS
INSERT INTO Users (Email, IsAdmin)
VALUES ('ericzimmer87@gmail.com', 1);

-- ROOMS DATA
INSERT INTO Rooms (RoomNumber, RoomType) VALUES
('101', 'double_queen'),
('102', 'double_queen'),
('103', 'double_queen'),
('104', 'double_queen'),
('105', 'double_queen'),
('106', 'double_queen'),
('107', 'double_queen'),
('108', 'double_queen'),
('109', 'double_queen'),
('110', 'single_queen_handicap'),
('111', 'double_queen'),
('112', 'double_queen'),
('113', 'double_queen'),
('114', 'double_queen'),
('115', 'king_suite'),
('116', 'king_suite');

-- GUESTS DATA
INSERT INTO Guests (FirstName, LastName, PhoneNumber, Address, Email, Notes, Company) VALUES
('John', 'Smith', '402-555-1234', '123 Main St, Lincoln, NE 68508', 'john.smith@example.com', 'Prefers 1st floor rooms', NULL),
('Jane', 'Doe', '308-555-7890', '456 Elm St, Kearney, NE 68845', 'jane.doe@example.com', 'Late check-in around 11 PM', NULL),
('Michael', 'Brown', '531-555-4444', '789 Maple Ave, Grand Island, NE 68801', NULL, 'Allergic to pets', 'Brown Logistics'),
('Emily', 'Johnson', '402-555-8888', '135 Oak St, Columbus, NE 68601', 'emilyj@techmail.com', NULL, NULL),
('Sara', 'Nguyen', '308-555-9876', '980 River Rd, Scottsbluff, NE 69361', NULL, 'Always pays cash', 'Sara Realty'),
('David', 'Lee', '531-555-2233', '22 Pine St, Norfolk, NE 68701', 'dlee@example.com', NULL, 'Lee Construction'),
('Karen', 'Thompson', '402-555-0000', '742 Willow Dr, Hastings, NE 68901', NULL, 'Talks a lot', NULL);
