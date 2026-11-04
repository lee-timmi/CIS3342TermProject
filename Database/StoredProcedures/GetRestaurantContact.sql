CREATE PROCEDURE GetRestaurantContact
	@RestaurantID INT
AS
BEGIN
	SELECT r.RestaurantID, r.RestaurantName, r.RestaurantCity, r.RestaurantState,
		u.FirstName AS RepFirstName, u.Email AS RepEmail
	FROM RestaurantTable r
	LEFT JOIN UsersTable u ON u.UserID = r.RepresentativeUserID
	WHERE r.RestaurantID = @RestaurantID;
END;
