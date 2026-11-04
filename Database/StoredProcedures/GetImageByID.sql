CREATE PROCEDURE GetImageByID
	@ImageID INT
AS
BEGIN
	SELECT ImageData, ImageContentType
	FROM RestaurantImages
	WHERE ImageID = @ImageID;
END;
