-- Database export via SQLPro (https://www.sqlprostudio.com/allapps.html)
-- Exported by huymk at 18-01-2023 16:24.
-- WARNING: This file may contain descructive statements such as DROPs.
-- Please ensure that you are running the script at the proper location.


-- BEGIN TABLE dbo.Account
IF OBJECT_ID('dbo.Account', 'U') IS NOT NULL
DROP TABLE dbo.Account;
GO

CREATE TABLE dbo.Account (
                             Id int NOT NULL IDENTITY(1,1),
                             LoginName nvarchar(50) NOT NULL,
                             Password nvarchar(255) NOT NULL,
                             RegisterDate datetime NOT NULL DEFAULT (getdate()),
                             AccountNumber nvarchar(12) NOT NULL,
                             Ballance float(8) NOT NULL DEFAULT ('0')
);
GO

ALTER TABLE dbo.Account ADD CONSTRAINT [Unique_Account_LoginName] UNIQUE NONCLUSTERED ([LoginName])

ALTER TABLE dbo.Account ADD CONSTRAINT PK__Account__3214EC07B1E1C8B5 PRIMARY KEY (Id);
GO

-- Table dbo.Account contains no data. No inserts have been genrated.
-- Inserting 0 rows into dbo.Account


-- END TABLE dbo.Account

-- BEGIN TABLE dbo.Transaction
IF OBJECT_ID('dbo.[Transaction]', 'U') IS NOT NULL
DROP TABLE dbo.[Transaction];
GO

CREATE TABLE dbo.[Transaction] (
                                   Id int NOT NULL IDENTITY(1,1),
    [Type] nvarchar(50) NOT NULL,
    Amount float(8) NOT NULL DEFAULT ('0'),
    AccountFrom nvarchar(12) NULL,
    AccountTo nvarchar(12) NULL,
    DateOfTransaction datetime NOT NULL DEFAULT (getdate()),
    EndBalance float(8) NOT NULL DEFAULT ('0')
    );
GO

ALTER TABLE dbo.[Transaction] ADD CONSTRAINT PK__Transact__3214EC07E1949B74 PRIMARY KEY (Id);
GO

-- Table dbo.[Transaction] contains no data. No inserts have been genrated.
-- Inserting 0 rows into dbo.[Transaction]


-- END TABLE dbo.Transaction

-- Database export via SQLPro (https://www.sqlprostudio.com/allapps.html)
-- Exported by huymk at 19-01-2023 15:58.
-- WARNING: This file may contain descructive statements such as DROPs.
-- Please ensure that you are running the script at the proper location.


IF EXISTS ( SELECT * FROM sysobjects WHERE  id = object_id(N'dbo.spAddAccount') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
DROP PROCEDURE dbo.spAddAccount
    GO
    SET QUOTED_IDENTIFIER ON
    GO
    SET ANSI_NULLS ON
    GO
CREATE PROCEDURE spAddAccount
(
    @LoginName VARCHAR(50),
    @Password VARCHAR(255),
    @AccountNumber VARCHAR(12),
    @Id int output
)
    AS
BEGIN
INSERT INTO dbo.[Account] (LoginName, Password, AccountNumber)
Values (@LoginName, @Password, @AccountNumber)
SET @Id = (SELECT SCOPE_IDENTITY())
END
GO

IF EXISTS ( SELECT * FROM sysobjects WHERE  id = object_id(N'dbo.spAddDepositTransaction') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
DROP PROCEDURE dbo.spAddDepositTransaction
    GO
    SET QUOTED_IDENTIFIER ON
    GO
    SET ANSI_NULLS ON
    GO
CREATE PROCEDURE [dbo].[spAddDepositTransaction]
(
	@Type VARCHAR(50), 
	@Amount FLOAT,
	@AccountTo VARCHAR(12),
	@EndBalance FLOAT,
	@Id INT OUTPUT
)
AS
BEGIN
BEGIN TRY
IF @Type = 'DEPOSIT'
BEGIN
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
BEGIN TRANSACTION
		DECLARE @currentBallance FLOAT;
		SET @currentBallance = (SELECT Ballance FROM Account WHERE AccountNumber = @AccountTo)

		INSERT INTO dbo.[Transaction] (Type, Amount, AccountTo, EndBalance)
		Values (@Type, @Amount, @AccountTo, @currentBallance + @Amount)

UPDATE dbo.[Account]
SET Ballance = Ballance + @Amount
WHERE AccountNumber = @AccountTo

SET @Id = (SELECT SCOPE_IDENTITY())
    COMMIT TRANSACTION
END
END TRY
BEGIN CATCH
IF @@TRANCOUNT > 0  
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
END
GO

IF EXISTS ( SELECT * FROM sysobjects WHERE  id = object_id(N'dbo.spAddExchangeTransaction') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
DROP PROCEDURE dbo.spAddExchangeTransaction
    GO
    SET QUOTED_IDENTIFIER ON
    GO
    SET ANSI_NULLS ON
    GO
CREATE PROCEDURE [dbo].[spAddExchangeTransaction]
(
	@Type VARCHAR(50), 
	@Amount FLOAT,
	@AccountFrom VARCHAR(12),
	@AccountTo VARCHAR(12),
	@EndBalance FLOAT,
	@Id INT OUTPUT
)
AS
BEGIN
BEGIN TRY
IF @Type = 'EXCHANGE'
BEGIN
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

BEGIN TRANSACTION;
		DECLARE @currentBallance FLOAT;
		SET @currentBallance = (SELECT Ballance FROM Account WHERE AccountNumber = @AccountFrom)
		
		INSERT INTO dbo.[Transaction] (Type, Amount, AccountFrom, AccountTo, EndBalance)
		Values (@Type, @Amount, @AccountFrom, @AccountTo, @currentBallance - @Amount)

UPDATE dbo.[Account]
SET Ballance = Ballance - @Amount
WHERE AccountNumber = @AccountFrom

UPDATE dbo.[Account]
SET Ballance = Ballance + @Amount
WHERE AccountNumber = @AccountTo

SET @Id = (SELECT SCOPE_IDENTITY())
    COMMIT TRANSACTION
END
END TRY
BEGIN CATCH
IF @@TRANCOUNT > 0  
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
END
GO

IF EXISTS ( SELECT * FROM sysobjects WHERE  id = object_id(N'dbo.spAddTransaction') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
DROP PROCEDURE dbo.spAddTransaction
    GO
    SET QUOTED_IDENTIFIER ON
    GO
    SET ANSI_NULLS ON
    GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spAddTransaction
(
    @Type VARCHAR(50),
    @Amount FLOAT,
    @AccountFrom VARCHAR(12),
    @AccountTo VARCHAR(12),
    @EndBalance FLOAT,
    @Id INT OUTPUT
)
    AS
BEGIN
INSERT INTO dbo.[Transaction] (Type, Amount, AccountFrom, AccountTo, EndBalance)
Values (@Type, @Amount, @AccountFrom, @AccountTo, @EndBalance)
SET @Id = (SELECT SCOPE_IDENTITY())
END
GO

IF EXISTS ( SELECT * FROM sysobjects WHERE  id = object_id(N'dbo.spAddWithDrawTransaction') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
DROP PROCEDURE dbo.spAddWithDrawTransaction
    GO
    SET QUOTED_IDENTIFIER ON
    GO
    SET ANSI_NULLS ON
    GO
CREATE PROCEDURE [dbo].[spAddWithDrawTransaction]
(
	@Type VARCHAR(50), 
	@Amount FLOAT,
	@AccountFrom VARCHAR(12),
	@EndBalance FLOAT,
	@Id INT OUTPUT
)
AS
BEGIN
BEGIN TRY
IF @Type = 'WITHDRAW'
BEGIN
SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

BEGIN TRANSACTION;
		DECLARE @currentBallance FLOAT;
		SET @currentBallance = (SELECT Ballance FROM Account WHERE AccountNumber = @AccountFrom)
		
		INSERT INTO dbo.[Transaction] (Type, Amount, AccountFrom, EndBalance)
		Values (@Type, @Amount, @AccountFrom, @currentBallance - @Amount)

UPDATE dbo.[Account]
SET Ballance = Ballance - @Amount
WHERE AccountNumber = @AccountFrom

SET @Id = (SELECT SCOPE_IDENTITY())
    COMMIT TRANSACTION
END
END TRY
BEGIN CATCH
IF @@TRANCOUNT > 0  
        ROLLBACK TRANSACTION;
    THROW;
END CATCH;
END
GO

IF EXISTS ( SELECT * FROM sysobjects WHERE  id = object_id(N'dbo.spGetAllTransaction') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
DROP PROCEDURE dbo.spGetAllTransaction
    GO
    SET QUOTED_IDENTIFIER ON
    GO
    SET ANSI_NULLS ON
    GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spGetAllTransaction
(
    @AccountNumber VARCHAR(12)
)
    AS
BEGIN
SELECT * FROM dbo.[Transaction]
WHERE (Type = 'DEPOSIT'  AND AccountTo = @AccountNumber) OR
    (Type = 'WITHDRAW'  AND AccountFrom = @AccountNumber) OR
    (Type = 'EXCHANGE'  AND AccountFrom = @AccountNumber)
ORDER BY DateOfTransaction DESC
END
GO

IF EXISTS ( SELECT * FROM sysobjects WHERE  id = object_id(N'dbo.spUpdateBallanceAccount') AND OBJECTPROPERTY(id, N'IsProcedure') = 1 )
DROP PROCEDURE dbo.spUpdateBallanceAccount
    GO
    SET QUOTED_IDENTIFIER ON
    GO
    SET ANSI_NULLS ON
    GO
CREATE PROCEDURE spUpdateBallanceAccount
(
    @Id INT,
    @Ballance FLOAT
)
    AS
BEGIN
UPDATE Account
SET Ballance = @Ballance
WHERE Id = @Id
END
GO

