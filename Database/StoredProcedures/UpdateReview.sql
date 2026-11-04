CREATE PROCEDURE UpdateReview
	@ReviewID INT,
	@ReviewUserID INT,
	@ReviewText VARCHAR(MAX),
	@FoodRating INT,
	@ServiceRating INT,
	@AtmosphereRating INT,
	@PriceRating INT,
	@VisitDateTime DATETIME,
	@ReviewImage VARBINARY(MAX),
	@ReviewImageType VARCHAR(50),
	@RemovePhoto BIT
AS
BEGIN
	UPDATE ReviewsTable
	SET ReviewText = @ReviewText,
		FoodRating = @FoodRating,
		ServiceRating = @ServiceRating,
		AtmosphereRating = @AtmosphereRating,
		PriceRating = @PriceRating,
		VisitDateTime = @VisitDateTime,
		ReviewImage = CASE WHEN @ReviewImage IS NOT NULL THEN @ReviewImage
		                   WHEN @RemovePhoto = 1 THEN NULL
		                   ELSE ReviewImage END,
		ReviewImageType = CASE WHEN @ReviewImage IS NOT NULL THEN @ReviewImageType
		                       WHEN @RemovePhoto = 1 THEN NULL
		                       ELSE ReviewImageType END
	WHERE ReviewID = @ReviewID
	  AND ReviewUserID = @ReviewUserID;
END;
