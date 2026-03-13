USE [HospitalDb];
GO

-- 1. Clear existing lab and report data to avoid duplicates for these patients
DELETE FROM [Labs];
DELETE FROM [PatientReports];
GO

-- 2. Variables for Doctors and Patients
DECLARE @DoctorId NVARCHAR(450) = (SELECT TOP 1 Id FROM AspNetUsers WHERE IsDoctor = 1);
DECLARE @AdmittedPatients TABLE (Id NVARCHAR(450));
DECLARE @OtherPatients TABLE (Id NVARCHAR(450));

INSERT INTO @AdmittedPatients (Id)
SELECT DISTINCT PatientId FROM RoomAllocations WHERE Status = 1;

INSERT INTO @OtherPatients (Id)
SELECT Id FROM AspNetUsers 
WHERE IsDoctor = 0 
AND Id NOT IN (SELECT Id FROM @AdmittedPatients);

-- 3. Seed Serious Data for Admitted Patients (200 Patients)
-- We insert 5 records per patient: 4 historical, 1 current
DECLARE @PatientId NVARCHAR(450);
DECLARE @i INT;
DECLARE @j INT;

DECLARE PatientCursor CURSOR FOR SELECT Id FROM @AdmittedPatients;
OPEN PatientCursor;
FETCH NEXT FROM PatientCursor INTO @PatientId;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @i = 1;
    WHILE @i <= 5
    BEGIN
        DECLARE @DaysAgo INT = (5 - @i) * 2; -- Records spread over 8 days
        DECLARE @Temp INT = 38 + (@i % 2); -- Feverish
        DECLARE @BP INT = 145 + (@i * 2); -- High BP
        DECLARE @TestValue FLOAT = 110.5 + (@i * 5); -- High value
        DECLARE @Results NVARCHAR(MAX);
        DECLARE @Diagnosis NVARCHAR(MAX);
        
        IF @i < 5
        BEGIN
            SET @Results = 'Hospitalized. Intensive treatment ongoing: IV Fluids started, Oxygen Support active. Patient condition monitored every 2 hours.';
            SET @Diagnosis = CASE WHEN @PatientId LIKE '%A%' THEN 'Acute Stroke' ELSE 'Myocardial Infarction' END;
        END
        ELSE
        BEGIN
            SET @Results = '### AI Analysis: Serious Condition. High metabolic stress detected. Action: Continue intensive monitoring and Injections (Heparin/Insulin).';
            SET @Diagnosis = CASE WHEN @PatientId LIKE '%A%' THEN 'Acute Stroke - Critical' ELSE 'Severe Cardiac Arrest' END;
        END

        INSERT INTO [Labs] (LabNumber, PatientId, TestType, TestCode, Weight, Height, BloodPressure, Temperature, TestValue, TestResults, CreatedAt)
        VALUES ('LAB-' + LEFT(CAST(NEWID() AS NVARCHAR(36)), 8), @PatientId, 'Blood Chemistry', 'BC-882', 75, 175, @BP, @Temp, @TestValue, @Results, DATEADD(DAY, -@DaysAgo, GETDATE()));

        IF @i = 5 -- Insert Report only for current status
        BEGIN
            INSERT INTO [PatientReports] (PatientId, DoctorId, Diagnose, MedicineName)
            VALUES (@PatientId, @DoctorId, @Diagnosis, 'IV Fluids, Oxygen, Injection Heparin');
        END

        SET @i = @i + 1;
    END

    FETCH NEXT FROM PatientCursor INTO @PatientId;
END

CLOSE PatientCursor;
DEALLOCATE PatientCursor;

-- 4. Seed Stable Data for Other Patients (34 Patients)
DECLARE OtherPatientCursor CURSOR FOR SELECT Id FROM @OtherPatients;
OPEN OtherPatientCursor;
FETCH NEXT FROM OtherPatientCursor INTO @PatientId;

WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @StableResults NVARCHAR(MAX) = 'Patient is stable. Vital signs within normal range. Recommended: Rest and oral medication.';
    
    INSERT INTO [Labs] (LabNumber, PatientId, TestType, TestCode, Weight, Height, BloodPressure, Temperature, TestValue, TestResults, CreatedAt)
    VALUES ('LAB-' + LEFT(CAST(NEWID() AS NVARCHAR(36)), 8), @PatientId, 'General Checkup', 'GC-101', 70, 170, 120, 37, 85.0, @StableResults, GETDATE());

    INSERT INTO [PatientReports] (PatientId, DoctorId, Diagnose, MedicineName)
    VALUES (@PatientId, @DoctorId, 'Common Viral Fever', 'Tablet Paracetamol 500mg, Multivitamins');

    FETCH NEXT FROM OtherPatientCursor INTO @PatientId;
END

CLOSE OtherPatientCursor;
DEALLOCATE OtherPatientCursor;

PRINT 'Clinical Data Seeding for 234 Patients Completed Successfully.';
GO
