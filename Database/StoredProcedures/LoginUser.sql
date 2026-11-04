CREATE PROCEDURE LoginUser
	@Username VARCHAR(50),
	@Password VARCHAR(100)
AS
BEGIN
	SELECT UserID, FirstName, LastName, UserType
	FROM UsersTable
	WHERE Username = @Username
		AND Password = @Password COLLATE SQL_Latin1_General_CP1_CS_AS;
END
