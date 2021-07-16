CREATE PROCEDURE [dbo].[spNotification_Insert]
	@GroupId int ,
	@Description nvarchar(250),
	@Name nvarchar(250),
	@NotificationType char(1),
	@CreatedBy int,
	@Id int = 0 output
AS
	INSERT INTO Notification (GroupId, Description, Name, NotificationType, CreatedBy) 
	VALUES (@GroupId, @Description, @Name, @NotificationType, @CreatedBy);

	select @id = SCOPE_IDENTITY();
RETURN 0