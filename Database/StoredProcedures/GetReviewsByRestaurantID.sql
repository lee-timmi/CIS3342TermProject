CREATE PROCEDURE GetReviewsByRestaurantID
	@RestaurantID INT
AS
BEGIN
	SELECT ReviewID, RestaurantID, ReviewerName, ReviewText, FoodRating, ServiceRating, AtmosphereRating,
		PriceRating, ReviewDate, VisitDateTime, HelpfulCount, NotHelpfulCount, ResponseText, ResponseDate,
		CASE WHEN ReviewImage IS NULL THEN 0 ELSE 1 END AS HasImage
	FROM ReviewsTable
	WHERE RestaurantID = @RestaurantID
	ORDER BY ReviewDate DESC;
END
