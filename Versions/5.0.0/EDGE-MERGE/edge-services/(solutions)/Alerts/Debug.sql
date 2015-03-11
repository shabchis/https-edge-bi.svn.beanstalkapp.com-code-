DECLARE	@return_value int

EXEC	@return_value = [dbo].[ConversionAlerts]
		@AccountID = 1239,
		@Period = 30,
		@ToDay = N'2012-10-30 00:00:36.563',
		@ChannelID = 1,
		@threshold = 3

SELECT	'Return Value' = @return_value