CREATE PROCEDURE SaveRestaurantHours
	@RestaurantID INT,
	@DayOfWeek TINYINT,
	@IsClosed BIT,
	@OpenTime TIME,
	@CloseTime TIME
AS
BEGIN
	IF EXISTS (SELECT 1 FROM RestaurantHours WHERE RestaurantID = @RestaurantID AND DayOfWeek = @DayOfWeek)
		UPDATE RestaurantHours
		SET IsClosed = @IsClosed, OpenTime = @OpenTime, CloseTime = @CloseTime
		WHERE RestaurantID = @RestaurantID AND DayOfWeek = @DayOfWeek;
	ELSE
		INSERT INTO RestaurantHours (RestaurantID, DayOfWeek, IsClosed, OpenTime, CloseTime)
		VALUES (@RestaurantID, @DayOfWeek, @IsClosed, @OpenTime, @CloseTime);
END;
