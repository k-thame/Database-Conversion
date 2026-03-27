-- ============================================================
--  SmolTech Shopping App — SQLite Database Schema
--  Converted from SQL Server (ThameJ25Su233x schema)
--  Branch: feature/sqlite-migration
--  GitHub Issue: "Convert SQL Server DB to SQLite"
-- ============================================================
--  SQL Server → SQLite translation notes:
--    IDENTITY(1,1)     → INTEGER PRIMARY KEY AUTOINCREMENT
--    BIT               → INTEGER  (0 = false, 1 = true)
--    NVARCHAR / VARCHAR → TEXT
--    DECIMAL(p,s)      → REAL
--    VARBINARY(MAX)    → BLOB
--    DATETIME/DATE     → TEXT  (ISO-8601: 'YYYY-MM-DD HH:MM:SS')
--    GETDATE()         → datetime('now')
--    Schema prefix     → removed (SQLite has no schema namespaces)
-- ============================================================

PRAGMA foreign_keys = ON;
PRAGMA journal_mode = WAL;

-- ------------------------------------------------------------
-- 1. Position  (lookup table for roles)
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Position (
    PositionID    INTEGER PRIMARY KEY AUTOINCREMENT,
    PositionTitle TEXT    NOT NULL UNIQUE
);

-- Seed standard roles so the app works out of the box
INSERT OR IGNORE INTO Position (PositionTitle) VALUES ('Customer');
INSERT OR IGNORE INTO Position (PositionTitle) VALUES ('Manager');

-- ------------------------------------------------------------
-- 2. Person  (core user record)
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Person (
    PersonID        INTEGER PRIMARY KEY AUTOINCREMENT,
    Title           TEXT,
    NameFirst       TEXT,
    NameMiddle      TEXT,
    NameLast        TEXT,
    Suffix          TEXT,
    Address1        TEXT,
    Address2        TEXT,
    Address3        TEXT,
    City            TEXT,
    Zipcode         TEXT,
    State           TEXT,
    Email           TEXT,
    PhonePrimary    TEXT,
    PhoneSecondary  TEXT,
    PositionID      INTEGER REFERENCES Position(PositionID)
);

-- ------------------------------------------------------------
-- 3. SecurityQuestions  (question bank, grouped by SetID)
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS SecurityQuestions (
    QuestionID      INTEGER PRIMARY KEY AUTOINCREMENT,
    SetID           INTEGER NOT NULL,   -- 1, 2, or 3
    QuestionPrompt  TEXT    NOT NULL
);

-- Seed default question sets (mirrors typical remote data)
INSERT OR IGNORE INTO SecurityQuestions (SetID, QuestionPrompt) VALUES
    (1, 'What was the name of your first pet?'),
    (1, 'What is your mother''s maiden name?'),
    (1, 'What city were you born in?'),
    (2, 'What was the name of your elementary school?'),
    (2, 'What is your oldest sibling''s middle name?'),
    (2, 'What was the make of your first car?'),
    (3, 'What is your favorite movie?'),
    (3, 'What was your childhood nickname?'),
    (3, 'What street did you grow up on?');

-- ------------------------------------------------------------
-- 4. Logon  (credentials + security challenge answers)
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Logon (
    LogonID                 INTEGER PRIMARY KEY AUTOINCREMENT,
    PersonID                INTEGER NOT NULL REFERENCES Person(PersonID),
    LogonName               TEXT    NOT NULL UNIQUE,
    Password                TEXT    NOT NULL,
    PositionTitle           TEXT,
    AccountDisabled         INTEGER NOT NULL DEFAULT 0,  -- BIT
    AccountDeleted          INTEGER NOT NULL DEFAULT 0,  -- BIT
    FirstChallengeQuestion  INTEGER REFERENCES SecurityQuestions(QuestionID),
    FirstChallengeAnswer    TEXT,
    SecondChallengeQuestion INTEGER REFERENCES SecurityQuestions(QuestionID),
    SecondChallengeAnswer   TEXT,
    ThirdChallengeQuestion  INTEGER REFERENCES SecurityQuestions(QuestionID),
    ThirdChallengeAnswer    TEXT
);

-- ------------------------------------------------------------
-- 5. Categories  (product categories)
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Categories (
    CategoryID   INTEGER PRIMARY KEY AUTOINCREMENT,
    CategoryName TEXT    NOT NULL
);

-- ------------------------------------------------------------
-- 6. Inventory  (product catalogue)
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Inventory (
    InventoryID      INTEGER PRIMARY KEY AUTOINCREMENT,
    ItemName         TEXT    NOT NULL,
    ItemDescription  TEXT,
    CategoryID       INTEGER REFERENCES Categories(CategoryID),
    RetailPrice      REAL    NOT NULL DEFAULT 0.00,
    Cost             REAL    NOT NULL DEFAULT 0.00,
    Quantity         INTEGER NOT NULL DEFAULT 0,
    RestockThreshold INTEGER NOT NULL DEFAULT 0,
    ItemImage        BLOB,
    Discontinued     INTEGER NOT NULL DEFAULT 0   -- BIT
);

-- ------------------------------------------------------------
-- 7. Discounts
--    DiscountLevel : 0 = Item-level, 1 = Cart-level
--    DiscountType  : 0 = Percentage, 1 = Dollar amount
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Discounts (
    DiscountID          INTEGER PRIMARY KEY AUTOINCREMENT,
    DiscountCode        TEXT    NOT NULL UNIQUE,
    Description         TEXT,
    DiscountLevel       INTEGER NOT NULL DEFAULT 0,
    DiscountType        INTEGER NOT NULL DEFAULT 0,
    DiscountPercentage  REAL,
    DiscountDollarAmount REAL,
    InventoryID         INTEGER REFERENCES Inventory(InventoryID),
    StartDate           TEXT    NOT NULL,   -- 'YYYY-MM-DD'
    ExpirationDate      TEXT    NOT NULL    -- 'YYYY-MM-DD'
);

-- ------------------------------------------------------------
-- 8. Orders  (one row per transaction)
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Orders (
    OrderID    INTEGER PRIMARY KEY AUTOINCREMENT,
    PersonID   INTEGER NOT NULL REFERENCES Person(PersonID),
    EmployeeID INTEGER REFERENCES Person(PersonID),   -- manager-assisted POS
    OrderDate  TEXT    NOT NULL DEFAULT (datetime('now')),
    CC_Number  TEXT,
    ExpDate    TEXT,
    CCV        TEXT
);

-- ------------------------------------------------------------
-- 9. OrderDetails  (line items for each order)
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS OrderDetails (
    OrderDetailID INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderID       INTEGER NOT NULL REFERENCES Orders(OrderID),
    InventoryID   INTEGER NOT NULL REFERENCES Inventory(InventoryID),
    Quantity      INTEGER NOT NULL DEFAULT 1,
    DiscountID    INTEGER REFERENCES Discounts(DiscountID)
);

-- ============================================================
--  End of schema
-- ============================================================
