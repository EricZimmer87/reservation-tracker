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
    NormalizedPhoneNumber VARCHAR(20) NOT NULL,
    Address      VARCHAR(MAX) NOT NULL,
    City         VARCHAR(100) NOT NULL,
    State        CHAR(2)      NOT NULL,
    Zipcode      VARCHAR(10)  NOT NULL,
    Email        VARCHAR(100) NULL,
    Notes        VARCHAR(MAX) NULL,
    Company      VARCHAR(100) NULL,

    CONSTRAINT UQ_Guests_FirstName_LastName_NormalizedPhoneNumber
        UNIQUE (FirstName, LastName, NormalizedPhoneNumber)
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
    ReservationId     BIGINT IDENTITY(1,1) PRIMARY KEY,
    GuestId           BIGINT NULL,
    UserId            BIGINT NULL, -- who created the reservation
    ModifiedByUserId  BIGINT NULL, -- who last modified the reservation
    RoomId            BIGINT NOT NULL,
    DateReserved      DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    ModifiedOn        DATETIME2 NULL,
    CanceledOn        DATETIME2 NULL,
    CheckInDate       DATE NOT NULL,
    CheckOutDate      DATE NOT NULL,
    NumberOfGuests    INT NULL,
    Notes             VARCHAR(MAX) NULL,
    Status            VARCHAR(20) NOT NULL
        CHECK (Status IN ('booked', 'checked_in', 'canceled', 'blocked', 'past')),
    CardLastFour      VARCHAR(4) NULL,

    CONSTRAINT FK_Guest FOREIGN KEY (GuestId)
        REFERENCES Guests(GuestId) ON DELETE NO ACTION,

    CONSTRAINT FK_User FOREIGN KEY (UserId)
        REFERENCES Users(UserId) ON DELETE NO ACTION,

    CONSTRAINT FK_ModifiedByUser FOREIGN KEY (ModifiedByUserId)
        REFERENCES Users(UserId) ON DELETE NO ACTION,

    CONSTRAINT FK_Room FOREIGN KEY (RoomId)
        REFERENCES Rooms(RoomId) ON DELETE NO ACTION
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
INSERT INTO Guests
    (FirstName, LastName, PhoneNumber, NormalizedPhoneNumber, Address, City, State, Zipcode, Email, Notes, Company)
VALUES
('John',   'Smith',    '402-555-1234', '4025551234', '123 Main St',   'Lincoln',      'NE', '68508', 'john.smith@example.com', 'Prefers 1st floor rooms', NULL),
('Jane',   'Doe',      '308-555-7890', '3085557890', '456 Elm St',    'Kearney',      'NE', '68845', 'jane.doe@example.com',   'Late check-in around 11 PM', NULL),
('Michael','Brown',    '531-555-4444', '5315554444', '789 Maple Ave', 'Grand Island', 'NE', '68801', NULL,                     'Allergic to pets', 'Brown Logistics'),
('Emily',  'Johnson',  '402-555-8888', '4025558888', '135 Oak St',    'Columbus',     'NE', '68601', 'emilyj@techmail.com',    NULL, NULL),
('Sara',   'Nguyen',   '308-555-9876', '3085559876', '980 River Rd',  'Scottsbluff',  'NE', '69361', NULL,                     'Always pays cash', 'Sara Realty'),
('David',  'Lee',      '531-555-2233', '5315552233', '22 Pine St',    'Norfolk',      'NE', '68701', 'dlee@example.com',       NULL, 'Lee Construction'),
('Karen',  'Thompson', '402-555-0000', '4025550000', '742 Willow Dr', 'Hastings',     'NE', '68901', NULL,                     'Talks a lot', NULL);

-- RESERVATIONS DATA
INSERT INTO Reservations
    (GuestId, UserId, RoomId, CheckInDate, CheckOutDate, NumberOfGuests, Notes, Status, CardLastFour, CanceledOn)
VALUES
-- Mar 15–17
(1, NULL, 1,  '2026-03-15', '2026-03-17', 2, 'Requested quiet room', 'past', '1234', NULL),

-- Mar 16–18
(2, NULL, 2,  '2026-03-16', '2026-03-18', 1, 'Late arrival confirmed', 'past', '5678', NULL),

-- Mar 18–20
(3, NULL, 15, '2026-03-18', '2026-03-20', 2, 'Business stay', 'past', '9012', NULL),

-- Mar 20–22
(4, NULL, 3,  '2026-03-20', '2026-03-22', 2, NULL, 'past', '3456', NULL),

-- Mar 22–25
(5, NULL, 16, '2026-03-22', '2026-03-25', 3, 'Prefers top floor', 'past', NULL, NULL),

-- Mar 26–28
(6, NULL, 4,  '2026-03-26', '2026-03-28', 1, NULL, 'checked_in', '7788', NULL),

-- Mar 27–Apr 1
(7, NULL, 5,  '2026-03-27', '2026-04-01', 2, 'Extended stay possible', 'checked_in', '9900', NULL),

-- Apr 2–4
(1, NULL, 6,  '2026-04-02', '2026-04-04', 2, NULL, 'booked', '1122', NULL),

-- Apr 3–6
(2, NULL, 7,  '2026-04-03', '2026-04-06', 1, 'Early check-in requested', 'booked', '3344', NULL),

-- Apr 5–7
(3, NULL, 14, '2026-04-05', '2026-04-07', 2, 'Pet allergy noted', 'booked', '5566', NULL),

-- Apr 8–10 (Canceled)
(4, NULL, 8,  '2026-04-08', '2026-04-10', 2, 'Canceled due to weather', 'canceled', NULL, SYSDATETIME()),

-- Apr 10–12
(5, NULL, 9,  '2026-04-10', '2026-04-12', 1, NULL, 'booked', '7788', NULL),

-- Apr 12–15
(6, NULL, 10, '2026-04-12', '2026-04-15', 1, 'Handicap-accessible room', 'booked', '4455', NULL),

-- Apr 15–18
(7, NULL, 11, '2026-04-15', '2026-04-18', 2, NULL, 'booked', '6677', NULL);
GO