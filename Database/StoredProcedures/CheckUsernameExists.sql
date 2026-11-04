CREATE PROCEDURE CheckUsernameExists
	@Username VARCHAR(50)
AS
BEGIN
	SELECT COUNT(*) AS UsernameCount
	FROM UsersTable
	WHERE Username = @Username;
END
