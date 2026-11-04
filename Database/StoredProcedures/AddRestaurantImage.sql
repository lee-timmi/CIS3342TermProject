CREATE PROCEDURE AddRestaurantImage
	@RestaurantID INT,
	@ImageData VARBINARY(MAX),
	@ImageContentType VARCHAR(50),
	@Caption VARCHAR(200),
	@ImageCategory VARCHAR(20)
AS
BEGIN TRANSACTION
	IF @ImageCategory IN ('Logo', 'Profile')
		DELETE FROM RestaurantImages
		WHERE RestaurantID = @RestaurantID AND ImageCategory = @ImageCategory;

	INSERT INTO RestaurantImages (RestaurantID, ImageData, ImageContentType, Caption, ImageCategory, UploadDate)
	VALUES (@RestaurantID, @ImageData, @ImageContentType, @Caption, @ImageCategory, GETDATE());
	IF @@ROWCOUNT = 0 GOTO on_Error

	COMMIT TRANSACTION
	RETURN

on_Error:
	ROLLBACK TRANSACTION
	RETURN
