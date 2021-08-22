USE ABV_Invest
GO

EXEC sp_rename N'[Balances].[AllSecuritiesMarketPrice]', N'AllSecuritiesTotalMarketPrice', N'COLUMN';
GO