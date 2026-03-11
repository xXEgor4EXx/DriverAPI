namespace MyAPP.Driver.MariaDB;

public class DDL
{
    public static readonly string[] Defenition = {
@"CREATE DATABASE IF NOT EXISTS `myapp` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;",
@"use `myapp`;
CREATE TABLE `Users` (
  `UserID` int(11) NOT NULL AUTO_INCREMENT,
  `Email` varchar(255) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `Role` varchar(50) NOT NULL,
  `IsActive` tinyint(1) NOT NULL DEFAULT 1,
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL,
  PRIMARY KEY (`UserID`),
  UNIQUE KEY `UX_Users_Email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;",
@"use `myapp`;
CREATE TABLE IF NOT EXISTS `RefreshTokens` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `Token` varchar(255) NOT NULL,
  `CreatedAt` datetime NOT NULL,
  `ExpiresAt` datetime NOT NULL,
  `IsRevoked` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UX_RefreshTokens_Token` (`Token`),
  KEY `IX_RefreshTokens_UserId` (`UserId`),
  CONSTRAINT `refreshtokens_ibfk_1`
    FOREIGN KEY (`UserId`)
    REFERENCES `Users` (`UserID`)
    ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;",
@"use `myapp`;
    CREATE TABLE `AccrualsType` (
      `AccrualsTypeID` int(11) NOT NULL AUTO_INCREMENT,
      `Name` varchar(100) NOT NULL,
      `position` int(11) NOT NULL,
      PRIMARY KEY (`AccrualsTypeID`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;",
    
    @"use `myapp`;
    CREATE TABLE `Employers` (
      `EmployeeID` int(11) NOT NULL AUTO_INCREMENT,
      `FullName` varchar(150) NOT NULL,
      `Phone` char(11) NOT NULL,
      PRIMARY KEY (`EmployeeID`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;",
    
    @"use `myapp`;
    CREATE TABLE `Operations` (
      `OperationID` int(11) NOT NULL AUTO_INCREMENT,
      `Description` varchar(200) NOT NULL,
      `RatePerUnit` decimal(10,2) NOT NULL,
      PRIMARY KEY (`OperationID`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;",
    
    @"use `myapp`;
    CREATE TABLE `Professions` (
      `ProfessionID` int(11) NOT NULL AUTO_INCREMENT,
      `Title` varchar(100) NOT NULL,
      PRIMARY KEY (`ProfessionID`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;",
    
    @"use `myapp`;
    CREATE TABLE `Works` (
      `WorkID` int(11) NOT NULL AUTO_INCREMENT,
      `EmployeeID` int(11) NOT NULL,
      `OperationID` int(11) NOT NULL,
      `WorkDate` date NOT NULL,
      `Quantity` int(11) NOT NULL,
      `RejectedQuantity` int(11) DEFAULT 0,
      PRIMARY KEY (`WorkID`),
      KEY `EmployeeID` (`EmployeeID`),
      KEY `OperationID` (`OperationID`),
      CONSTRAINT `works_ibfk_1` FOREIGN KEY (`EmployeeID`) REFERENCES `Employers` (`EmployeeID`),
      CONSTRAINT `works_ibfk_2` FOREIGN KEY (`OperationID`) REFERENCES `Operations` (`OperationID`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;",
    
    @"use `myapp`;
    CREATE TABLE `Accruals` (
      `AccrualID` int(11) NOT NULL AUTO_INCREMENT,
      `WorkID` int(11) NOT NULL,
      `Bonus` decimal(10,2) DEFAULT 0.00,
      `AccrualTotal` decimal(10,2) NOT NULL,
      `AccrualsTypeID` int(11) NOT NULL,
      PRIMARY KEY (`AccrualID`),
      KEY `WorkID` (`WorkID`),
      KEY `AccrualsTypeID` (`AccrualsTypeID`),
      CONSTRAINT `accruals_accrualstype_FK` FOREIGN KEY (`AccrualsTypeID`) REFERENCES `AccrualsType` (`AccrualsTypeID`),
      CONSTRAINT `accruals_ibfk_1` FOREIGN KEY (`WorkID`) REFERENCES `Works` (`WorkID`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;",
    
    @"use `myapp`;
    CREATE TABLE `Payments` (
      `PaymentID` int(11) NOT NULL AUTO_INCREMENT,
      `EmployeeID` int(11) NOT NULL,
      `AmountToPay` decimal(10,2) NOT NULL,
      `PaymentDate` date DEFAULT curdate(),
      PRIMARY KEY (`PaymentID`),
      KEY `EmployeeID` (`EmployeeID`),
      CONSTRAINT `payments_ibfk_1` FOREIGN KEY (`EmployeeID`) REFERENCES `Employers` (`EmployeeID`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;",
    
    @"use `myapp`;
    CREATE TABLE `ProfessionEmployers` (
      `ProfessionEmployeeID` int(11) NOT NULL AUTO_INCREMENT,
      `EmployeeID` int(11) NOT NULL,
      `ProfessionID` int(11) NOT NULL,
      `Name` varchar(250) NOT NULL,
      `DateOfStart` date DEFAULT curdate(),
      `DateOfEnd` date DEFAULT NULL,
      PRIMARY KEY (`ProfessionEmployeeID`),
      KEY `EmployeeID` (`EmployeeID`),
      KEY `ProfessionID` (`ProfessionID`),
      CONSTRAINT `professionemployers_ibfk_1` FOREIGN KEY (`EmployeeID`) REFERENCES `Employers` (`EmployeeID`),
      CONSTRAINT `professionemployers_ibfk_2` FOREIGN KEY (`ProfessionID`) REFERENCES `Professions`(`ProfessionID`)
    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;"
};
}
