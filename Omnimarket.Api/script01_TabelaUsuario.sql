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
CREATE TABLE [TBL_USUARIO] (
    [Id] int NOT NULL IDENTITY,
    [Cpf] varchar(200) NOT NULL,
    [Nome] varchar(200) NOT NULL,
    [Sobrenome] varchar(200) NOT NULL,
    [PasswordHash] varbinary(max) NULL,
    [PasswordSalt] varbinary(max) NULL,
    [Foto] varbinary(max) NULL,
    [DataAcesso] datetime2 NULL,
    [DataCadastro] datetime2 NOT NULL,
    [Email] varchar(200) NOT NULL,
    CONSTRAINT [PK_TBL_USUARIO] PRIMARY KEY ([Id])
);

CREATE TABLE [TBL_ENDERECO] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [TipoLogradouro] nvarchar(200) NOT NULL,
    [NomeEndereco] varchar(200) NOT NULL,
    [Numero] varchar(200) NOT NULL,
    [Complemento] varchar(200) NULL,
    [Cep] varchar(200) NOT NULL,
    [Cidade] varchar(200) NOT NULL,
    [Uf] varchar(200) NOT NULL,
    [IsPrincipal] bit NOT NULL,
    CONSTRAINT [PK_TBL_ENDERECO] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_ENDERECO_TBL_USUARIO_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [TBL_USUARIO] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [TBL_TELEFONE] (
    [Id] int NOT NULL IDENTITY,
    [UsuarioId] int NOT NULL,
    [NumeroE164] varchar(200) NOT NULL,
    [IsPrincipal] bit NOT NULL,
    CONSTRAINT [PK_TBL_TELEFONE] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TBL_TELEFONE_TBL_USUARIO_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [TBL_USUARIO] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_TBL_ENDERECO_UsuarioId] ON [TBL_ENDERECO] ([UsuarioId]);

CREATE INDEX [IX_TBL_TELEFONE_UsuarioId] ON [TBL_TELEFONE] ([UsuarioId]);

CREATE UNIQUE INDEX [IX_TBL_USUARIO_Cpf] ON [TBL_USUARIO] ([Cpf]);

CREATE UNIQUE INDEX [IX_TBL_USUARIO_Email] ON [TBL_USUARIO] ([Email]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260127205840_InitialCreate', N'9.0.0');

COMMIT;
GO

