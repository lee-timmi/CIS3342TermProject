CREATE PROCEDURE GetReservationsForRep
	@RepresentativeUserID INT
AS
BEGIN
	SELECT v.ReservationID, v.ContactName, v.ContactPhone, v.ContactEmail, v.ReservationDateTime,
		v.PartySize, v.SpecialRequests, v.Status, v.CreatedDate,
		CASE WHEN v.ReservationDateTime >= GETDATE() THEN 1 ELSE 0 END AS IsUpcoming
	FROM Reservations v
	JOIN RestaurantTable r ON r.RestaurantID = v.RestaurantID
	WHERE r.RepresentativeUserID = @RepresentativeUserID
	ORDER BY CASE WHEN v.ReservationDateTime >= GETDATE() THEN 0 ELSE 1 END,
		CASE WHEN v.ReservationDateTime >= GETDATE() THEN v.ReservationDateTime END ASC,
		v.ReservationDateTime DESC;
END;
