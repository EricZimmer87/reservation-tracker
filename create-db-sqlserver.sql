IF DB_ID('ReservationTracker') IS NULL
    CREATE DATABASE ReservationTracker;
GO

USE ReservationTracker;
GO

-- USERS TABLE
CREATE TABLE Users (
    UserId       BIGINT IDENTITY(1,1) PRIMARY KEY,
    GoogleId     VARCHAR(255) UNIQUE NULL,
    Email        VARCHAR(100) NOT NULL UNIQUE,
    DisplayName  VARCHAR(100) NULL,
    Picture      VARCHAR(MAX) NULL,
    IsAdmin      BIT NOT NULL DEFAULT 0,
    IsBanned     BIT NOT NULL DEFAULT 0
);

-- GUESTS TABLE
CREATE TABLE Guests (
    GuestId      BIGINT IDENTITY(1,1) PRIMARY KEY,
    FirstName    VARCHAR(50)  NOT NULL,
    LastName     VARCHAR(50)  NOT NULL,
    PhoneNumber  VARCHAR(20)  NOT NULL,
    Address      VARCHAR(MAX) NOT NULL,
    City         VARCHAR(100) NOT NULL,
    State        CHAR(2)      NOT NULL,
    Zipcode      VARCHAR(10)  NOT NULL,
    Email        VARCHAR(100) NULL,
    Notes        VARCHAR(MAX) NULL,
    Company      VARCHAR(100) NULL
);

-- ROOMS TABLE
CREATE TABLE Rooms (
    RoomId     BIGINT IDENTITY(1,1) PRIMARY KEY,
    RoomNumber VARCHAR(10) NOT NULL UNIQUE,
    RoomType   VARCHAR(50) NOT NULL,
    Notes      VARCHAR(MAX)
);

-- RESERVATIONS TABLE
CREATE TABLE Reservations (
    ReservationId   BIGINT IDENTITY(1,1) PRIMARY KEY,
    GuestId         BIGINT NULL,
    UserId          BIGINT NULL,
    RoomId          BIGINT NOT NULL,
    DateReserved    DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CheckInDate     DATE NOT NULL,
    CheckOutDate    DATE NOT NULL,
    NumberOfGuests  INT NULL,
    Notes           VARCHAR(MAX) NULL,
    Status          VARCHAR(20) NOT NULL 
        CHECK (Status IN ('booked', 'checked_in', 'canceled', 'blocked', 'past')),
    CardLastFour    VARCHAR(4) NULL,

    CONSTRAINT FK_Guest FOREIGN KEY (GuestId) REFERENCES Guests(GuestId) ON DELETE CASCADE,
    CONSTRAINT FK_User  FOREIGN KEY (UserId)  REFERENCES Users(UserId)  ON DELETE SET NULL,
    CONSTRAINT FK_Room  FOREIGN KEY (RoomId)  REFERENCES Rooms(RoomId)  ON DELETE NO ACTION
);

-- USERS
-- INSERT INTO Users (Email, IsAdmin)
-- VALUES ('ericzimmer87@gmail.com', 1);

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
INSERT INTO Guests (FirstName, LastName, PhoneNumber, Address, City, State, Zipcode, Email, Notes, Company) VALUES
('John',   'Smith',    '402-555-1234', '123 Main St',   'Lincoln',     'NE', '68508', 'john.smith@example.com', 'Prefers 1st floor rooms', NULL),
('Jane',   'Doe',      '308-555-7890', '456 Elm St',    'Kearney',     'NE', '68845', 'jane.doe@example.com',   'Late check-in around 11 PM', NULL),
('Michael','Brown',    '531-555-4444', '789 Maple Ave', 'Grand Island','NE', '68801', NULL,                     'Allergic to pets', 'Brown Logistics'),
('Emily',  'Johnson',  '402-555-8888', '135 Oak St',    'Columbus',    'NE', '68601', 'emilyj@techmail.com',    NULL, NULL),
('Sara',   'Nguyen',   '308-555-9876', '980 River Rd',  'Scottsbluff', 'NE', '69361', NULL,                     'Always pays cash', 'Sara Realty'),
('David',  'Lee',      '531-555-2233', '22 Pine St',    'Norfolk',     'NE', '68701', 'dlee@example.com',       NULL, 'Lee Construction'),
('Karen',  'Thompson', '402-555-0000', '742 Willow Dr', 'Hastings',    'NE', '68901', NULL,                     'Talks a lot', NULL);

-- RESERVATIONS DATA
INSERT INTO Reservations
    (GuestId, UserId, RoomId, CheckInDate, CheckOutDate, NumberOfGuests, Notes, Status, CardLastFour)
VALUES
-- Feb 15–17
(1, NULL, 1,  '2025-02-15', '2025-02-17', 2, 'Requested quiet room', 'past', '1234'),

-- Feb 16–18
(2, NULL, 2,  '2025-02-16', '2025-02-18', 1, 'Late arrival confirmed', 'past', '5678'),

-- Feb 18–20
(3, NULL, 15, '2025-02-18', '2025-02-20', 2, 'Business stay', 'past', '9012'),

-- Feb 20–22
(4, NULL, 3,  '2025-02-20', '2025-02-22', 2, NULL, 'past', '3456'),

-- Feb 22–25
(5, NULL, 16, '2025-02-22', '2025-02-25', 3, 'Prefers top floor', 'past', NULL),

-- Feb 26–28
(6, NULL, 4,  '2025-02-26', '2025-02-28', 1, NULL, 'checked_in', '7788'),

-- Feb 27–Mar 1
(7, NULL, 5,  '2025-02-27', '2025-03-01', 2, 'Extended stay possible', 'checked_in', '9900'),

-- Mar 2–4
(1, NULL, 6,  '2025-03-02', '2025-03-04', 2, NULL, 'booked', '1122'),

-- Mar 3–6
(2, NULL, 7,  '2025-03-03', '2025-03-06', 1, 'Early check-in requested', 'booked', '3344'),

-- Mar 5–7
(3, NULL, 14, '2025-03-05', '2025-03-07', 2, 'Pet allergy noted', 'booked', '5566'),

-- Mar 8–10 (Canceled)
(4, NULL, 8,  '2025-03-08', '2025-03-10', 2, 'Canceled due to weather', 'canceled', NULL),

-- Mar 10–12
(5, NULL, 9,  '2025-03-10', '2025-03-12', 1, NULL, 'booked', '7788'),

-- Mar 12–15
(6, NULL, 10, '2025-03-12', '2025-03-15', 1, 'Handicap-accessible room', 'booked', '4455'),

-- Mar 15–18
(7, NULL, 11, '2025-03-15', '2025-03-18', 2, NULL, 'booked', '6677');
