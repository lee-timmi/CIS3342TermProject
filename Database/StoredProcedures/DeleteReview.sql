CREATE PROCEDURE DeleteReview
	@ReviewID INT,
	@ReviewUserID INT
AS
BEGIN
	DELETE FROM ReviewsTable
	WHERE ReviewID = @ReviewID AND ReviewUserID = @ReviewUserID;
END;
