CREATE PROCEDURE SearchRestaurants
	@Cuisines VARCHAR(MAX),
	@Location VARCHAR(100)
AS
BEGIN
	DECLARE @CityState VARCHAR(300) = LTRIM(RTRIM(REPLACE(REPLACE(REPLACE(REPLACE(@Location, ',', ' '), ' ', '<>'), '><', ''), '<>', ' ')));

	SELECT r.RestaurantID, r.RestaurantName, r.RestaurantCuisine, r.RestaurantImage,
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
	WHERE (@Cuisines = ''
			OR r.RestaurantCuisine IN (SELECT value FROM STRING_SPLIT(@Cuisines, ',')))
		AND (@Location = ''
			OR r.RestaurantCity LIKE '%' + @Location + '%'
			OR r.RestaurantState LIKE '%' + @Location + '%'
			OR (r.RestaurantCity + ' ' + r.RestaurantState) LIKE '%' + @CityState + '%')
	ORDER BY r.RestaurantName;
END
