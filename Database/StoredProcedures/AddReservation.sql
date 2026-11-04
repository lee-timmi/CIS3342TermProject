CREATE PROCEDURE AddReservation
	@RestaurantID INT,
	@UserID INT,
	@ContactName VARCHAR(100),
	@ContactPhone VARCHAR(20),
	@ContactEmail VARCHAR(100),
	@ReservationDateTime DATETIME,
	@PartySize INT,
	@SpecialRequests VARCHAR(500)
AS
BEGIN
	INSERT INTO Reservations (RestaurantID, UserID, ContactName, ContactPhone, ContactEmail,
		ReservationDateTime, PartySize, SpecialRequests)
	VALUES (@RestaurantID, @UserID, @ContactName, @ContactPhone, @ContactEmail,
		@ReservationDateTime, @PartySize, @SpecialRequests);

	RETURN CAST(SCOPE_IDENTITY() AS INT);
END;
