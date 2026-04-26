-- First, let's check if RefreshTokens table exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RefreshTokens')
BEGIN
    -- Create RefreshTokens table
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
    
    -- Create indexes
    CREATE UNIQUE INDEX [IX_RefreshTokens_Token] ON [RefreshTokens] ([Token]);
    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
    
    -- Add foreign key constraint
    ALTER TABLE [RefreshTokens] 
    ADD CONSTRAINT [FK_RefreshTokens_Users_UserId] 
    FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE;
    
    PRINT 'RefreshTokens table created successfully';
END
ELSE
BEGIN
    PRINT 'RefreshTokens table already exists';
END

-- Update migration history to mark migrations as applied
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260419060815_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) 
    VALUES ('20260419060815_InitialCreate', '8.0.0');
END

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260420225814_RemoveDiscount')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) 
    VALUES ('20260420225814_RemoveDiscount', '8.0.0');
END

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260421053529_AddExpenseRecurringFields')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) 
    VALUES ('20260421053529_AddExpenseRecurringFields', '8.0.0');
END

PRINT 'Migration history updated';