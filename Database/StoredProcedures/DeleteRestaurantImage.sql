CREATE PROCEDURE DeleteRestaurantImage
	@ImageID INT,
	@RestaurantID INT
AS
BEGIN
	DELETE FROM RestaurantImages
	WHERE ImageID = @ImageID AND RestaurantID = @RestaurantID;
END;
