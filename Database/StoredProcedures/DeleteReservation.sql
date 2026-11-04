CREATE PROCEDURE DeleteReservation
	@ReservationID INT,
	@RepresentativeUserID INT
AS
BEGIN
	DELETE v
	FROM Reservations v
	JOIN RestaurantTable r ON r.RestaurantID = v.RestaurantID
	WHERE v.ReservationID = @ReservationID AND r.RepresentativeUserID = @RepresentativeUserID;
END;
