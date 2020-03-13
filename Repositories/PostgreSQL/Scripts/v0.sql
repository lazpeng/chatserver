﻿-- Version 0 upgrade script

CREATE SCHEMA IF NOT EXISTS chat;

-- Information about registered users
CREATE TABLE IF NOT EXISTS chat.USERS (
    Id CHAR(36) PRIMARY KEY,
    UserName VARCHAR(128) NOT NULL,
    FullName VARCHAR(256) NOT NULL,
    Email VARCHAR(256),
    Bio TEXT,
    AccountCreated TIMESTAMPTZ NOT NULL,
    LastLogin TIMESTAMPTZ NOT NULL,
    LastSeen TIMESTAMPTZ NOT NULL,
    DateOfBirth DATE NOT NULL,
    FindInSearch BOOLEAN NOT NULL,
    OpenChat BOOLEAN NOT NULL,
    PasswordHash CHAR(44) NOT NULL,
    PasswordSalt CHAR(44) NOT NULL,
    CONSTRAINT USER_UNIQUE UNIQUE (UserName),
    CONSTRAINT EMAIL_UNIQUE UNIQUE (Email)
);

-- Friend list and pending friend requests
CREATE TABLE IF NOT EXISTS chat.FRIENDS (
    A CHAR(36),
    B CHAR(36),
    SentDate TIMESTAMPTZ NOT NULL,
    SettleDate TIMESTAMPTZ,
    Accepted BOOLEAN NOT NULL DEFAULT FALSE,
    CONSTRAINT FRIENDS_PK PRIMARY KEY (A, B),
    CONSTRAINT FRIENDS_A_FK FOREIGN KEY (A) REFERENCES chat.USERS (Id) ON DELETE CASCADE,
    CONSTRAINT FRIENDS_B_FK FOREIGN KEY (B) REFERENCES chat.USERS (Id) ON DELETE CASCADE
);

-- All blocked users
CREATE TABLE IF NOT EXISTS chat.BLOCKLIST (
    UserId CHAR(36),
    BlockedId CHAR(36),
    BlockDate TIMESTAMPTZ NOT NULL,
    CONSTRAINT BLOCKLIST_PK PRIMARY KEY (UserId, BlockedId),
    CONSTRAINT BLOCKLIST_USER FOREIGN KEY (UserId) REFERENCES chat.USERS (Id) ON DELETE CASCADE,
    CONSTRAINT BLOCKLIST_BLOCKED FOREIGN KEY (BlockedId) REFERENCES chat.USERS (Id) ON DELETE CASCADE
);

-- Currently active sessions after users log in
CREATE TABLE IF NOT EXISTS chat.SESSIONS (
    UserId CHAR(36) NOT NULL,
    Token CHAR(36) NOT NULL,
    CreatedOn TIMESTAMPTZ NOT NULL,
    ExpirationDate TIMESTAMPTZ NOT NULL,
    CONSTRAINT SESSIONS_PK PRIMARY KEY (UserId, Token)
);

-- Contains all sent messages, to whom and its content
CREATE TABLE IF NOT EXISTS chat.MESSAGES (
    Id BIGSERIAL PRIMARY KEY,
    SourceId CHAR(36),
    TargetId CHAR(36),
    DateSent TIMESTAMPTZ NOT NULL,
    DateSeen TIMESTAMPTZ,
    Content TEXT NOT NULL,
    InReplyTo BIGINT,
    CONSTRAINT MESSAGES_SOURCE_FK FOREIGN KEY (SourceId) REFERENCES chat.USERS (Id) ON DELETE CASCADE,
    CONSTRAINT MESSAGES_TARGET_FK FOREIGN KEY (TargetId) REFERENCES chat.USERS (Id) ON DELETE NO ACTION,
    CONSTRAINT MESSAGES_REPLY_FK  FOREIGN KEY (InReplyTo) REFERENCES chat.MESSAGES (Id) ON DELETE NO ACTION
);

-- Contains information about currently installed versions
CREATE TABLE IF NOT EXISTS chat.DbUpgrade (
    Version INT PRIMARY KEY,
    DateInstalled TIMESTAMP NOT NULL
);