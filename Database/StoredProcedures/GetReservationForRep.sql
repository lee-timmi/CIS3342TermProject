CREATE PROCEDURE GetReservationForRep
	@ReservationID INT,
	@RepresentativeUserID INT
AS
BEGIN
	SELECT v.ReservationID, v.RestaurantID, r.RestaurantName, v.ContactName, v.ContactPhone, v.ContactEmail,
		v.ReservationDateTime, v.PartySize, v.SpecialRequests, v.Status
	FROM Reservations v
	JOIN RestaurantTable r ON r.RestaurantID = v.RestaurantID
	WHERE v.ReservationID = @ReservationID AND r.RepresentativeUserID = @RepresentativeUserID;
END;
