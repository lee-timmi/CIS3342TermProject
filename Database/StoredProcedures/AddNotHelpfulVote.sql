CREATE PROCEDURE AddNotHelpfulVote
	@ReviewID INT
AS
BEGIN
	UPDATE ReviewsTable
	SET NotHelpfulCount = NotHelpfulCount + 1
	WHERE ReviewID = @ReviewID;
END;
