BEGIN TRANSACTION;
USE ABV_Invest
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Balances]') AND [c].[name] = N'BalanceId');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Balances] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Balances] DROP COLUMN [BalanceId];
GO

COMMIT;