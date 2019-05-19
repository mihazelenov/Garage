USE master
GO

CREATE DATABASE PGKCenter
GO

USE PGKCenter
GO

CREATE TABLE Garage
(
	ID int IDENTITY(1,1) PRIMARY KEY,
	Number varchar(4) NOT NULL CONSTRAINT UK_Number UNIQUE,
	Name varchar(max) NOT NULL,
	Comment varchar(max) NULL,
	Address varchar(max) NOT NULL,
	Square decimal NOT NULL,
	CellPhone varchar(20) NULL,
	StaticPhone varchar(20) NULL
)
GO