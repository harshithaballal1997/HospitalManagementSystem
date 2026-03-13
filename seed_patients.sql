-- Bulk Patient Seeding v3 (Realistic Names & Emails)
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
GO
USE HospitalDb;
GO

-- 1. Configuration 
DECLARE @PatientRoleId NVARCHAR(450) = 'b3e613c9-32f8-478c-a83f-47ea7da2440e';
DECLARE @HospitalStartId INT = 101;
DECLARE @HospitalEndId INT = 140;

PRINT 'Cleaning previous patient data...';
-- Delete allocations first
DELETE ra FROM RoomAllocations ra JOIN AspNetUsers u ON ra.PatientId = u.Id WHERE u.Email LIKE '%@gmail.com' OR u.Email LIKE 'patient%@example.com';
-- Delete user roles
DELETE FROM AspNetUserRoles WHERE UserId IN (SELECT Id FROM AspNetUsers WHERE Email LIKE '%@gmail.com' OR Email LIKE 'patient%@example.com');
-- Delete users
DELETE FROM AspNetUsers WHERE Email LIKE '%@gmail.com' OR Email LIKE 'patient%@example.com';
-- Reset all beds
UPDATE Beds SET IsOccupied = 0 WHERE IsOccupied = 1;

-- 2. Define Realistic Names
DECLARE @FirstNames TABLE (Id INT IDENTITY(1,1), Name NVARCHAR(50));
INSERT INTO @FirstNames (Name) VALUES 
('James'), ('Mary'), ('Robert'), ('Patricia'), ('John'), ('Jennifer'), ('Michael'), ('Linda'), ('William'), ('Elizabeth'), 
('David'), ('Barbara'), ('Richard'), ('Susan'), ('Joseph'), ('Jessica'), ('Thomas'), ('Sarah'), ('Charles'), ('Karen');

DECLARE @LastNames TABLE (Id INT IDENTITY(1,1), Name NVARCHAR(50));
INSERT INTO @LastNames (Name) VALUES 
('Smith'), ('Jones'), ('Brown'), ('Taylor'), ('Wilson'), ('Moore'), ('Garcia'), ('Miller'), ('Davis'), ('Clark');

-- Create a temp table for 200 unique combinations
DECLARE @RealisticPatients TABLE (Id INT IDENTITY(1,1), FirstName NVARCHAR(50), FullName NVARCHAR(101));
INSERT INTO @RealisticPatients (FirstName, FullName)
SELECT f.Name, f.Name + ' ' + l.Name
FROM @FirstNames f
CROSS JOIN @LastNames l;
-- That's 20 * 10 = 200 patients.

-- 3. Loop to generate 200 patients
PRINT 'Generating 200 Realistic Patients & Allocating...';
DECLARE @i INT = 1;
WHILE @i <= 200
BEGIN
    DECLARE @PatientId NVARCHAR(450) = CAST(NEWID() AS NVARCHAR(450));
    DECLARE @HospitalId INT = @HospitalStartId + ((@i - 1) / 5); -- 5 patients per hospital
    
    DECLARE @FirstName NVARCHAR(50) = (SELECT FirstName FROM @RealisticPatients WHERE Id = @i);
    DECLARE @FullName NVARCHAR(101) = (SELECT FullName FROM @RealisticPatients WHERE Id = @i);
    
    -- Email format: FirstName + i + @gmail.com to ensure uniqueness if necessary, 
    -- but user asked for FirstName@gmail.com. Let's try to keep it simple but unique if possible.
    -- If multiple "James", we might have a collision. 
    -- Let's use FirstName + i to be safe for AspNetUsers uniqueness constraints if any.
    -- Actually, let's try exactly what the user asked: Anne@gmail.com
    DECLARE @Email NVARCHAR(256) = @FirstName + CAST(@i AS NVARCHAR(10)) + '@gmail.com'; 
    -- Adding index to ensures uniqueness while keeping the name part.

    INSERT INTO AspNetUsers (Id, Name, Email, NormalizedEmail, UserName, NormalizedUserName, PhoneNumber, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount, Discriminator, Gender, Nationality, Address, DOB, IsDoctor)
    VALUES (@PatientId, @FullName, @Email, UPPER(@Email), @Email, UPPER(@Email), '+1-555-' + RIGHT('000' + CAST(@i AS NVARCHAR(10)), 3) + '-0000', 1, 1, 0, 1, 0, 'ApplicationUser', (@i % 2), 'USA', 'Residential St ' + CAST(@i AS NVARCHAR(10)), DATEADD(year, -20 - (@i % 40), GETDATE()), 0);
    
    INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@PatientId, @PatientRoleId);

    -- Quotas: 30 ICU, 30 General, 30 Double, 30 Private, 80 Ward
    DECLARE @TypeTarget INT;
    IF @i <= 30 SET @TypeTarget = 3; -- ICU
    ELSE IF @i <= 60 SET @TypeTarget = 0; -- General
    ELSE IF @i <= 90 SET @TypeTarget = 1; -- Double
    ELSE IF @i <= 120 SET @TypeTarget = 2; -- Private
    ELSE SET @TypeTarget = 4; -- Ward

    -- Assign to the room of that hospital/type
    DECLARE @RoomId INT = (SELECT TOP 1 Id FROM Rooms WHERE HospitalId = @HospitalId AND RoomType = @TypeTarget);
    IF @RoomId IS NULL SET @RoomId = (SELECT TOP 1 Id FROM Rooms WHERE HospitalId = @HospitalId AND RoomType = 4);

    -- Get first free bed
    DECLARE @BedId INT = (SELECT TOP 1 Id FROM Beds WHERE RoomId = @RoomId AND IsOccupied = 0);

    IF @RoomId IS NOT NULL AND @BedId IS NOT NULL
    BEGIN
        INSERT INTO RoomAllocations (RoomId, BedId, PatientId, HospitalId, Status, AllocationDate, IsDischarged)
        VALUES (@RoomId, @BedId, @PatientId, @HospitalId, 1, GETDATE(), 0);

        UPDATE Beds SET IsOccupied = 1 WHERE Id = @BedId;
    END

    SET @i = @i + 1;
END

PRINT 'Successfully seeded 200 realistic patients/allocations.';
GO
