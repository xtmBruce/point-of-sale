-- Create RefreshTokens table
-- This script creates the RefreshTokens table if it doesn't exist
-- and updates the migration history to prevent conflicts

-- Step 1: Create the RefreshTokens table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RefreshTokens')
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] uniqueidentifier NOT NULL DEFAULT NEWID(),
        [Token] nvarchar(450) NOT NULL,
        [Expires] datetime2 NOT NULL,
        [Created] datetime2 NOT NULL,
        [CreatedByIp] nvarchar(max) NULL,
        [Revoked] datetime2 NULL,
        [RevokedByIp] nvarchar(max) NULL,
        [ReplacedByToken] nvarchar(max) NULL,
        [UserId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id])
    );
    
    PRINT 'RefreshTokens table created successfully';
END
ELSE
BEGIN
    PRINT 'RefreshTokens table already exists';
END

-- Step 2: Create indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RefreshTokens_Token')
BEGIN
    CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);
    PRINT 'Unique index on Token created';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RefreshTokens_UserId')
BEGIN
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
    PRINT 'Index on UserId created';
END

-- Step 3: Add foreign key constraint
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RefreshTokens_Users_UserId')
BEGIN
    ALTER TABLE [RefreshTokens] 
    ADD CONSTRAINT [FK_RefreshTokens_Users_UserId] 
    FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE;
    PRINT 'Foreign key constraint added';
END

-- Step 4: Update migration history to mark pending migrations as applied
-- This prevents Entity Framework from trying to create tables that already exist

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260419060815_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) 
    VALUES ('20260419060815_InitialCreate', '8.0.0');
    PRINT 'InitialCreate migration marked as applied';
END

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260420225814_RemoveDiscount')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) 
    VALUES ('20260420225814_RemoveDiscount', '8.0.0');
    PRINT 'RemoveDiscount migration marked as applied';
END

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260421053529_AddExpenseRecurringFields')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) 
    VALUES ('20260421053529_AddExpenseRecurringFields', '8.0.0');
    PRINT 'AddExpenseRecurringFields migration marked as applied';
END

PRINT 'Database setup completed successfully!';