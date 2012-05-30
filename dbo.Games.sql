USE [CHESSDB]
GO

/****** Object:  Table [dbo].[Games]    Script Date: 05/30/2012 19:26:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Games](
	[ID] [int] NOT NULL,
	[Event] [nvarchar](50) NULL,
	[Site] [nvarchar](50) NULL,
	[Date] [nvarchar](50) NULL,
	[Round] [nvarchar](50) NULL,
	[White] [nvarchar](50) NULL,
	[Black] [nvarchar](50) NULL,
	[Result] [nvarchar](50) NULL,
	[Moves] [nvarchar](max) NULL,
 CONSTRAINT [PK_Games] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

