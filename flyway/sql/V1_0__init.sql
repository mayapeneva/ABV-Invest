WAITFOR DELAY '00:00:03'

CREATE LOGIN AppLogin WITH PASSWORD = 'Asd0123K'
GO

DROP USER IF EXISTS ABVInvestUser
CREATE USER ABVInvestUser FROM LOGIN AppLogin
GO

DROP ROLE IF EXISTS AspNetRoles
CREATE ROLE AspNetRoles
GO

DROP DATABASE IF EXISTS ABV_Invest
GO

GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE TO AspNetRoles
EXEC sp_addrolemember N'AspNetRoles', N'ABVInvestUser' 
GO

Create DATABASE ABV_Invest
GO

BEGIN TRANSACTION;
USE ABV_Invest
GO 

CREATE TABLE [AbvInvestUsers] (
    [Id] nvarchar(200) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    [PIN] nvarchar(max) NULL,
    [FullName] nvarchar(max) NULL,
    [BalanceId] int NOT NULL,
    CONSTRAINT [PK_AbvInvestUsers] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(200) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Currencies] (
    [Id] int NOT NULL IDENTITY,
    [Code] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Currencies] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Issuers] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Issuers] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Markets] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Markets] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(200) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AbvInvestUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbvInvestUsers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(100) NOT NULL,
    [ProviderKey] nvarchar(100) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(200) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AbvInvestUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbvInvestUsers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(200) NOT NULL,
    [LoginProvider] nvarchar(100) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AbvInvestUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbvInvestUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [DailyDeals] (
    [Id] int NOT NULL IDENTITY,
    [AbvInvestUserId] nvarchar(200) NULL,
    [Date] datetime2 NOT NULL,
    CONSTRAINT [PK_DailyDeals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DailyDeals_AbvInvestUsers_AbvInvestUserId] FOREIGN KEY ([AbvInvestUserId]) REFERENCES [AbvInvestUsers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [DailySecuritiesPerClient] (
    [Id] int NOT NULL IDENTITY,
    [AbvInvestUserId] nvarchar(200) NULL,
    [Date] datetime2 NOT NULL,
    CONSTRAINT [PK_DailySecuritiesPerClient] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DailySecuritiesPerClient_AbvInvestUsers_AbvInvestUserId] FOREIGN KEY ([AbvInvestUserId]) REFERENCES [AbvInvestUsers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(200) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(200) NOT NULL,
    [RoleId] nvarchar(200) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AbvInvestUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AbvInvestUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Balances] (
    [Id] int NOT NULL IDENTITY,
    [BalanceId] int NOT NULL,
    [CurrencyId] int NOT NULL,
    [Cash] decimal(18, 2) NOT NULL,
    [AllSecuritiesMarketPrice] decimal(18, 4) NOT NULL,
    [VirtualProfit] decimal(18, 4) NOT NULL,
    [VirtualProfitPercentage] decimal(18, 4) NOT NULL,
    CONSTRAINT [PK_Balances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Balances_Currencies_CurrencyId] FOREIGN KEY ([CurrencyId]) REFERENCES [Currencies] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Securities] (
    [Id] int NOT NULL IDENTITY,
    [IssuerId] int NOT NULL,
    [SecuritiesType] nvarchar(max) NULL,
    [ISIN] nvarchar(max) NOT NULL,
    [BfbCode] nvarchar(max) NULL,
    [CurrencyId] int NULL,
    CONSTRAINT [PK_Securities] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Securities_Currencies_CurrencyId] FOREIGN KEY ([CurrencyId]) REFERENCES [Currencies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Securities_Issuers_IssuerId] FOREIGN KEY ([IssuerId]) REFERENCES [Issuers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [DailyBalance] (
    [Id] int NOT NULL IDENTITY,
    [AbvInvestUserId] nvarchar(200) NULL,
    [Date] datetime2 NOT NULL,
    [BalanceId] int NOT NULL,
    CONSTRAINT [PK_DailyBalance] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DailyBalance_AbvInvestUsers_AbvInvestUserId] FOREIGN KEY ([AbvInvestUserId]) REFERENCES [AbvInvestUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DailyBalance_Balances_BalanceId] FOREIGN KEY ([BalanceId]) REFERENCES [Balances] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Deals] (
    [Id] int NOT NULL IDENTITY,
    [DailyDealsId] int NOT NULL,
    [DealType] nvarchar(max) NOT NULL,
    [SecurityId] int NOT NULL,
    [Quantity] decimal(18, 2) NOT NULL,
    [Price] decimal(18, 4) NOT NULL,
    [TotalPrice] decimal(18, 4) NOT NULL,
    [Fee] decimal(18, 4) NOT NULL,
    [CurrencyId] int NOT NULL,
    [Settlement] datetime2 NOT NULL,
    [MarketId] int NOT NULL,
    CONSTRAINT [PK_Deals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Deals_Currencies_CurrencyId] FOREIGN KEY ([CurrencyId]) REFERENCES [Currencies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Deals_DailyDeals_DailyDealsId] FOREIGN KEY ([DailyDealsId]) REFERENCES [DailyDeals] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Deals_Markets_MarketId] FOREIGN KEY ([MarketId]) REFERENCES [Markets] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Deals_Securities_SecurityId] FOREIGN KEY ([SecurityId]) REFERENCES [Securities] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [SecuritiesPerClient] (
    [Id] int NOT NULL IDENTITY,
    [DailySecuritiesPerClientId] int NOT NULL,
    [SecurityId] int NOT NULL,
    [Quantity] decimal(18, 2) NOT NULL,
    [CurrencyId] int NOT NULL,
    [AveragePriceBuy] decimal(18, 4) NOT NULL,
    [MarketPrice] decimal(18, 4) NOT NULL,
    [TotalMarketPrice] decimal(18, 4) NOT NULL,
    [Profit] decimal(18, 4) NOT NULL,
    [ProfitInBGN] decimal(18, 4) NOT NULL,
    [ProfitPercentаge] decimal(18, 4) NOT NULL,
    [PortfolioShare] decimal(18, 4) NOT NULL,
    CONSTRAINT [PK_SecuritiesPerClient] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SecuritiesPerClient_Currencies_CurrencyId] FOREIGN KEY ([CurrencyId]) REFERENCES [Currencies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SecuritiesPerClient_DailySecuritiesPerClient_DailySecuritiesPerClientId] FOREIGN KEY ([DailySecuritiesPerClientId]) REFERENCES [DailySecuritiesPerClient] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SecuritiesPerClient_Securities_SecurityId] FOREIGN KEY ([SecurityId]) REFERENCES [Securities] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [EmailIndex] ON [AbvInvestUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AbvInvestUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [IX_Balances_CurrencyId] ON [Balances] ([CurrencyId]);
GO

CREATE INDEX [IX_DailyBalance_AbvInvestUserId] ON [DailyBalance] ([AbvInvestUserId]);
GO

CREATE UNIQUE INDEX [IX_DailyBalance_BalanceId] ON [DailyBalance] ([BalanceId]);
GO

CREATE INDEX [IX_DailyDeals_AbvInvestUserId] ON [DailyDeals] ([AbvInvestUserId]);
GO

CREATE INDEX [IX_DailySecuritiesPerClient_AbvInvestUserId] ON [DailySecuritiesPerClient] ([AbvInvestUserId]);
GO

CREATE INDEX [IX_Deals_CurrencyId] ON [Deals] ([CurrencyId]);
GO

CREATE INDEX [IX_Deals_DailyDealsId] ON [Deals] ([DailyDealsId]);
GO

CREATE INDEX [IX_Deals_MarketId] ON [Deals] ([MarketId]);
GO

CREATE INDEX [IX_Deals_SecurityId] ON [Deals] ([SecurityId]);
GO

CREATE INDEX [IX_Securities_CurrencyId] ON [Securities] ([CurrencyId]);
GO

CREATE INDEX [IX_Securities_IssuerId] ON [Securities] ([IssuerId]);
GO

CREATE INDEX [IX_SecuritiesPerClient_CurrencyId] ON [SecuritiesPerClient] ([CurrencyId]);
GO

CREATE INDEX [IX_SecuritiesPerClient_DailySecuritiesPerClientId] ON [SecuritiesPerClient] ([DailySecuritiesPerClientId]);
GO

CREATE INDEX [IX_SecuritiesPerClient_SecurityId] ON [SecuritiesPerClient] ([SecurityId]);
GO

COMMIT;
