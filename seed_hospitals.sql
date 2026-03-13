-- Hospital Migration and Seed Script
USE HospitalDb;
GO

-- 1. Add Services column if it doesn't exist
IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.HospitalInfos') AND name = 'Services')
BEGIN
    ALTER TABLE [dbo].[HospitalInfos] ADD [Services] NVARCHAR(MAX) NULL;
    PRINT 'Added Services column to HospitalInfos.';
END
GO

-- 2. Record migration in EF history
IF NOT EXISTS (SELECT 1 FROM [dbo].[__EFMigrationsHistory] WHERE MigrationId = '20260311152000_AddHospitalServicesField')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] (MigrationId, ProductVersion)
    VALUES ('20260311152000_AddHospitalServicesField', '8.0.22');
    PRINT 'Updated EFMigrationsHistory.';
END
GO

-- 3. Bulk Seed 40 Hospital Records
-- Only seed if the table is relatively empty or specific seed record 101 is missing
IF NOT EXISTS (SELECT 1 FROM HospitalInfos WHERE Id = 101)
BEGIN
    SET IDENTITY_INSERT HospitalInfos ON;

    INSERT INTO HospitalInfos (Id, Name, Type, City, Pincode, Country, PhoneNumber, EmailAddress, WebsiteUrl, OperatingHours, RegistrationNumber, StreetAddress, LogoUrl, Services)
    VALUES
    (101, 'Mercy General Hospital', 'General', 'New York', '10001', 'USA', '+1 (212) 555-0101', 'info@mercygeneral.com', 'https://www.mercygeneral.com', 'Mon-Sun: 24/7', 'REG-100001', '300 East 66th Street, New York, NY 10001', NULL, 'Emergencies'),
    (102, 'St. Lukes Medical Center', 'Teaching', 'Chicago', '60601', 'USA', '+1 (312) 555-0102', 'contact@stlukesmed.com', 'https://www.stlukesmed.com', 'Mon-Sun: 24/7', 'REG-100002', '1725 W Harrison St, Chicago, IL 60612', NULL, 'Disease research'),
    (103, 'Sunrise Specialty Hospital', 'Specialty', 'Los Angeles', '90001', 'USA', '+1 (213) 555-0103', 'hello@sunrisespecialty.com', 'https://www.sunrisespecialty.com', 'Mon-Fri: 6AM-10PM', 'REG-100003', '5800 Sunset Blvd, Los Angeles, CA 90028', NULL, 'Cancer treatment'),
    (104, 'Valley Rehabilitation Center', 'Rehabilitation', 'Phoenix', '85001', 'USA', '+1 (602) 555-0104', 'support@valleyrehab.com', 'https://www.valleyrehab.com', 'Mon-Sat: 7AM-9PM', 'REG-100004', '1850 N Central Ave, Phoenix, AZ 85004', NULL, 'Recovery programs'),
    (105, 'Lakeside General Hospital', 'General', 'Houston', '77001', 'USA', '+1 (713) 555-0105', 'info@lakesidegeneral.com', 'https://www.lakesidegeneral.com', 'Mon-Sun: 24/7', 'REG-100005', '6550 Fannin St, Houston, TX 77030', NULL, 'Common surgeries'),
    (106, 'Greenfield Specialty Clinic', 'Specialty', 'Philadelphia', '19101', 'USA', '+1 (215) 555-0106', 'info@greenfieldspe.com', 'https://www.greenfieldspe.com', 'Mon-Fri: 8AM-6PM', 'REG-100006', '212 Broad St, Philadelphia, PA 19102', NULL, 'Pediatrics'),
    (107, 'Pinnacle Teaching Hospital', 'Teaching', 'San Antonio', '78201', 'USA', '+1 (210) 555-0107', 'contact@pinnacleteach.com', 'https://www.pinnacleteach.com', 'Mon-Sun: 24/7', 'REG-100007', '4502 Medical Dr, San Antonio, TX 78229', NULL, 'New treatment trials'),
    (108, 'Harbor View Medical Center', 'General', 'San Diego', '92101', 'USA', '+1 (619) 555-0108', 'admin@harborviewmc.com', 'https://www.harborviewmc.com', 'Mon-Sun: 24/7', 'REG-100008', '140 Arbor Dr, San Diego, CA 92103', NULL, 'Emergencies'),
    (109, 'Mountain Recovery Institute', 'Rehabilitation', 'Dallas', '75201', 'USA', '+1 (214) 555-0109', 'info@mountainrecovery.com', 'https://www.mountainrecovery.com', 'Mon-Sat: 7AM-8PM', 'REG-100009', '8200 Walnut Hill Ln, Dallas, TX 75231', NULL, 'Long-term physical therapy'),
    (110, 'Northside Mental Health', 'Specialty', 'Jacksonville', '32201', 'USA', '+1 (904) 555-0110', 'care@northsidemh.com', 'https://www.northsidemh.com', 'Mon-Fri: 9AM-5PM', 'REG-100010', '655 W 8th St, Jacksonville, FL 32209', NULL, 'Mental health'),
    (111, 'Capital General Hospital', 'General', 'Austin', '78701', 'USA', '+1 (512) 555-0111', 'info@capitalgeneral.com', 'https://www.capitalgeneral.com', 'Mon-Sun: 24/7', 'REG-100011', '1201 W 38th St, Austin, TX 78705', NULL, 'Emergencies'),
    (112, 'Westbrook University Hospital', 'Teaching', 'Columbus', '43201', 'USA', '+1 (614) 555-0112', 'inquiries@westbrookuh.com', 'https://www.westbrookuh.com', 'Mon-Sun: 24/7', 'REG-100012', '410 W 10th Ave, Columbus, OH 43210', NULL, 'Disease research'),
    (113, 'Childrens Specialty Center', 'Specialty', 'Charlotte', '28201', 'USA', '+1 (704) 555-0113', 'kids@childrensspecialty.com', 'https://www.childrensspecialty.com', 'Mon-Fri: 7AM-9PM', 'REG-100013', '100 Blythe Blvd, Charlotte, NC 28203', NULL, 'Pediatrics'),
    (114, 'Bay Area Rehab Hospital', 'Rehabilitation', 'San Francisco', '94102', 'USA', '+1 (415) 555-0114', 'info@bayarearehab.com', 'https://www.bayarearehab.com', 'Mon-Sat: 8AM-6PM', 'REG-100014', '1001 Potrero Ave, San Francisco, CA 94110', NULL, 'Recovery programs'),
    (115, 'Eastside Community Hospital', 'General', 'Indianapolis', '46201', 'USA', '+1 (317) 555-0115', 'contact@eastsidech.com', 'https://www.eastsidech.com', 'Mon-Sun: 24/7', 'REG-100015', '1701 N Senate Ave, Indianapolis, IN 46202', NULL, 'Common surgeries'),
    (116, 'Coastal Oncology Specialty', 'Specialty', 'Seattle', '98101', 'USA', '+1 (206) 555-0116', 'oncology@coastalspec.com', 'https://www.coastalspec.com', 'Mon-Fri: 8AM-5PM', 'REG-100016', '325 9th Ave, Seattle, WA 98104', NULL, 'Cancer treatment'),
    (117, 'Metro Teaching Medical', 'Teaching', 'Denver', '80201', 'USA', '+1 (303) 555-0117', 'info@metroteaching.com', 'https://www.metroteaching.com', 'Mon-Sun: 24/7', 'REG-100017', '777 Bannock St, Denver, CO 80204', NULL, 'New treatment trials'),
    (118, 'Southern Cross Hospital', 'General', 'Nashville', '37201', 'USA', '+1 (615) 555-0118', 'admin@southerncrossh.com', 'https://www.southerncrossh.com', 'Mon-Sun: 24/7', 'REG-100018', '1161 21st Ave S, Nashville, TN 37232', NULL, 'Emergencies'),
    (119, 'Sunrise Physical Therapy', 'Rehabilitation', 'Baltimore', '21201', 'USA', '+1 (410) 555-0119', 'info@sunrisetherapy.com', 'https://www.sunrisetherapy.com', 'Mon-Sat: 7AM-7PM', 'REG-100019', '22 S Green St, Baltimore, MD 21201', NULL, 'Long-term physical therapy'),
    (120, 'Pediatric Specialty Hospital', 'Specialty', 'Louisville', '40201', 'USA', '+1 (502) 555-0120', 'kids@pediatricsh.com', 'https://www.pediatricsh.com', 'Mon-Fri: 7AM-8PM', 'REG-100020', '200 Abraham Flexner Way, Louisville, KY 40202', NULL, 'Pediatrics'),
    (121, 'Riverside General Medical', 'General', 'Portland', '97201', 'USA', '+1 (503) 555-0121', 'info@riversidegm.com', 'https://www.riversidegm.com', 'Mon-Sun: 24/7', 'REG-100021', '3181 SW Sam Jackson Park Rd, Portland, OR 97239', NULL, 'Common surgeries'),
    (122, 'Central University Clinic', 'Teaching', 'Las Vegas', '89101', 'USA', '+1 (702) 555-0122', 'contact@centraluc.com', 'https://www.centraluc.com', 'Mon-Sun: 24/7', 'REG-100022', '1800 W Charleston Blvd, Las Vegas, NV 89102', NULL, 'Disease research'),
    (123, 'Atlantic Cancer Center', 'Specialty', 'Memphis', '38101', 'USA', '+1 (901) 555-0123', 'info@atlanticcancer.com', 'https://www.atlanticcancer.com', 'Mon-Fri: 8AM-6PM', 'REG-101023', '1265 Union Ave, Memphis, TN 38104', NULL, 'Cancer treatment'),
    (124, 'Parkside Rehabilitation', 'Rehabilitation', 'Albuquerque', '87101', 'USA', '+1 (505) 555-0124', 'care@parksiderehab.com', 'https://www.parksiderehab.com', 'Mon-Sat: 8AM-7PM', 'REG-101024', '2211 Lomas Blvd NE, Albuquerque, NM 87106', NULL, 'Recovery programs'),
    (125, 'Lakeview General Hospital', 'General', 'Tucson', '85701', 'USA', '+1 (520) 555-0125', 'info@lakeviewgh.com', 'https://www.lakeviewgh.com', 'Mon-Sun: 24/7', 'REG-101025', '1501 N Campbell Ave, Tucson, AZ 85724', NULL, 'Emergencies'),
    (126, 'Horizon Mental Health Clinic', 'Specialty', 'Fresno', '93701', 'USA', '+1 (559) 555-0126', 'support@horizonmhc.com', 'https://www.horizonmhc.com', 'Mon-Fri: 9AM-5PM', 'REG-101026', '155 Fresno St, Fresno, CA 93721', NULL, 'Mental health'),
    (127, 'Pacific Research Hospital', 'Teaching', 'Sacramento', '95814', 'USA', '+1 (916) 555-0127', 'research@pacifichosp.com', 'https://www.pacifichosp.com', 'Mon-Sun: 24/7', 'REG-101027', '2315 Stockton Blvd, Sacramento, CA 95817', NULL, 'New treatment trials'),
    (128, 'Uptown General Hospital', 'General', 'Kansas City', '64101', 'USA', '+1 (816) 555-0128', 'admin@uptowngeneral.com', 'https://www.uptowngeneral.com', 'Mon-Sun: 24/7', 'REG-101028', '2301 Holmes St, Kansas City, MO 64108', NULL, 'Common surgeries'),
    (129, 'Harmony Recovery Center', 'Rehabilitation', 'Mesa', '85201', 'USA', '+1 (480) 555-0129', 'info@harmonyrecovery.com', 'https://www.harmonyrecovery.com', 'Mon-Sat: 7AM-8PM', 'REG-101029', '525 N Dobson Rd, Mesa, AZ 85201', NULL, 'Long-term physical therapy'),
    (130, 'Sunbelt Specialty Hospital', 'Specialty', 'Atlanta', '30301', 'USA', '+1 (404) 555-0130', 'contact@sunbeltsh.com', 'https://www.sunbeltsh.com', 'Mon-Fri: 7AM-8PM', 'REG-101030', '550 Peachtree St NE, Atlanta, GA 30308', NULL, 'Cancer treatment'),
    (131, 'Maplewood Community Hospital', 'General', 'Omaha', '68101', 'USA', '+1 (402) 555-0131', 'info@maplewoodch.com', 'https://www.maplewoodch.com', 'Mon-Sun: 24/7', 'REG-101031', '7500 Mercy Rd, Omaha, NE 68124', NULL, 'Emergencies'),
    (132, 'Grand Valley Teaching Hospital', 'Teaching', 'Colorado Springs', '80901', 'USA', '+1 (719) 555-0132', 'contact@grandvalleyth.com', 'https://www.grandvalleyth.com', 'Mon-Sun: 24/7', 'REG-101032', '1400 E Boulder St, Colorado Springs, CO 80909', NULL, 'Disease research'),
    (133, 'Desert Pediatric Center', 'Specialty', 'Raleigh', '27601', 'USA', '+1 (919) 555-0133', 'hello@desertpediatric.com', 'https://www.desertpediatric.com', 'Mon-Fri: 8AM-6PM', 'REG-101033', '3000 New Bern Ave, Raleigh, NC 27610', NULL, 'Pediatrics'),
    (134, 'Green Valley Rehab Institute', 'Rehabilitation', 'Long Beach', '90801', 'USA', '+1 (562) 555-0134', 'info@greenvalleyri.com', 'https://www.greenvalleyri.com', 'Mon-Sat: 8AM-8PM', 'REG-101034', '2776 Pacific Ave, Long Beach, CA 90806', NULL, 'Recovery programs'),
    (135, 'Bayfront General Hospital', 'General', 'Minneapolis', '55401', 'USA', '+1 (612) 555-0135', 'admin@bayfrontgh.com', 'https://www.bayfrontgh.com', 'Mon-Sun: 24/7', 'REG-101035', '701 Park Ave, Minneapolis, MN 55415', NULL, 'Common surgeries'),
    (136, 'Eastern Oncology Specialists', 'Specialty', 'Tampa', '33601', 'USA', '+1 (813) 555-0136', 'care@easternoncology.com', 'https://www.easternoncology.com', 'Mon-Fri: 8AM-5PM', 'REG-101036', '1 Tampa General Cir, Tampa, FL 33606', NULL, 'Cancer treatment'),
    (137, 'Capital City University Med', 'Teaching', 'New Orleans', '70112', 'USA', '+1 (504) 555-0137', 'contact@capitalcitymed.com', 'https://www.capitalcitymed.com', 'Mon-Sun: 24/7', 'REG-101037', '2021 Perdido St, New Orleans, LA 70112', NULL, 'New treatment trials'),
    (138, 'Northgate General Hospital', 'General', 'Arlington', '76001', 'USA', '+1 (817) 555-0138', 'info@northgategh.com', 'https://www.northgategh.com', 'Mon-Sun: 24/7', 'REG-101038', '800 W Randol Mill Rd, Arlington, TX 76012', NULL, 'Emergencies'),
    (139, 'Clearwater Physical Institute', 'Rehabilitation', 'Bakersfield', '93301', 'USA', '+1 (661) 555-0139', 'info@clearwaterpi.com', 'https://www.clearwaterpi.com', 'Mon-Sat: 7AM-7PM', 'REG-101039', '2615 Eye St, Bakersfield, CA 93301', NULL, 'Long-term physical therapy'),
    (140, 'Midtown Mental Wellness', 'Specialty', 'Aurora', '80010', 'USA', '+1 (720) 555-0140', 'wellness@midtownmw.com', 'https://www.midtownmw.com', 'Mon-Fri: 9AM-6PM', 'REG-101040', '1375 E Colfax Ave, Aurora, CO 80010', NULL, 'Mental health');

    SET IDENTITY_INSERT HospitalInfos OFF;
    PRINT 'Seeded 40 hospitals successfully.';
END
GO
