CREATE PROCEDURE SaveReviewResponse
	@ReviewID INT,
	@RepresentativeUserID INT,
	@ResponseText VARCHAR(MAX)
AS
BEGIN
	UPDATE v
	SET ResponseText = NULLIF(@ResponseText, ''),
		ResponseDate = CASE WHEN @ResponseText = '' THEN NULL ELSE GETDATE() END
	FROM ReviewsTable v
	JOIN RestaurantTable r ON r.RestaurantID = v.RestaurantID
	WHERE v.ReviewID = @ReviewID AND r.RepresentativeUserID = @RepresentativeUserID;
END;
