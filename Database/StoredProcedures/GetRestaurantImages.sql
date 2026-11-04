CREATE PROCEDURE GetRestaurantImages
	@RestaurantID INT
AS
BEGIN
	SELECT ImageID, Caption, ImageCategory, UploadDate
	FROM RestaurantImages
	WHERE RestaurantID = @RestaurantID
	ORDER BY CASE ImageCategory WHEN 'Logo' THEN 1 WHEN 'Profile' THEN 2 ELSE 3 END, UploadDate;
END;
