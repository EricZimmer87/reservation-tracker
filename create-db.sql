CREATE DATABASE IF NOT EXISTS reservation_tracker;
USE reservation_tracker;

-- USERS TABLE
CREATE TABLE users (
    user_id BIGSERIAL PRIMARY KEY,
    google_id VARCHAR(255) UNIQUE NULL,
    email VARCHAR(100) NOT NULL UNIQUE,
    display_name VARCHAR(100) NULL,
    picture TEXT NULL,
    is_admin BOOLEAN DEFAULT FALSE,
    is_banned BOOLEAN DEFAULT FALSE
);

-- GUESTS TABLE
CREATE TABLE guests (
    guest_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    phone_number VARCHAR(20) NOT NULL,
    address TEXT NOT NULL,
    email VARCHAR(100),
    notes TEXT,
    company VARCHAR(100)
);

-- ROOMS TABLE
CREATE TABLE rooms (
    room_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    room_number VARCHAR(10) NOT NULL UNIQUE,
    room_type VARCHAR(50) NOT NULL,
    notes TEXT
);

-- RESERVATIONS TABLE
CREATE TABLE reservations (
    reservation_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    guest_id BIGINT,
    user_id BIGINT,
    room_id BIGINT NOT NULL,
    date_reserved TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    check_in_date DATE NOT NULL,
    check_out_date DATE NOT NULL,
    number_of_guests INT,
    notes TEXT,
    status VARCHAR(20) NOT NULL CHECK (status IN ('booked', 'checked_in', 'canceled', 'blocked', 'past')),
    card_last_four VARCHAR(4),

    -- Foreign keys
    CONSTRAINT fk_guest FOREIGN KEY (guest_id) REFERENCES guests(guest_id) ON DELETE CASCADE,
    CONSTRAINT fk_user FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE SET NULL,
    CONSTRAINT fk_room FOREIGN KEY (room_id) REFERENCES rooms(room_id) ON DELETE RESTRICT
);

-- INSERT SAMPLE DATA
-- USERS
INSERT INTO users (email, is_admin)
VALUES ('ericzimmer87@gmail.com', TRUE);

-- ROOMS
-- rooms 101–109 (double queen)
INSERT INTO rooms (room_number, room_type)
VALUES
('101', 'double_queen'),
('102', 'double_queen'),
('103', 'double_queen'),
('104', 'double_queen'),
('105', 'double_queen'),
('106', 'double_queen'),
('107', 'double_queen'),
('108', 'double_queen'),
('109', 'double_queen');

-- room 110: Single Queen Handicap
INSERT INTO rooms (room_number, room_type)
VALUES ('110', 'single_queen_handicap');

-- rooms 111–114 (double queen)
INSERT INTO rooms (room_number, room_type)
VALUES
('111', 'double_queen'),
('112', 'double_queen'),
('113', 'double_queen'),
('114', 'double_queen');

-- rooms 115 & 116: King Suites
INSERT INTO rooms (room_number, room_type)
VALUES
('115', 'king_suite'),
('116', 'king_suite');

---

-- GUESTS
INSERT INTO guests (first_name, last_name, phone_number, address, email, notes, company)
VALUES
('John', 'Smith', '402-555-1234', '123 Main St, Lincoln, NE 68508', 'john.smith@example.com', 'Prefers 1st floor rooms', NULL),
('Jane', 'Doe', '308-555-7890', '456 Elm St, Kearney, NE 68845', 'jane.doe@example.com', 'Late check-in around 11 PM', NULL),
('Michael', 'Brown', '531-555-4444', '789 Maple Ave, Grand Island, NE 68801', NULL, 'Allergic to pets', 'Brown Logistics'),
('Emily', 'Johnson', '402-555-8888', '135 Oak St, Columbus, NE 68601', 'emilyj@techmail.com', NULL, NULL),
('Sara', 'Nguyen', '308-555-9876', '980 River Rd, Scottsbluff, NE 69361', NULL, 'Always pays cash', 'Sara Realty'),
('David', 'Lee', '531-555-2233', '22 Pine St, Norfolk, NE 68701', 'dlee@example.com', NULL, 'Lee Construction'),
('Karen', 'Thompson', '402-555-0000', '742 Willow Dr, Hastings, NE 68901', NULL, 'Talks a lot', NULL);
