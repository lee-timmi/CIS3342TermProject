CREATE PROCEDURE GetRestaurantRatings
	@RestaurantID INT
AS
BEGIN
	SELECT COUNT(*) AS ReviewCount,
		AVG(CAST(FoodRating AS FLOAT)) AS AvgFood,
		AVG(CAST(ServiceRating AS FLOAT)) AS AvgService,
		AVG(CAST(AtmosphereRating AS FLOAT)) AS AvgAtmosphere,
		AVG(CAST(PriceRating AS FLOAT)) AS AvgPrice,
		AVG(CASE WHEN AtmosphereRating IS NULL
			THEN (FoodRating + ServiceRating) / 2.0
			ELSE (FoodRating + ServiceRating + AtmosphereRating) / 3.0 END) AS AvgOverall
	FROM ReviewsTable
	WHERE RestaurantID = @RestaurantID;
END;
