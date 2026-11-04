CREATE PROCEDURE GetReviewsForRep
	@RepresentativeUserID INT
AS
BEGIN
	SELECT v.ReviewID, v.ReviewerName, v.ReviewText, v.FoodRating, v.ServiceRating, v.AtmosphereRating,
		v.PriceRating, v.ReviewDate, v.VisitDateTime, v.ResponseText, v.ResponseDate,
		CASE WHEN v.ReviewImage IS NULL THEN 0 ELSE 1 END AS HasImage
	FROM ReviewsTable v
	JOIN RestaurantTable r ON r.RestaurantID = v.RestaurantID
	WHERE r.RepresentativeUserID = @RepresentativeUserID
	ORDER BY CASE WHEN v.ResponseText IS NULL THEN 0 ELSE 1 END, v.ReviewDate DESC;
END;
