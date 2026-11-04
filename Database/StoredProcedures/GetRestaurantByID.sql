CREATE PROCEDURE GetRestaurantByID
	@RestaurantID INT
AS
BEGIN
	SELECT *
	FROM RestaurantTable
	WHERE RestaurantID = @RestaurantID;
END;
