USE ABV_Invest
GO

ALTER TABLE [Deals] ADD [Coupon] decimal(18, 4) NOT NULL DEFAULT 0.0;
GO

ALTER TABLE [Deals] ADD [TotalPriceInBGN] decimal(18, 4) NOT NULL DEFAULT 0.0;
GO

ALTER TABLE [Balances] ADD [AllSecuritiesTotalPriceBuy] decimal(18, 4) NOT NULL DEFAULT 0.0;
GO