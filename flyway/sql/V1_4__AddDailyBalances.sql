USE ABV_Invest
GO

ALTER TABLE [DailyBalance] DROP CONSTRAINT [FK_DailyBalance_AbvInvestUsers_AbvInvestUserId];
GO

ALTER TABLE [DailyBalance] DROP CONSTRAINT [FK_DailyBalance_Balances_BalanceId];
GO

ALTER TABLE [DailyBalance] DROP CONSTRAINT [PK_DailyBalance];
GO

EXEC sp_rename N'[DailyBalance]', N'DailyBalances';
GO

EXEC sp_rename N'[DailyBalances].[IX_DailyBalance_BalanceId]', N'IX_DailyBalances_BalanceId', N'INDEX';
GO

EXEC sp_rename N'[DailyBalances].[IX_DailyBalance_AbvInvestUserId]', N'IX_DailyBalances_AbvInvestUserId', N'INDEX';
GO

ALTER TABLE [Markets] ADD [MIC] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [DailyBalances] ADD CONSTRAINT [PK_DailyBalances] PRIMARY KEY ([Id]);
GO

ALTER TABLE [DailyBalances] ADD CONSTRAINT [FK_DailyBalances_AbvInvestUsers_AbvInvestUserId] FOREIGN KEY ([AbvInvestUserId]) REFERENCES [AbvInvestUsers] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [DailyBalances] ADD CONSTRAINT [FK_DailyBalances_Balances_BalanceId] FOREIGN KEY ([BalanceId]) REFERENCES [Balances] ([Id]) ON DELETE CASCADE;
GO