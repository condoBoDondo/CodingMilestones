-- GuildCars stored procedures
USE GuildCars
GO


-- VehicleSelectFeatured:
-- selects vehicles set as "featured" for main page's small buttons
-- selects id, make, model, year, sale price, image

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'VehicleSelectFeatured')
		DROP PROCEDURE VehicleSelectFeatured
GO

CREATE PROCEDURE VehicleSelectFeatured AS
BEGIN
	SELECT
		VehicleId,
		ma.MakeName AS MakeName,
		mo.ModelName AS ModelName,
		[Year],
		SalePrice,
		ImageFileName
	FROM Vehicle v
		INNER JOIN Make ma ON v.MakeId = ma.MakeId
		INNER JOIN Model mo ON v.ModelId = mo.ModelId
	WHERE ListingStatusId IN (2, 3) -- featured or purchased
END
GO


-- VehicleSelectDetails:
-- selects a single vehicle by ID for inventory/details/{id} and sales/purchase/{id}
-- selects all columns

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'VehicleSelectDetails')
		DROP PROCEDURE VehicleSelectDetails
GO

CREATE PROCEDURE VehicleSelectDetails (
	@VehicleId int
) AS
BEGIN
	SELECT
		VehicleId,
		ma.MakeName AS MakeName,
		mo.ModelName AS ModelName,
		b.BodyStyleName AS BodyStyleName,
		[Year],
		t.TransmissionName AS TransmissionName,
		ce.ColorName AS ExteriorName,
		ci.ColorName AS InteriorName,
		Mileage,
		VIN,
		MSRP,
		SalePrice,
		[Description],
		ImageFileName,
		ListingStatusId -- determines what can be done with this item on page
	FROM Vehicle v
		INNER JOIN Make ma ON v.MakeId = ma.MakeId
		INNER JOIN Model mo ON v.ModelId = mo.ModelId
		INNER JOIN BodyStyle b ON v.BodyStyleId = b.BodyStyleId
		INNER JOIN Transmission t ON v.TransmissionId = t.TransmissionId
		INNER JOIN Exterior e ON v.ExteriorId = e.ExteriorId
		INNER JOIN Interior i ON v.InteriorId = i.InteriorId
		INNER JOIN Color ce ON e.ColorId = ce.ColorId
		INNER JOIN Color ci ON i.ColorId = ci.ColorId
	WHERE ListingStatusId IN (2, 3) -- featured or purchased
		AND @VehicleId = VehicleId  -- matches given Id
END
GO


-- SalePurchase:
-- sets given vehicle Id as being purchased,
--   and inserts given purchase in Sale table
-- as these are batched, vehicle will not be updated
--   if there's a problem with inserting new sale

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'SalePurchase')
		DROP PROCEDURE SalePurchase
GO

CREATE PROCEDURE SalePurchase (
	@SaleId int output,
	@CustomerName varchar(50),
	@Phone varchar(20),
	@Email varchar(128),
	@Address1 varchar(50),
	@Address2 varchar(50),
	@City varchar(50),
	@StateId char(2),
	@Zip char(5),
	@VehicleId int,
	@PurchasePrice decimal(9,2),
	@PurchaseTypeId int,
	@Salesperson nvarchar(128)
) AS
BEGIN
	-- set given vehicle to purchased status
	UPDATE Vehicle
	SET ListingStatusId = 3
	WHERE VehicleId = @VehicleId

	-- insert sale
	INSERT INTO Sale(
		CustomerName,
		Phone,
		Email,
		Address1,
		Address2,
		City,
		StateId,
		Zip,
		VehicleId,
		PurchasePrice,
		PurchaseTypeId,
		Salesperson)
	VALUES(
		@CustomerName,
		@Phone,
		@Email,
		@Address1,
		@Address2,
		@City,
		@StateId,
		@Zip,
		@VehicleId,
		@PurchasePrice,
		@PurchaseTypeId,
		@Salesperson)

	SET @SaleId = SCOPE_IDENTITY()
END
GO


-- VehicleInsert:
-- adds a vehicle to table with given column data
-- returns new VehicleId (to redirect to edit page immediately)

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'VehicleInsert')
		DROP PROCEDURE VehicleInsert
GO

CREATE PROCEDURE VehicleInsert (
	@VehicleId int output,
	@MakeId int,
	@ModelId int,
	@BodyStyleId int,
	@Year int,
	@TransmissionId int,
	@ExteriorId int,
	@InteriorId int,
	@Mileage int,
	@VIN char(17),
	@MSRP decimal(9,2),
	@SalePrice decimal(9,2),
	@Description varchar(500),
	@ImageFileName varchar(50)
) AS
BEGIN
	INSERT INTO Vehicle(
		MakeId,
		ModelId,
		BodyStyleId,
		[Year],
		TransmissionId,
		ExteriorId,
		InteriorId,
		Mileage,
		VIN,
		MSRP,
		SalePrice,
		[Description],
		ImageFileName)
	VALUES(
		@MakeId,
		@ModelId,
		@BodyStyleId,
		@Year,
		@TransmissionId,
		@ExteriorId,
		@InteriorId,
		@Mileage,
		@VIN,
		@MSRP,
		@SalePrice,
		@Description,
		@ImageFileName)

	SET @VehicleId = SCOPE_IDENTITY()
END
GO


-- VehicleUpdate:
-- updates already-existing vehicle with given column data

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'VehicleUpdate')
		DROP PROCEDURE VehicleUpdate
GO

CREATE PROCEDURE VehicleUpdate (
	@VehicleId int,
	@MakeId int,
	@ModelId int,
	@BodyStyleId int,
	@Year int,
	@TransmissionId int,
	@ExteriorId int,
	@InteriorId int,
	@Mileage int,
	@VIN char(17),
	@MSRP decimal(9,2),
	@SalePrice decimal(9,2),
	@Description varchar(500),
	@ImageFileName varchar(50),
	@ListingStatusId int
) AS
BEGIN
	UPDATE Vehicle SET
		MakeId = @MakeId,
		ModelId = @ModelId,
		BodyStyleId = @BodyStyleId,
		[Year] = @Year,
		TransmissionId = @TransmissionId,
		ExteriorId = @ExteriorId,
		InteriorId = @InteriorId,
		Mileage = @Mileage,
		VIN = @VIN,
		MSRP = @MSRP,
		SalePrice = @SalePrice,
		[Description] = @Description,
		ImageFileName = ISNULL(@ImageFileName, ImageFileName), -- set to current value if no new given
		ListingStatusId = @ListingStatusId
	WHERE VehicleId = @VehicleId
END
GO


-- MakeSelectAllReport:
-- selects all makes for admin purposes
-- selects make name, date added, and user who added it

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'MakeSelectAllReport')
		DROP PROCEDURE MakeSelectAllReport
GO

CREATE PROCEDURE MakeSelectAllReport AS
BEGIN
	SELECT MakeName, DateAdded, u.UserName AS AddedBy
	FROM Make m
	INNER JOIN AspNetUsers u ON m.AddedBy = u.Id
END
GO


-- MakeInsert:
-- inserts new make into its table

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'MakeInsert')
		DROP PROCEDURE MakeInsert
GO

CREATE PROCEDURE MakeInsert (
	@MakeId int output,
	@MakeName varchar(30),
	@AddedBy nvarchar(128)
) AS
BEGIN
	INSERT INTO Make(MakeName, AddedBy)
	VALUES(@MakeName, @AddedBy)

	SET @MakeId = SCOPE_IDENTITY()
END
GO


-- ContactInsert:
-- inserts new contact into its table

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'ContactInsert')
		DROP PROCEDURE ContactInsert
GO

CREATE PROCEDURE ContactInsert (
	@ContactId int output,
	@Name varchar(50),
	@Phone varchar(20),
	@Email varchar(128),
	@Message varchar(500),
	@Regarding varchar(50)
) AS
BEGIN
	INSERT INTO Contact([Name], Phone, Email, [Message], Regarding)
	VALUES(@Name, @Phone, @Email, @Message, @Regarding)

	SET @ContactId = SCOPE_IDENTITY()
END
GO


-- ModelSelectAllReport:
-- selects all models for admin purposes
-- selects make name, model name date added, and user who added it

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'ModelSelectAllReport')
		DROP PROCEDURE ModelSelectAllReport
GO

CREATE PROCEDURE ModelSelectAllReport AS
BEGIN
	SELECT ma.MakeName, ModelName, mo.DateAdded, u.UserName AS AddedBy
	FROM Model mo
	INNER JOIN Make ma ON mo.MakeId = ma.MakeId
	INNER JOIN AspNetUsers u ON mo.AddedBy = u.Id
END
GO


-- ModelInsert:
-- inserts new model into its table

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'ModelInsert')
		DROP PROCEDURE ModelInsert
GO

CREATE PROCEDURE ModelInsert (
	@ModelId int output,
	@MakeId int,
	@ModelName varchar(30),
	@AddedBy nvarchar(128)
) AS
BEGIN
	INSERT INTO Model(MakeId, ModelName, AddedBy)
	VALUES(@MakeId, @ModelName, @AddedBy)

	SET @ModelId = SCOPE_IDENTITY()
END
GO


-- SpecialInsert:
-- inserts new special into its table

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'SpecialInsert')
		DROP PROCEDURE SpecialInsert
GO

CREATE PROCEDURE SpecialInsert (
	@SpecialId int output,
	@Title varchar(50),
	@Description varchar(500)
) AS
BEGIN
	INSERT INTO Special(Title, [Description])
	VALUES(@Title, @Description)

	SET @SpecialId = SCOPE_IDENTITY()
END
GO


-- SpecialDelete:
-- deletes special from its table

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'SpecialDelete')
		DROP PROCEDURE SpecialDelete
GO

CREATE PROCEDURE SpecialDelete (
    @SpecialId int
) AS
BEGIN
	DELETE FROM Special WHERE SpecialId = @SpecialId
END
GO


-- VehicleDelete:
-- deletes vehicle from its table

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'VehicleDelete')
		DROP PROCEDURE VehicleDelete
GO

CREATE PROCEDURE VehicleDelete (
    @VehicleId int
) AS
BEGIN
	DELETE FROM Vehicle WHERE VehicleId = @VehicleId
END
GO


-- SaleAggregate:
-- counts number of vehicles sold and total in sales per salesperson
-- gets possible min/max date of sales

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'SaleAggregate')
		DROP PROCEDURE SaleAggregate
GO

CREATE PROCEDURE SaleAggregate (
	@Salesperson nvarchar(128) = null,
	@MinDate date = null,
	@MaxDate date = null
) AS
BEGIN
	SELECT
		u.Id AS UserId,
		u.FirstName + ' ' + u.LastName AS FirstLastName,
		u.Email AS Email,
		SUM(PurchasePrice) AS TotalSales,
		COUNT(Salesperson) AS TotalVehicles
	FROM Sale s
		INNER JOIN AspNetUsers u ON s.Salesperson = u.Id
	WHERE Salesperson LIKE ISNULL(@Salesperson, '%')
		AND PurchaseDate BETWEEN ISNULL(@MinDate, '1-1-0001') AND ISNULL(@MaxDate, '12-31-9999')
	GROUP BY u.Id, Salesperson, u.FirstName, u.LastName, u.Email
	ORDER BY TotalSales DESC
END
GO


-- BodyStyleSelectAll

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'BodyStyleSelectAll')
		DROP PROCEDURE BodyStyleSelectAll
GO

CREATE PROCEDURE BodyStyleSelectAll AS
BEGIN
	SELECT BodyStyleId, BodyStyleName
	FROM BodyStyle
END
GO


-- ExteriorSelectAll

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'ExteriorSelectAll')
		DROP PROCEDURE ExteriorSelectAll
GO

CREATE PROCEDURE ExteriorSelectAll AS
BEGIN
	SELECT ExteriorId, e.ColorId, c.ColorName
	FROM Exterior e
		INNER JOIN Color c ON e.ColorId = c.ColorId
END
GO


-- InteriorSelectAll

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'InteriorSelectAll')
		DROP PROCEDURE InteriorSelectAll
GO

CREATE PROCEDURE InteriorSelectAll AS
BEGIN
	SELECT InteriorId, i.ColorId, c.ColorName
	FROM Interior i
		INNER JOIN Color c ON i.ColorId = c.ColorId
END
GO


-- MakeSelectAll

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'MakeSelectAll')
		DROP PROCEDURE MakeSelectAll
GO

CREATE PROCEDURE MakeSelectAll AS
BEGIN
	SELECT MakeId, MakeName
	FROM Make
END
GO


-- ModelSelectAll

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'ModelSelectAll')
		DROP PROCEDURE ModelSelectAll
GO

CREATE PROCEDURE ModelSelectAll AS
BEGIN
	SELECT ModelId, MakeId, ModelName
	FROM Model
END
GO


-- ModelSelectByMake:
-- only gets models that match their make

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'ModelSelectByMake')
		DROP PROCEDURE ModelSelectByMake
GO

CREATE PROCEDURE ModelSelectByMake (
	@MakeId int
) AS
BEGIN
	SELECT ModelId, mo.MakeId, ModelName, MakeName
	FROM Model mo
		INNER JOIN Make ma ON mo.MakeId = ma.MakeId
	WHERE mo.MakeId = @MakeId
END
GO


-- PurchaseTypeSelectAll

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'PurchaseTypeSelectAll')
		DROP PROCEDURE PurchaseTypeSelectAll
GO

CREATE PROCEDURE PurchaseTypeSelectAll AS
BEGIN
	SELECT PurchaseTypeId, PurchaseTypeName
	FROM PurchaseType
END
GO


-- SpecialSelectAll

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'SpecialSelectAll')
		DROP PROCEDURE SpecialSelectAll
GO

CREATE PROCEDURE SpecialSelectAll AS
BEGIN
	SELECT SpecialId, Title, [Description]
	FROM Special
	ORDER BY SpecialId DESC
END
GO


-- StateSelectAll

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'StateSelectAll')
		DROP PROCEDURE StateSelectAll
GO

CREATE PROCEDURE StateSelectAll AS
BEGIN
	SELECT StateId
	FROM [State]
END
GO


-- SaleSelectAll

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'SaleSelectAll')
		DROP PROCEDURE SaleSelectAll
GO

CREATE PROCEDURE SaleSelectAll AS
BEGIN
	SELECT SaleId, CustomerName, Phone, Email,
		   Address1, Address2, City, StateId, Zip,
		   VehicleId, PurchasePrice, PurchaseDate,
		   Salesperson
	FROM Sale
END
GO


-- TransmissionSelectAll

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'TransmissionSelectAll')
		DROP PROCEDURE TransmissionSelectAll
GO

CREATE PROCEDURE TransmissionSelectAll AS
BEGIN
	SELECT TransmissionId, TransmissionName
	FROM Transmission
END
GO


-- VehicleSelectAll

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'VehicleSelectAll')
		DROP PROCEDURE VehicleSelectAll
GO

CREATE PROCEDURE VehicleSelectAll AS
BEGIN
	SELECT VehicleId, MakeId, ModelId, BodyStyleId, [Year], TransmissionId,
		   ExteriorId, InteriorId, Mileage, VIN, MSRP, SalePrice,
		   [Description], ImageFileName, ListingStatusId
	FROM Vehicle
END
GO


-- VehicleSelectById

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'VehicleSelectById')
		DROP PROCEDURE VehicleSelectById
GO

CREATE PROCEDURE VehicleSelectById (
	@VehicleId int
)AS
BEGIN
	SELECT VehicleId, MakeId, ModelId, BodyStyleId, [Year], TransmissionId,
		   ExteriorId, InteriorId, Mileage, VIN, MSRP, SalePrice,
		   [Description], ImageFileName, ListingStatusId
	FROM Vehicle
	WHERE VehicleId = @VehicleId
END
GO


-- UserSelectShort:
-- selects all users formatted with minimal data

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'UserSelectShort')
		DROP PROCEDURE UserSelectShort
GO

CREATE PROCEDURE UserSelectShort AS
BEGIN
	SELECT u.Id, FirstName, LastName, Email, r.Id AS RoleId, r.[Name] AS RoleName
	FROM AspNetUsers u
		LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId -- get users with no role assigned
		LEFT JOIN AspNetRoles r ON r.Id = ur.RoleId
END
GO


-- UserSelectShortById:
-- selects single user by id formatted with minimal data

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'UserSelectShortById')
		DROP PROCEDURE UserSelectShortById
GO

CREATE PROCEDURE UserSelectShortById (
	@UserId nvarchar(128)
)AS
BEGIN
	SELECT u.Id, FirstName, LastName, Email, r.Id AS RoleId, r.[Name] AS RoleName
	FROM AspNetUsers u
		INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
		INNER JOIN AspNetRoles r ON r.Id = ur.RoleId
	WHERE u.Id = @UserId
END
GO


-- UserUpdate:
-- updates given user's first name, last name, username, email
IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'UserUpdate')
		DROP PROCEDURE UserUpdate
GO

CREATE PROCEDURE UserUpdate (
	@UserId nvarchar(128),
	@FirstName nvarchar(64),
	@LastName nvarchar(64),
	@Email nvarchar(256),
	@UserName nvarchar(256)
)AS
BEGIN
	UPDATE AspNetUsers SET
		FirstName = @FirstName,
		LastName = @LastName,
		Email = @Email,
		UserName = @UserName
	WHERE Id = @UserId
END
GO


-- UserRoleSelectShort:
-- selects user roles formatted with minimal data

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'UserRoleSelectShort')
		DROP PROCEDURE UserRoleSelectShort
GO

CREATE PROCEDURE UserRoleSelectShort AS
BEGIN
	SELECT r.Id, r.[Name]
	FROM AspNetRoles r
END
GO



--------------------------------------------------------------
--
-- Deprecated sprocs (replaced by flexible statements in app)
--
--------------------------------------------------------------

-- VehicleSelectNew: (deprecated)
-- selects top 20 NEW vehicles set as "featured" or "purchased" for inventory/new
-- "new" means mileage is between 0-1000 (inclusive)
-- selects all columns except mileage, description, and listing status
-- sorts by highest to lowest MSRP
-- supports searching by:
--   search field (make, model, or year)
--   min/max price (ignore any non-set fields)
--   min/max year (ignore any non-set fields)

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'VehicleSelectNew')
		DROP PROCEDURE VehicleSelectNew
GO

CREATE PROCEDURE VehicleSelectNew (
	@Input varchar(30) = null,
	@MinPrice decimal = null,
	@MaxPrice decimal = null,
	@MinYear int = null,
	@MaxYear int = null
) AS
BEGIN
	SELECT TOP 20
		VehicleId,
		ma.MakeName AS MakeName,
		mo.ModelName AS ModelName,
		b.BodyStyleName AS BodyStyleName,
		[Year],
		t.TransmissionName AS TransmissionName,
		ce.ColorName AS ExteriorName,
		ci.ColorName AS InteriorName,
		Mileage,
		VIN,
		MSRP,
		SalePrice,
		ImageFileName,
		ListingStatusId
	FROM Vehicle v
		INNER JOIN Make ma ON v.MakeId = ma.MakeId
		INNER JOIN Model mo ON v.ModelId = mo.ModelId
		INNER JOIN BodyStyle b ON v.BodyStyleId = b.BodyStyleId
		INNER JOIN Transmission t ON v.TransmissionId = t.TransmissionId
		INNER JOIN Exterior e ON v.ExteriorId = e.ExteriorId
		INNER JOIN Interior i ON v.InteriorId = i.InteriorId
		INNER JOIN Color ce ON e.ColorId = ce.ColorId
		INNER JOIN Color ci ON i.ColorId = ci.ColorId
	WHERE ListingStatusId IN (2, 3)														-- featured or purchased
		AND Mileage BETWEEN 0 AND 1000													-- between given mileage
		AND ([Year] = @Input															-- included in search field
			OR ma.MakeName LIKE '%' + ISNULL(@Input, '') + '%'
			OR mo.ModelName LIKE '%' + ISNULL(@Input, '') + '%')
		AND SalePrice BETWEEN ISNULL(@MinPrice, 0.00) AND ISNULL(@MaxPrice, 9999999.99)	-- between price range
		AND [Year] BETWEEN ISNULL(@MinYear, 1) AND ISNULL(@MaxYear, 9999)				-- between year range
	ORDER BY MSRP DESC
END
GO


-- VehicleSelectUsed: (deprecated)
-- selects top 20 USED vehicles set as "featured" or "purchased" for inventory/used
-- "used" means mileage is above 1000
-- selects all columns except description and listing status
-- sorts by highest to lowest MSRP
-- supports searching by:
--   search field (make, model, or year)
--   min/max price (ignore any non-set fields)
--   min/max year (ignore any non-set fields)

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'VehicleSelectUsed')
		DROP PROCEDURE VehicleSelectUsed
GO

CREATE PROCEDURE VehicleSelectUsed(
	@Input varchar(30) = null,
	@MinPrice decimal = null,
	@MaxPrice decimal = null,
	@MinYear int = null,
	@MaxYear int = null
) AS
BEGIN
	SELECT TOP 20
		VehicleId,
		ma.MakeName AS MakeName,
		mo.ModelName AS ModelName,
		b.BodyStyleName AS BodyStyleName,
		[Year],
		t.TransmissionName AS TransmissionName,
		ce.ColorName AS ExteriorName,
		ci.ColorName AS InteriorName,
		Mileage,
		VIN,
		MSRP,
		SalePrice,
		ImageFileName,
		ListingStatusId
	FROM Vehicle v
		INNER JOIN Make ma ON v.MakeId = ma.MakeId
		INNER JOIN Model mo ON v.ModelId = mo.ModelId
		INNER JOIN BodyStyle b ON v.BodyStyleId = b.BodyStyleId
		INNER JOIN Transmission t ON v.TransmissionId = t.TransmissionId
		INNER JOIN Exterior e ON v.ExteriorId = e.ExteriorId
		INNER JOIN Interior i ON v.InteriorId = i.InteriorId
		INNER JOIN Color ce ON e.ColorId = ce.ColorId
		INNER JOIN Color ci ON i.ColorId = ci.ColorId
	WHERE ListingStatusId IN (2, 3)														-- featured or purchased
		AND Mileage > 1000																-- above given mileage
		AND ([Year] = @Input															-- included in search field
			OR ma.MakeName LIKE '%' + ISNULL(@Input, '') + '%'
			OR mo.ModelName LIKE '%' + ISNULL(@Input, '') + '%')
		AND SalePrice BETWEEN ISNULL(@MinPrice, 0.00) AND ISNULL(@MaxPrice, 9999999.99)	-- between price range
		AND [Year] BETWEEN ISNULL(@MinYear, 1) AND ISNULL(@MaxYear, 9999)				-- between year range
	ORDER BY MSRP DESC
END
GO


-- VehicleSelectSales: (deprecated)
-- selects top 20 vehicles (new and used) set as "featured" for sales/index
-- selects all columns except description and listing status
-- sorts by highest to lowest MSRP
-- supports searching by:
--   search field (make, model, or year)
--   min/max price (ignore any non-set fields)
--   min/max year (ignore any non-set fields)

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'VehicleSelectSales')
		DROP PROCEDURE VehicleSelectSales
GO

CREATE PROCEDURE VehicleSelectSales (
	@Input varchar(30) = null,
	@MinPrice decimal = null,
	@MaxPrice decimal = null,
	@MinYear int = null,
	@MaxYear int = null
) AS
BEGIN
	SELECT TOP 20
		VehicleId,
		ma.MakeName AS MakeName,
		mo.ModelName AS ModelName,
		b.BodyStyleName AS BodyStyleName,
		[Year],
		t.TransmissionName AS TransmissionName,
		ce.ColorName AS ExteriorName,
		ci.ColorName AS InteriorName,
		Mileage,
		VIN,
		MSRP,
		SalePrice,
		ImageFileName,
		ListingStatusId
	FROM Vehicle v
		INNER JOIN Make ma ON v.MakeId = ma.MakeId
		INNER JOIN Model mo ON v.ModelId = mo.ModelId
		INNER JOIN BodyStyle b ON v.BodyStyleId = b.BodyStyleId
		INNER JOIN Transmission t ON v.TransmissionId = t.TransmissionId
		INNER JOIN Exterior e ON v.ExteriorId = e.ExteriorId
		INNER JOIN Interior i ON v.InteriorId = i.InteriorId
		INNER JOIN Color ce ON e.ColorId = ce.ColorId
		INNER JOIN Color ci ON i.ColorId = ci.ColorId
	WHERE ListingStatusId = 2															-- featured only
		AND ([Year] = @Input															-- included in search field
			OR ma.MakeName LIKE '%' + ISNULL(@Input, '') + '%'
			OR mo.ModelName LIKE '%' + ISNULL(@Input, '') + '%')
		AND SalePrice BETWEEN ISNULL(@MinPrice, 0.00) AND ISNULL(@MaxPrice, 9999999.99)	-- between price range
		AND [Year] BETWEEN ISNULL(@MinYear, 1) AND ISNULL(@MaxYear, 9999)				-- between year range
	ORDER BY MSRP DESC
END
GO


-- VehicleSelectAdmin: (deprecated)
-- selects top 20 vehicles (new and used) set as "not featured" or "featured" for admin/vehicles
-- selects all columns except description
-- sorts by highest to lowest MSRP
-- supports searching by:
--   search field (make, model, or year)
--   min/max price (ignore any non-set fields)
--   min/max year (ignore any non-set fields)

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'VehicleSelectAdmin')
		DROP PROCEDURE VehicleSelectAdmin
GO

CREATE PROCEDURE VehicleSelectAdmin (
	@Input varchar(30) = null,
	@MinPrice decimal = null,
	@MaxPrice decimal = null,
	@MinYear int = null,
	@MaxYear int = null
) AS
BEGIN
	SELECT TOP 20
		VehicleId,
		ma.MakeName AS MakeName,
		mo.ModelName AS ModelName,
		b.BodyStyleName AS BodyStyleName,
		[Year],
		t.TransmissionName AS TransmissionName,
		ce.ColorName AS ExteriorName,
		ci.ColorName AS InteriorName,
		Mileage,
		VIN,
		MSRP,
		SalePrice,
		ImageFileName,
		ListingStatusId -- used to highlight what is/isn't featured
	FROM Vehicle v
		INNER JOIN Make ma ON v.MakeId = ma.MakeId
		INNER JOIN Model mo ON v.ModelId = mo.ModelId
		INNER JOIN BodyStyle b ON v.BodyStyleId = b.BodyStyleId
		INNER JOIN Transmission t ON v.TransmissionId = t.TransmissionId
		INNER JOIN Exterior e ON v.ExteriorId = e.ExteriorId
		INNER JOIN Interior i ON v.InteriorId = i.InteriorId
		INNER JOIN Color ce ON e.ColorId = ce.ColorId
		INNER JOIN Color ci ON i.ColorId = ci.ColorId
	WHERE ListingStatusId IN (1, 2)														-- is or is not featured
		AND ([Year] = @Input															-- included in search field
			OR ma.MakeName LIKE '%' + ISNULL(@Input, '') + '%'
			OR mo.ModelName LIKE '%' + ISNULL(@Input, '') + '%')
		AND SalePrice BETWEEN ISNULL(@MinPrice, 0.00) AND ISNULL(@MaxPrice, 9999999.99)	-- between price range
		AND [Year] BETWEEN ISNULL(@MinYear, 1) AND ISNULL(@MaxYear, 9999)				-- between year range
	ORDER BY MSRP DESC
END
GO


-- VehicleAggregateNew: (deprecated)
-- counts new vehicles still in stock and gets their total value
-- sorts by year -> make -> model

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'VehicleAggregateNew')
		DROP PROCEDURE VehicleAggregateNew
GO

CREATE PROCEDURE VehicleAggregateNew AS
BEGIN
	SELECT
		[Year],
		ma.MakeName AS MakeName,
		mo.ModelName AS ModelName,
		COUNT([Year]) AS [Count],
		SUM(MSRP) AS StockValue
	FROM Vehicle v
		INNER JOIN Make ma ON v.MakeId = ma.MakeId
		INNER JOIN Model mo ON v.ModelId = mo.ModelId
	WHERE ListingStatusId IN (1, 2) -- is or is not featured
		AND Mileage BETWEEN 0 AND 1000
	GROUP BY [Year], ma.MakeName, mo.ModelName
	ORDER BY [Year] DESC, ma.MakeName, mo.ModelName
END
GO


-- VehicleAggregateUsed: (deprecated)
-- counts used vehicles still in stock and gets their total value
-- sorts by year -> make -> model

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'VehicleAggregateUsed')
		DROP PROCEDURE VehicleAggregateUsed
GO

CREATE PROCEDURE VehicleAggregateUsed AS
BEGIN
	SELECT
		[Year],
		ma.MakeName AS MakeName,
		mo.ModelName AS ModelName,
		COUNT([Year]) AS [Count],
		SUM(MSRP) AS StockValue
	FROM Vehicle v
		INNER JOIN Make ma ON v.MakeId = ma.MakeId
		INNER JOIN Model mo ON v.ModelId = mo.ModelId
	WHERE ListingStatusId IN (1, 2) -- is or is not featured
		AND Mileage > 1000
	GROUP BY [Year], ma.MakeName, mo.ModelName
	ORDER BY [Year] DESC, ma.MakeName, mo.ModelName
END
GO


-- UserSelectShortWithSales: (deprecated)
-- selects all users that have made sales

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'UserSelectShortWithSales')
		DROP PROCEDURE UserSelectShortWithSales
GO

CREATE PROCEDURE UserSelectShortWithSales AS
BEGIN
	SELECT u.Id, FirstName, LastName, u.Email, r.Id AS RoleId, r.[Name] AS RoleName
	FROM AspNetUsers u
		INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
		INNER JOIN AspNetRoles r ON r.Id = ur.RoleId
		INNER JOIN Sale s ON u.Id = s.Salesperson
END
GO