CREATE TABLE [Users](
    [ID] [INT] IDENTITY(1,1) NOT NULL,
	[EmailAddress] [nvarchar](255) NOT NULL,
    [CreatedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
) ON [PRIMARY]

CREATE TABLE [StorageAreas](
    [ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[CreatedBy] [int] FOREIGN KEY REFERENCES Users(ID),
    [CreatedOn] [datetime] NOT NULL,
	[LastModified] [datetime] NOT NULL,
 CONSTRAINT [PK_StorageAreas] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
) ON [PRIMARY]


CREATE TABLE [Roles](
    [ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)
WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
) ON [PRIMARY]

INSERT INTO [Roles] ([Name])
VALUES ('Super User'),
('Creator'),
('Delete'),
('Edit'),
('Add'),
('View')


CREATE TABLE [StorageAreaAccess](
    [ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] FOREIGN KEY REFERENCES Users(ID),
	[StorageAreaID] [int] FOREIGN KEY REFERENCES StorageAreas(ID),
	[RoleID] [int] FOREIGN KEY REFERENCES Roles(ID),
 CONSTRAINT [PK_StorageAreaAccess] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
) ON [PRIMARY]


CREATE TABLE [ColumnTypes](
    [ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[SqlDataType]  [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ColumnTypes] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
) ON [PRIMARY]


INSERT INTO [ColumnTypes] ([Name], [SqlDataType]) 
VALUES ('Yes/No', 'bit'),
('Integer','int'),
('Decimal','float'),
('Small Text', 'nvarchar(255)'),
('Big Text','nvarchar(MAX)'),
('GUID', 'uniqueidentifier'),
('XML', 'xml'),
('Date/Time','datetime')