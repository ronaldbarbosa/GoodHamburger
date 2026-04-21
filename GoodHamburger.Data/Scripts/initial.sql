IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Orders] (
    [Id] int NOT NULL IDENTITY,
    [Subtotal] decimal(18,2) NOT NULL,
    [Discount] decimal(18,2) NOT NULL,
    [Total] decimal(18,2) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [Status] nvarchar(20) NOT NULL DEFAULT N'Pending',
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id])
);

CREATE TABLE [ProductCategories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_ProductCategories] PRIMARY KEY ([Id])
);

CREATE TABLE [Products] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(150) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [CategoryId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Products_ProductCategories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [ProductCategories] ([Id])
);

CREATE TABLE [OrderItems] (
    [Id] int NOT NULL IDENTITY,
    [OrderId] int NOT NULL,
    [ProductId] int NOT NULL,
    [Quantity] int NOT NULL,
    [UnitPrice] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE NO ACTION
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[ProductCategories]'))
    SET IDENTITY_INSERT [ProductCategories] ON;
INSERT INTO [ProductCategories] ([Id], [CreatedAt], [IsActive], [Name])
VALUES (1, '2026-04-21T17:00:00.0000000', CAST(1 AS bit), N'Sanduíches'),
(2, '2026-04-21T17:00:00.0000000', CAST(1 AS bit), N'Acompanhamentos');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[ProductCategories]'))
    SET IDENTITY_INSERT [ProductCategories] OFF;

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CategoryId', N'CreatedAt', N'IsActive', N'Name', N'Price') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] ON;
INSERT INTO [Products] ([Id], [CategoryId], [CreatedAt], [IsActive], [Name], [Price])
VALUES (1, 1, '2026-04-21T17:00:00.0000000', CAST(1 AS bit), N'X-Burger', 15.0),
(2, 1, '2026-04-21T17:00:00.0000000', CAST(1 AS bit), N'X-Egg', 17.0),
(3, 1, '2026-04-21T17:00:00.0000000', CAST(1 AS bit), N'X-Bacon', 19.0),
(4, 2, '2026-04-21T17:00:00.0000000', CAST(1 AS bit), N'Batata Frita', 10.0),
(5, 2, '2026-04-21T17:00:00.0000000', CAST(1 AS bit), N'Refrigerante', 5.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CategoryId', N'CreatedAt', N'IsActive', N'Name', N'Price') AND [object_id] = OBJECT_ID(N'[Products]'))
    SET IDENTITY_INSERT [Products] OFF;

CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);

CREATE INDEX [IX_OrderItems_ProductId] ON [OrderItems] ([ProductId]);

CREATE INDEX [IX_Orders_CreatedAt] ON [Orders] ([CreatedAt]);

CREATE INDEX [IX_Orders_Status] ON [Orders] ([Status]);

CREATE UNIQUE INDEX [IX_ProductCategories_Name] ON [ProductCategories] ([Name]);

CREATE INDEX [IX_Products_CategoryId] ON [Products] ([CategoryId]);

CREATE INDEX [IX_Products_IsActive] ON [Products] ([IsActive]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260421200852_InitialCreate', N'10.0.6');

COMMIT;
GO

