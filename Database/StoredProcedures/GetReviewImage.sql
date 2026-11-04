CREATE PROCEDURE GetReviewImage
	@ReviewID INT
AS
BEGIN
	SELECT ReviewImage AS ImageData, ReviewImageType AS ImageContentType
	FROM ReviewsTable
	WHERE ReviewID = @ReviewID AND ReviewImage IS NOT NULL;
END;
