CREATE PROCEDURE AddUser
	@Username VARCHAR(50),
	@Password VARCHAR(100),
	@FirstName VARCHAR(50),
	@LastName VARCHAR(50),
	@Email VARCHAR(100),
	@Phone VARCHAR(20),
	@UserType VARCHAR(20)
AS
BEGIN
	INSERT INTO UsersTable (Username, Password, FirstName, LastName, Email, Phone, UserType)
	VALUES (@Username, @Password, @FirstName, @LastName, @Email, @Phone, @UserType);
END
