CREATE PROCEDURE GetCuisines
AS
BEGIN
	SELECT DISTINCT RestaurantCuisine
	FROM RestaurantTable
	ORDER BY RestaurantCuisine;
END;
