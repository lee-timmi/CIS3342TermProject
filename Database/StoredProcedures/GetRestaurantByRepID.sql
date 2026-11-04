CREATE PROCEDURE GetRestaurantByRepID
	@RepresentativeUserID INT
AS
BEGIN
	SELECT *
	FROM RestaurantTable
	WHERE RepresentativeUserID = @RepresentativeUserID;
END;
