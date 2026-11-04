CREATE PROCEDURE AddReview
	@RestaurantID INT,
	@ReviewUserID INT,
	@ReviewText VARCHAR(MAX),
	@FoodRating INT,
	@ServiceRating INT,
	@AtmosphereRating INT,
	@PriceRating INT,
	@VisitDateTime DATETIME,
	@ReviewImage VARBINARY(MAX),
	@ReviewImageType VARCHAR(50)
AS
BEGIN
	INSERT INTO ReviewsTable (RestaurantID, ReviewUserID, ReviewerName, ReviewText, FoodRating, ServiceRating,
		AtmosphereRating, PriceRating, VisitDateTime, ReviewImage, ReviewImageType, ReviewDate)
	SELECT @RestaurantID, @ReviewUserID, FirstName + ' ' + LEFT(LastName, 1) + '.', @ReviewText, @FoodRating, @ServiceRating,
		@AtmosphereRating, @PriceRating, @VisitDateTime, @ReviewImage, @ReviewImageType, GETDATE()
	FROM UsersTable
	WHERE UserID = @ReviewUserID
		AND (UserType = 'Reviewer'
			OR (UserType = 'Representative'
				AND NOT EXISTS (SELECT 1 FROM RestaurantTable
					WHERE RestaurantID = @RestaurantID AND RepresentativeUserID = @ReviewUserID)));
END;
