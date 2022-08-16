-- purge all GuildCars table data and populate with prefabricated data

USE GuildCars
GO

IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.ROUTINES
	WHERE ROUTINE_NAME = 'DbReset')
		DROP PROCEDURE DbReset
GO

CREATE PROCEDURE DbReset AS
BEGIN
	DELETE FROM Sale;
	DELETE FROM Vehicle;
	DELETE FROM Exterior;
	DELETE FROM Interior;
	DELETE FROM Color;
	DELETE FROM Model;
	DELETE FROM Make;
	DELETE FROM BodyStyle;
	DELETE FROM Transmission;
	DELETE FROM ListingStatus;
	DELETE FROM Special;
	DELETE FROM PurchaseType;
	DELETE FROM [State];
	DELETE FROM Contact;

	DELETE FROM AspNetUsers WHERE Id = '00000000-0000-0000-0000-000000000000';

	-- reseed identity columns
	DBCC CHECKIDENT('Make', RESEED, 1)
	DBCC CHECKIDENT('Model', RESEED, 1)
	DBCC CHECKIDENT('BodyStyle', RESEED, 1)
	DBCC CHECKIDENT('Transmission', RESEED, 1)
	DBCC CHECKIDENT('Color', RESEED, 1)
	DBCC CHECKIDENT('Exterior', RESEED, 1)
	DBCC CHECKIDENT('Interior', RESEED, 1)
	DBCC CHECKIDENT('ListingStatus', RESEED, 1)
	DBCC CHECKIDENT('Vehicle', RESEED, 1)
	DBCC CHECKIDENT('Special', RESEED, 1)
	DBCC CHECKIDENT('PurchaseType', RESEED, 1)
	DBCC CHECKIDENT('Sale', RESEED, 1)
	DBCC CHECKIDENT('Contact', RESEED, 1)

	-- test user
	INSERT INTO AspNetUsers(Id, FirstName, LastName, Email, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, UserName)
	VALUES ('00000000-0000-0000-0000-000000000000', 'Newman', 'Testguy', 'test@test.com', 0, 0, 0, 0, 0, 'test')

	-- ensure added data is always in the same order for tables with
	--   an auto-incrementing identity column (IDENTITY_INSERT)
	SET IDENTITY_INSERT Make ON
	INSERT INTO Make(MakeId, MakeName, AddedBy)
	VALUES
	(1, 'Audi', '00000000-0000-0000-0000-000000000000'),
	(2, 'Chevrolet', '00000000-0000-0000-0000-000000000000'),
	(3, 'Ford', '00000000-0000-0000-0000-000000000000'),
	(4, 'Hyundai', '00000000-0000-0000-0000-000000000000'),
	(5, 'Subaru', '00000000-0000-0000-0000-000000000000')
	SET IDENTITY_INSERT Make OFF

	SET IDENTITY_INSERT Model ON
	INSERT INTO Model(ModelId, MakeId, ModelName, AddedBy)
	VALUES
	(1, 1, 'A3', '00000000-0000-0000-0000-000000000000'),
	(2, 1, 'A8', '00000000-0000-0000-0000-000000000000'),
	(3, 2, 'Spark', '00000000-0000-0000-0000-000000000000'),
	(4, 2, 'Malibu', '00000000-0000-0000-0000-000000000000'),
	(5, 3, 'F150', '00000000-0000-0000-0000-000000000000'),
	(6, 3, 'Bronco', '00000000-0000-0000-0000-000000000000'),
	(7, 4, 'Palisade', '00000000-0000-0000-0000-000000000000'),
	(8, 4, 'Accent', '00000000-0000-0000-0000-000000000000'),
	(9, 5, 'Forester', '00000000-0000-0000-0000-000000000000'),
	(10,5, 'Outback', '00000000-0000-0000-0000-000000000000')
	SET IDENTITY_INSERT Model OFF

	SET IDENTITY_INSERT BodyStyle ON
	INSERT INTO BodyStyle(BodyStyleId, BodyStyleName)
	VALUES
	(1, 'Car'),
	(2, 'SUV'),
	(3, 'Truck'),
	(4, 'Van')
	SET IDENTITY_INSERT BodyStyle OFF

	SET IDENTITY_INSERT Transmission ON
	INSERT INTO Transmission(TransmissionId, TransmissionName)
	VALUES
	(1, 'Automatic'),
	(2, 'Manual')
	SET IDENTITY_INSERT Transmission OFF

	SET IDENTITY_INSERT Color ON
	INSERT INTO Color(ColorId, ColorName)
	VALUES
	(1, 'White'),
	(2, 'Gray'),
	(3, 'Black'),
	(4, 'Red'),
	(5, 'Burgundy'),
	(6, 'Electric Blue'),
	(7, 'Mahogany'),
	(8, 'Ash')
	SET IDENTITY_INSERT Color OFF

	SET IDENTITY_INSERT Exterior ON
	INSERT INTO Exterior(ExteriorId, ColorId)
	VALUES
	(1, 1),
	(2, 2),
	(3, 3),
	(4, 4),
	(5, 6)
	SET IDENTITY_INSERT Exterior OFF

	SET IDENTITY_INSERT Interior ON
	INSERT INTO Interior(InteriorId, ColorId)
	VALUES
	(1, 2),
	(2, 3),
	(3, 5),
	(4, 7),
	(5, 8)
	SET IDENTITY_INSERT Interior OFF

	SET IDENTITY_INSERT ListingStatus ON
	INSERT INTO ListingStatus(ListingStatusId, ListingStatusName)
	VALUES
	(1, 'Not Featured'),
	(2, 'Featured'),
	(3, 'Purchased')
	SET IDENTITY_INSERT ListingStatus OFF

	SET IDENTITY_INSERT Vehicle ON
	INSERT INTO Vehicle(VehicleId, MakeId, ModelId, BodyStyleId, [Year], TransmissionId, ExteriorId, InteriorId, Mileage, VIN, MSRP, SalePrice, [Description], ImageFileName, ListingStatusId)
	VALUES
	(1, 1, 2, 1, 2015, 1, 3, 4, 8300, '1ABCD23EFGH456789', 10000.00, 8500.00, 'Nice, lightly-used.', 'placeholder.png', 1),
	(2, 2, 3, 2, 2010, 1, 2, 3, 40000, '901784782HBA90CNL', 5000.00, 4500.00, 'Gone through a lot.', 'placeholder.png', 2),
	(3, 3, 6, 1, 2008, 2, 1, 2, 100000, '9827834761298376A', 6500.00, 5900.00, 'This is definitely a car.', 'placeholder.png', 2),
	(4, 4, 7, 3, 2009, 1, 4, 5, 200, '16297843561293841', 25000.00, 23500.00, 'Practically new!', 'placeholder.png', 2),
	(5, 5, 9, 4, 2018, 1, 5, 1, 999, '9NOF9283O948N223D', 22300.00, 21000.00, 'Bought already.', 'placeholder.png', 3),
	(6, 2, 4, 2, 2011, 2, 2, 2, 30000, '89JFHH9085HG09438', 12400.00, 11500.00, 'Another bought one.', 'placeholder.png', 3),
	(7, 4, 7, 3, 2009, 1, 4, 5, 500, 'H3456H456H3456634', 22200.00, 21000.00, 'Almost new!', 'placeholder.png', 1),
	(8, 5, 9, 4, 2009, 1, 5, 1, 999, '9NOF9283O948N223D', 15000.00, 14000.00, 'Test.', 'placeholder.png', 2)
	SET IDENTITY_INSERT Vehicle OFF

	SET IDENTITY_INSERT Special ON
	INSERT INTO Special(SpecialId, Title, [Description])
	VALUES
	(1, 'Huge Sale!', 'It''s a blowout! You won''t want to miss these AMAZING deals.'),
	(2, 'Big Big Big!', 'BIG! Yes, you saw that right. Bigger than you''ve ever seen.'),
	(3, 'Truck Deal!', '15% OFF on select trucks! We must be going crazy!')
	SET IDENTITY_INSERT Special OFF

	SET IDENTITY_INSERT PurchaseType ON
	INSERT INTO PurchaseType(PurchaseTypeId, PurchaseTypeName)
	VALUES
	(1, 'Bank Finance'),
	(2, 'Cash'),
	(3, 'Dealer Finance')
	SET IDENTITY_INSERT PurchaseType OFF

	INSERT INTO [State](StateId)
	VALUES ('MN'),('WI'),('SD'),('ND'),('IA')

	SET IDENTITY_INSERT Sale ON
	INSERT INTO Sale(SaleId, CustomerName, Phone, Email, Address1, City, StateId, Zip, VehicleId, PurchasePrice, PurchaseTypeId, Salesperson)
	VALUES
	(1, 'Johnny Williams', '123-456-7890', 'JohnBoy@something.com', '123 Walnut Ave', 'Greensborough', 'MN', '55555', 5, 15000.00, 1, '6af6dd06-8777-4802-a72e-40a6dbdfa269'),
	(2, 'Cheryl Brown', '345-756-9812', 'test@somewhere.com', '909 Duck Rd', 'Gooseville', 'WI', '98521', 6, 13000.00, 2, '6af6dd06-8777-4802-a72e-40a6dbdfa269'),
	(3, 'Mister Guy', '111-111-1111', 'test@test.com', '909 Duck Rd', 'Gooseville', 'WI', '98521', 6, 21300.00, 3, '6af6dd06-8777-4802-a72e-40a6dbdfa269'),
	(4, 'Billy Williams', '123-456-7890', 'BillBoy@something.com', '123 Walnut Ave', 'Greensborough', 'MN', '55555', 5, 6541.00, 3, '22d05136-f0f0-4d7b-9f51-ae93ded8b5bc'),
	(5, 'Janice Brown', '345-756-9812', 'test2@somewhere.com', '909 Duck Rd', 'Gooseville', 'WI', '98521', 6, 13864.00, 2, '22d05136-f0f0-4d7b-9f51-ae93ded8b5bc'),
	(6, 'Missus Gal', '111-111-1111', 'test2@test.com', '909 Duck Rd', 'Gooseville', 'WI', '98521', 6, 12345.00, 1, '22d05136-f0f0-4d7b-9f51-ae93ded8b5bc')
	SET IDENTITY_INSERT Sale OFF

	SET IDENTITY_INSERT Contact ON
	INSERT INTO Contact(ContactId, [Name], Phone, Email, [Message], Regarding)
	VALUES
	(1, 'Bobson Dugnutt', '892-123-6521', 'baseball@homerun.com', 'Guy with a funny name here interested in that car of yours.', '9827834761298376A'),
	(2, 'Bill Slamson', '999-999-9999', 'popcorn@homerun.com', 'I need something that I can drive onto a baseball field.', '901784782HBA90CNL')
	SET IDENTITY_INSERT Contact OFF

END