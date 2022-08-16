-- GuildCars db creation

USE master
GO

-- remove existing db
IF EXISTS(SELECT * FROM sys.databases WHERE name='GuildCars')
	DROP DATABASE GuildCars
GO

CREATE DATABASE GuildCars
GO

USE GuildCars
GO