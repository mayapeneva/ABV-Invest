BEGIN TRANSACTION;
USE ABV_Invest
GO

ALTER TABLE [Balances] DROP CONSTRAINT [FK_Balances_Currencies_CurrencyId];
GO

DROP INDEX [IX_Balances_CurrencyId] ON [Balances];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Balances]') AND [c].[name] = N'CurrencyId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Balances] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Balances] DROP COLUMN [CurrencyId];
GO

ALTER TABLE [Balances] ADD [CurrencyCode] nvarchar(max) NULL;
GO

COMMIT;