CREATE PROCEDURE GetRestaurantHours
	@RestaurantID INT
AS
BEGIN
	SELECT DayOfWeek, IsClosed,
		CONVERT(VARCHAR(5), OpenTime, 108) AS OpenTime,
		CONVERT(VARCHAR(5), CloseTime, 108) AS CloseTime
	FROM RestaurantHours
	WHERE RestaurantID = @RestaurantID
	ORDER BY DayOfWeek;
END;
