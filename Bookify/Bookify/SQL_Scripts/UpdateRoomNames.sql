-- SQL Script to Update Room Names
-- Run this directly in your database if room names are showing as null

UPDATE r 
SET Name = CASE 
    WHEN rt.Name LIKE '%Luxury%' THEN 
        CASE r.RoomNumber 
            WHEN 2501 THEN 'Premium City View'
            WHEN 2502 THEN 'Executive Suite'
            WHEN 2601 THEN 'Corner View Room'
            WHEN 2602 THEN 'Royal Luxury Suite'
            WHEN 2701 THEN 'Sky High Penthouse'
            ELSE 'Luxury Room ' + CAST(r.RoomNumber AS VARCHAR(10))
        END
    WHEN rt.Name LIKE '%Family%' THEN 
        CASE r.RoomNumber
            WHEN 1205 THEN 'Spacious Family Haven'
            WHEN 1206 THEN 'Family Suite Plus'
            WHEN 1401 THEN 'Kids Paradise Room'
            WHEN 1402 THEN 'Cozy Family Retreat'
            WHEN 1501 THEN 'Grand Family Suite'
            ELSE 'Family Room ' + CAST(r.RoomNumber AS VARCHAR(10))
        END
    WHEN rt.Name LIKE '%Budget%' THEN 
        CASE r.RoomNumber
            WHEN 310 THEN 'Smart Budget Room'
            WHEN 311 THEN 'Essential Comfort'
            WHEN 405 THEN 'Artistic Budget Room'
            WHEN 406 THEN 'Twin Friends Room'
            WHEN 501 THEN 'Solo Traveler Pod'
            ELSE 'Budget Room ' + CAST(r.RoomNumber AS VARCHAR(10))
        END
    WHEN rt.Name LIKE '%Romantic%' OR rt.Name LIKE '%Sea View%' THEN 
        CASE r.RoomNumber
            WHEN 801 THEN 'Ocean Romance Suite'
            WHEN 802 THEN 'Intimate Sea View'
            WHEN 901 THEN 'Royal Honeymoon Suite'
            WHEN 902 THEN 'Plunge Pool Paradise'
            WHEN 1001 THEN 'Sunset Penthouse'
            ELSE 'Romantic Suite ' + CAST(r.RoomNumber AS VARCHAR(10))
        END
    ELSE 'Room ' + CAST(r.RoomNumber AS VARCHAR(10))
END
FROM Rooms r
INNER JOIN RoomTypes rt ON r.RoomTypeId = rt.Id
WHERE r.Name IS NULL OR r.Name = '';

-- Verify the update
SELECT r.Id, r.RoomNumber, r.Name, rt.Name as RoomTypeName
FROM Rooms r
INNER JOIN RoomTypes rt ON r.RoomTypeId = rt.Id
ORDER BY r.RoomNumber;