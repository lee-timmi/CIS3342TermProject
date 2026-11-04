CREATE PROCEDURE UpdateRestaurant
	@RestaurantID INT,
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
	UPDATE RestaurantTable
	SET RestaurantName = @RestaurantName,
		RestaurantAddress = @RestaurantAddress,
		RestaurantCity = @RestaurantCity,
		RestaurantState = @RestaurantState,
		RestaurantZipCode = @RestaurantZipCode,
		RestaurantCuisine = @RestaurantCuisine,
		RestaurantDescription = @RestaurantDescription,
		RestaurantWebsite = @RestaurantWebsite,
		RestaurantSocialMedia = @RestaurantSocialMedia,
		RestaurantPhone = @RestaurantPhone,
		RestaurantOwner = @RestaurantOwner
	WHERE RestaurantID = @RestaurantID
	  AND RepresentativeUserID = @RepresentativeUserID;
END;
