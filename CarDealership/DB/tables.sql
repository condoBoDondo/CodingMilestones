-- GuildCars tables

USE GuildCars
GO

-- remove tables with dependencies on others first

IF EXISTS(SELECT * FROM sys.tables WHERE name='Sale')
	DROP TABLE Sale
GO

IF EXISTS(SELECT * FROM sys.tables WHERE name='Vehicle')
	DROP TABLE Vehicle
GO

IF EXISTS(SELECT * FROM sys.tables WHERE name='Exterior')
	DROP TABLE Exterior
GO

IF EXISTS(SELECT * FROM sys.tables WHERE name='Interior')
	DROP TABLE Interior
GO

IF EXISTS(SELECT * FROM sys.tables WHERE name='Color')
	DROP TABLE Color
GO

IF EXISTS(SELECT * FROM sys.tables WHERE name='Model')
	DROP TABLE Model
GO

IF EXISTS(SELECT * FROM sys.tables WHERE name='Make')
	DROP TABLE Make
GO

--IF EXISTS(SELECT * FROM sys.tables WHERE name='Age')
--	DROP TABLE Age
--GO

IF EXISTS(SELECT * FROM sys.tables WHERE name='BodyStyle')
	DROP TABLE BodyStyle
GO

IF EXISTS(SELECT * FROM sys.tables WHERE name='Transmission')
	DROP TABLE Transmission
GO

IF EXISTS(SELECT * FROM sys.tables WHERE name='ListingStatus')
	DROP TABLE ListingStatus
GO

IF EXISTS(SELECT * FROM sys.tables WHERE name='Special')
	DROP TABLE Special
GO

IF EXISTS(SELECT * FROM sys.tables WHERE name='PurchaseType')
	DROP TABLE PurchaseType
GO

IF EXISTS(SELECT * FROM sys.tables WHERE name='State')
	DROP TABLE [State]
GO

IF EXISTS(SELECT * FROM sys.tables WHERE name='Contact')
	DROP TABLE Contact
GO


-- create tables that others depend on first

--   VEHICLES:

CREATE TABLE Make (
	MakeId int identity(1,1) not null primary key,
	MakeName varchar(30) not null,
	DateAdded date not null default(cast(getdate() AS date)), -- set as today's date
	AddedBy nvarchar(128) not null foreign key references AspNetUsers(Id)
)

-- app should populate model dropdown depending on selected make
CREATE TABLE Model (
	ModelId int identity(1,1) not null primary key,
	MakeId int not null foreign key references Make(MakeId),
	ModelName varchar(30) not null,
	DateAdded date not null default(cast(getdate() AS date)), -- set as today's date
	AddedBy nvarchar(128) not null foreign key references AspNetUsers(Id)
)

-- "Age" table is redundant as app checks whether new/used by mileage
--CREATE TABLE Age (
--	AgeId int identity(1,1) not null primary key,
--	AgeName varchar(4) not null -- new, used
--)

CREATE TABLE BodyStyle (
	BodyStyleId int identity(1,1) not null primary key,
	BodyStyleName varchar(15) not null -- car, suv, truck, van
)

CREATE TABLE Transmission (
	TransmissionId int identity(1,1) not null primary key,
	TransmissionName varchar(9) not null -- automatic, manual
)

-- Exterior and Interior get their color names from here; bound to be a few shared colors
-- should have at least 5 shared between each
CREATE TABLE Color (
	ColorId int identity(1,1) not null primary key,
	ColorName varchar(15) not null
)

CREATE TABLE Exterior (
	ExteriorId int identity(1,1) not null primary key,
	ColorId int not null foreign key references Color(ColorId)
)

CREATE TABLE Interior (
	InteriorId int identity(1,1) not null primary key,
	ColorId int not null foreign key references Color(ColorId)
)

CREATE TABLE ListingStatus (
	ListingStatusId int identity(1,1) not null primary key,
	ListingStatusName varchar(15) not null -- not featured, featured, purchased
										   -- featured and purchased appear for normal users
	                                       -- only featured appear for salespeople
										   -- not featured and featured appear for admins
)

CREATE TABLE Vehicle (
	VehicleId int identity(1,1) not null primary key,
	MakeId int not null foreign key references Make(MakeId),
	ModelId int not null foreign key references Model(ModelId),
	BodyStyleId int not null foreign key references BodyStyle(BodyStyleId),
	[Year] int not null, -- validate in app (between 2000 and current year + 1)
	TransmissionId int not null foreign key references Transmission(TransmissionId),
	ExteriorId int not null foreign key references Exterior(ExteriorId),
	InteriorId int not null foreign key references Interior(InteriorId),
	Mileage int not null, -- validate in app (new is 0-1000, used is anything higher)
	VIN char(17) not null, -- exactly 17 characters; can't be PK due to possibility of duplicate VINs; validate in app (regex)
	MSRP decimal(9,2) not null, -- validate in app (positive)
	SalePrice decimal(9,2) not null, -- validate in app (positive, can't be higher than MSRP)
	[Description] varchar(500) not null,
	ImageFileName varchar(50) null, -- image should be saved as 'inventory-x-y.ext' where x is VehicleId, y is VIN
	ListingStatusId int not null foreign key references ListingStatus(ListingStatusId) default(1)
)

-- Specials will be sorted by highest to lowest ID (least to most recent)
CREATE TABLE Special (
	SpecialId int identity(1,1) not null primary key,
	Title varchar(50) not null,
	[Description] varchar(500) not null,
)


--   SALES:

CREATE TABLE PurchaseType (
	PurchaseTypeId int identity(1,1) not null primary key,
	PurchaseTypeName varchar(20) not null -- bank finance, cash, dealer finance
)

-- don't need full names, just the 2-character abbreviations
CREATE TABLE [State] (
	StateId char(2) not null primary key
)

-- Sale: keeps track of vehicle sales made by salespeople
-- all entered info is of the customer who made the purchase
CREATE TABLE Sale (
	SaleId int identity(1,1) not null primary key,
	CustomerName varchar(50) not null,
	Phone varchar(20) null,  -- one or the other (or both)
	Email varchar(128) null, -- between phone/email; validate email in-app (regex)
	Address1 varchar(50) not null,
	Address2 varchar(50) null, -- street address line 2 optional
	City varchar(50) not null,
	StateId char(2) not null foreign key references [State](StateId),
	Zip char(5) not null, -- 5 digits only
	VehicleId int not null foreign key references Vehicle(VehicleId), -- which vehicle was purchased
	PurchasePrice decimal(9,2) not null, -- validate in-app (between 95% of sales price and MSRP)
	PurchaseDate date not null default(cast(getdate() AS date)), -- set as today's date
	PurchaseTypeId int not null foreign key references PurchaseType(PurchaseTypeId), -- what kind of purchase it is
	Salesperson nvarchar(128) not null foreign key references AspNetUsers(Id) -- set as whichever logged-in user made sale
)


--   CONTACTS:

-- Contact: keeps track of filled-out contact forms by users
CREATE TABLE Contact (
	ContactId int identity(1,1) not null primary key,
	[Name] varchar(50) not null,
	Phone varchar(20) null,  -- one or the other (or both)
	Email varchar(128) null, -- between these two; validate in-app
	[Message] varchar(500) not null,
	ContactDate date not null default(cast(getdate() AS date)), -- set as today's date
	Regarding varchar(50) null -- where the user clicked on contact form
	                           -- get VIN if from vehicle
							   -- get Title if from special
							   -- nothing if from navbar
)