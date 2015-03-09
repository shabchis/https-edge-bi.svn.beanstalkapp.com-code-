GO

DECLARE	@return_value int

EXEC	@return_value = [dbo].[CLR_GetTablesList]
		@accountID = 95,
		@channelID = -1

SELECT	'Return Value' = @return_value

GO
