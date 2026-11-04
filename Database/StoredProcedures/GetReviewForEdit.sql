CREATE PROCEDURE GetReviewForEdit
	@ReviewID INT,
	@ReviewUserID INT
AS
BEGIN
	SELECT v.ReviewID, v.RestaurantID, r.RestaurantName, v.ReviewText, v.FoodRating, v.ServiceRating,
		v.AtmosphereRating, v.PriceRating, v.VisitDateTime,
		CASE WHEN v.ReviewImage IS NULL THEN 0 ELSE 1 END AS HasImage
	FROM ReviewsTable v
	JOIN RestaurantTable r ON r.RestaurantID = v.RestaurantID
	WHERE v.ReviewID = @ReviewID AND v.ReviewUserID = @ReviewUserID;
END;
