CREATE PROCEDURE GetTopRatedRestaurants
	@City VARCHAR(100),
	@State VARCHAR(50),
	@Count INT
AS
BEGIN
	SELECT TOP (@Count) *
	FROM (SELECT r.RestaurantID, r.RestaurantName, r.RestaurantCuisine, r.RestaurantImage,
			r.RestaurantCity, r.RestaurantState,
			(SELECT TOP 1 ImageID FROM RestaurantImages
			 WHERE RestaurantID = r.RestaurantID AND ImageCategory = 'Logo') AS LogoImageID,
			(SELECT COUNT(*) FROM ReviewsTable WHERE RestaurantID = r.RestaurantID) AS ReviewCount,
			(SELECT AVG(CAST(FoodRating AS FLOAT)) FROM ReviewsTable WHERE RestaurantID = r.RestaurantID) AS AvgFood,
			(SELECT AVG(CAST(ServiceRating AS FLOAT)) FROM ReviewsTable WHERE RestaurantID = r.RestaurantID) AS AvgService,
			(SELECT AVG(CAST(PriceRating AS FLOAT)) FROM ReviewsTable WHERE RestaurantID = r.RestaurantID) AS AvgPrice,
			(SELECT AVG(CASE WHEN AtmosphereRating IS NULL
						THEN (FoodRating + ServiceRating) / 2.0
						ELSE (FoodRating + ServiceRating + AtmosphereRating) / 3.0 END)
			 FROM ReviewsTable WHERE RestaurantID = r.RestaurantID) AS AvgOverall
		FROM dbo.RestaurantTable r
		WHERE r.RestaurantCity = @City AND r.RestaurantState = @State) AS rated
	WHERE rated.ReviewCount > 0
	ORDER BY rated.AvgOverall DESC, rated.ReviewCount DESC, rated.RestaurantName;
END
