namespace MyAPP.Driver;

public class DDL
{
    public static readonly string[] Defenition = {
        // Включение поддержки внешних ключей в SQLite
        "PRAGMA foreign_keys = ON;",
        @"CREATE TABLE IF NOT EXISTS Users (
        UserID INTEGER PRIMARY KEY AUTOINCREMENT,
        Email TEXT NOT NULL,
        PasswordHash TEXT NOT NULL,
        Role TEXT NOT NULL,
        IsActive INTEGER NOT NULL DEFAULT 1,
        CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
        UpdatedAt DATETIME
        );",

        @"CREATE UNIQUE INDEX IF NOT EXISTS UX_Users_Email
        ON Users (Email);",
        
        @"CREATE TABLE IF NOT EXISTS RefreshTokens (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            UserId INTEGER NOT NULL,
            Token TEXT NOT NULL,
            CreatedAt DATETIME NOT NULL,
            ExpiresAt DATETIME NOT NULL,
            IsRevoked INTEGER NOT NULL DEFAULT 0,
            FOREIGN KEY (UserId) REFERENCES Users(UserID) ON DELETE CASCADE
        );",

        @"CREATE UNIQUE INDEX IF NOT EXISTS UX_RefreshTokens_Token
          ON RefreshTokens (Token);",
        
        // Таблица Employers
        @"CREATE TABLE IF NOT EXISTS Employers (
            EmployeeID INTEGER PRIMARY KEY AUTOINCREMENT,
            FullName TEXT NOT NULL,
            Phone TEXT NOT NULL
        );",

        // Таблица AccrualsType 
        @"CREATE TABLE IF NOT EXISTS AccrualsType (
            AccrualsTypeID INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT NOT NULL,
            position INTEGER NOT NULL
        );",

        // Таблица Professions
        @"CREATE TABLE IF NOT EXISTS Professions (
            ProfessionID INTEGER PRIMARY KEY AUTOINCREMENT,
            Title TEXT NOT NULL
        );",

        // Таблица Operations (ИСПРАВЛЕНО: RatePerUnit как REAL для десятичных)
        @"CREATE TABLE IF NOT EXISTS Operations (
            OperationID INTEGER PRIMARY KEY AUTOINCREMENT,
            Description TEXT NOT NULL,
            RatePerUnit REAL NOT NULL
        );",

        // Таблица Works
        @"CREATE TABLE IF NOT EXISTS Works (
            WorkID INTEGER PRIMARY KEY AUTOINCREMENT,
            EmployeeID INTEGER NOT NULL,
            OperationID INTEGER NOT NULL,
            WorkDate DATETIME NOT NULL,
            Quantity INTEGER NOT NULL,
            RejectedQuantity INTEGER DEFAULT 0,
            FOREIGN KEY (EmployeeID) REFERENCES Employers(EmployeeID),
            FOREIGN KEY (OperationID) REFERENCES Operations(OperationID)
        );",

        // Таблица Accruals (ИСПРАВЛЕНО: внешний ключ и типы REAL)
        @"CREATE TABLE IF NOT EXISTS Accruals (
            AccrualID INTEGER PRIMARY KEY AUTOINCREMENT,
            AccrualsTypeID INTEGER NOT NULL,
            WorkID INTEGER NOT NULL,
            Bonus REAL DEFAULT 0.0,
            AccrualTotal REAL NOT NULL,
            FOREIGN KEY (AccrualsTypeID) REFERENCES AccrualsType(AccrualsTypeID),
            FOREIGN KEY (WorkID) REFERENCES Works(WorkID)
        );",

        // Таблица Payments (ИСПРАВЛЕНО: AmountToPay как REAL)
        @"CREATE TABLE IF NOT EXISTS Payments (
            PaymentID INTEGER PRIMARY KEY AUTOINCREMENT,
            EmployeeID INTEGER NOT NULL,
            AmountToPay REAL NOT NULL,
            PaymentDate DATETIME,
            FOREIGN KEY (EmployeeID) REFERENCES Employers(EmployeeID)
        );",

        // Таблица ProfessionEmployers (ИСПРАВЛЕНО: имя PRIMARY KEY)
        @"CREATE TABLE IF NOT EXISTS ProfessionEmployers (
            ProfessionEmployerID INTEGER PRIMARY KEY AUTOINCREMENT,
            EmployeeID INTEGER NOT NULL,
            ProfessionID INTEGER NOT NULL,
            Name TEXT NOT NULL,
            DateOfStart DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
            DateOfEnd DATETIME,
            FOREIGN KEY (EmployeeID) REFERENCES Employers(EmployeeID),
            FOREIGN KEY (ProfessionID) REFERENCES Professions(ProfessionID)
        );"
    };
}