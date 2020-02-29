-- Version 0 upgrade script

CREATE SCHEMA chat

-- Information about registered users
CREATE TABLE chat.USERS (
    Id CHAR(36) PRIMARY KEY,
    UserName VARCHAR(128) NOT NULL,
    FullName VARCHAR(256) NOT NULL,
    Email VARCHAR(256),
    Bio TEXT,
    AccountCreated DATETIME NOT NULL,
    LastLogin DATETIME NOT NULL,
    LastSeen DATETIME NOT NULL,
    DateOfBirth DATE NOT NULL,
    ProfilePicUrl TEXT,
    FindInSearch BIT NOT NULL DEFAULT 1,
    OpenChat BIT NOT NULL DEFAULT 1,
    PasswordHash CHAR(44) NOT NULL,
    PasswordSalt CHAR(44) NOT NULL,
    CONSTRAINT USER_UNIQUE UNIQUE (UserName),
    CONSTRAINT EMAIL_UNIQUE UNIQUE (Email)
)

-- Friend list and pending friend requests
CREATE TABLE chat.FRIENDS (
    A CHAR(36) FOREIGN KEY REFERENCES chat.USERS (Id),
    B CHAR(36) FOREIGN KEY REFERENCES chat.USERS (Id),
    SentDate DATETIME NOT NULL,
    SettleDate DATETIME,
    Accepted BIT NOT NULL DEFAULT 0,
    CONSTRAINT FRIENDS_PK PRIMARY KEY (A, B)
)

-- All blocked users
CREATE TABLE chat.BLOCKLIST (
    UserId CHAR(36) FOREIGN KEY REFERENCES chat.USERS (Id),
    BlockedId CHAR(36) FOREIGN KEY REFERENCES chat.USERS (Id),
    BlockDate DATETIME NOT NULL,
    CONSTRAINT BLOCKLIST_PK PRIMARY KEY (UserId, BlockedId)
)

-- Currently active sessions after users log in
CREATE TABLE chat.SESSIONS (
    UserId CHAR(36) NOT NULL,
    Token CHAR(36) NOT NULL,
    CreatedOn DATETIME NOT NULL,
    ExpirationDate DATETIME NOT NULL,
    CONSTRAINT SESSIONS_PK PRIMARY KEY (UserId, Token),
    CHECK (ExpirationDate > SYSDATETIME())
)

-- Contains all sent messages, to whom and its content
CREATE TABLE chat.MESSAGES (
    Id BigInt IDENTITY(1,1) PRIMARY KEY,
    SourceId CHAR(36) FOREIGN KEY REFERENCES chat.USERS (Id),
    TargetId CHAR(36) FOREIGN KEY REFERENCES chat.USERS (Id),
    DateSent DATETIME NOT NULL,
    DateSeen DATETIME,
    Content TEXT NOT NULL,
    InReplyTo BigInt FOREIGN KEY REFERENCES chat.MESSAGES (Id) ON DELETE NO ACTION
)

-- Contains information about currently installed versions
CREATE TABLE chat.DBINFO (
    Version INT PRIMARY KEY,
    DateInstalled DATETIME NOT NULL,
)

GO

-- Since ON DELETE CASCADE isn't exactly supported on Sql Server under this conditions,
-- we set up a trigger to delete all user related rows in other tables
CREATE TRIGGER chat.TK_USERS_DELETE ON chat.USERS FOR DELETE AS
DECLARE
    @OldId CHAR(36);

    SELECT @OldId = del.ID from deleted del;

    DELETE FROM chat.FRIENDS WHERE A = @OldId OR B = @OldId;
    DELETE FROM chat.BLOCKLIST WHERE UserId = @OldId OR BlockedId = @OldId;
    DELETE FROM chat.SESSIONS WHERE UserId = @OldId;
    DELETE FROM chat.MESSAGES WHERE SourceId = @OldId;

GO

-- Save the successful upgrade to version 1
INSERT INTO chat.DBINFO(Version, DateInstalled) VALUES (1, SYSDATETIME())