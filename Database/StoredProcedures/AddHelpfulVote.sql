CREATE PROCEDURE AddHelpfulVote
	@ReviewID INT
AS
BEGIN
	UPDATE ReviewsTable
	SET HelpfulCount = HelpfulCount + 1
	WHERE ReviewID = @ReviewID;
END;
