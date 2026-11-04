CREATE PROCEDURE UpdateReservation
	@ReservationID INT,
	@RepresentativeUserID INT,
	@ReservationDateTime DATETIME,
	@PartySize INT,
	@SpecialRequests VARCHAR(500),
	@Status VARCHAR(20)
AS
BEGIN
	UPDATE v
	SET ReservationDateTime = @ReservationDateTime,
		PartySize = @PartySize,
		SpecialRequests = @SpecialRequests,
		Status = @Status
	FROM Reservations v
	JOIN RestaurantTable r ON r.RestaurantID = v.RestaurantID
	WHERE v.ReservationID = @ReservationID AND r.RepresentativeUserID = @RepresentativeUserID;
END;
