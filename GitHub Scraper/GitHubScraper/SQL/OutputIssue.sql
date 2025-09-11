DECLARE	@return_value int

EXEC	@return_value = [dbo].[StoreIssue]
		@repository,
		@issueId,
		@number,
		@title,
		@assignee,
		@type,
		@status,
		@dateCreated,
		@dateSolved

SELECT	'Return Value' = @return_value