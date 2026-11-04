CREATE PROCEDURE AddRestaurant
	@RepresentativeUserID INT,
	@RestaurantName VARCHAR(100),
	@RestaurantAddress VARCHAR(200),
	@RestaurantCity VARCHAR(100),
	@RestaurantState VARCHAR(50),
	@RestaurantZipCode VARCHAR(20),
	@RestaurantCuisine VARCHAR(50),
	@RestaurantDescription VARCHAR(MAX),
	@RestaurantWebsite VARCHAR(200),
	@RestaurantSocialMedia VARCHAR(200),
	@RestaurantPhone VARCHAR(20),
	@RestaurantOwner VARCHAR(100)
AS
BEGIN
	IF EXISTS (SELECT 1 FROM RestaurantTable WHERE RepresentativeUserID = @RepresentativeUserID)
		RETURN 0;

	INSERT INTO RestaurantTable (RepresentativeUserID, RestaurantName, RestaurantAddress, RestaurantCity,
		RestaurantState, RestaurantZipCode, RestaurantCuisine, RestaurantDescription, RestaurantWebsite,
		RestaurantSocialMedia, RestaurantPhone, RestaurantOwner)
	VALUES (@RepresentativeUserID, @RestaurantName, @RestaurantAddress, @RestaurantCity,
		@RestaurantState, @RestaurantZipCode, @RestaurantCuisine, @RestaurantDescription, @RestaurantWebsite,
		@RestaurantSocialMedia, @RestaurantPhone, @RestaurantOwner);

	RETURN CAST(SCOPE_IDENTITY() AS INT);
END;
