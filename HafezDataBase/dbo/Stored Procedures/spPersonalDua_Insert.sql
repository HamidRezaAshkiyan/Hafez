CREATE PROCEDURE [dbo].[spPersonalDua_Insert]
	@DuaName nvarchar(25),
	@Id int output
AS
	INSERT INTO PersonalDuaList(DuaName) 
	VALUES (@DuaName);

	select @id = SCOPE_IDENTITY();
RETURN 0