CREATE PROCEDURE GetReviewsByUserID
	@ReviewUserID INT
AS
BEGIN
	SELECT v.ReviewID, v.RestaurantID, r.RestaurantName, v.ReviewText, v.FoodRating, v.ServiceRating,
		v.AtmosphereRating, v.PriceRating, v.VisitDateTime, v.ReviewDate, v.ResponseText, v.ResponseDate,
		CASE WHEN v.ReviewImage IS NULL THEN 0 ELSE 1 END AS HasImage
	FROM ReviewsTable v
	JOIN RestaurantTable r ON r.RestaurantID = v.RestaurantID
	WHERE v.ReviewUserID = @ReviewUserID
	ORDER BY v.ReviewDate DESC;
END;
