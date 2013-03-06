USE [Crawler]
GO

/****** Object:  Table [dbo].[crawl]    Script Date: 12/09/2012 20:09:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[crawl](
	[UrlID] [int] IDENTITY(1,1) NOT NULL,
	[URL] [nvarchar](max) NOT NULL,
	[ParentUrlID] [int] NULL,
	[isCrawled] [bit] NOT NULL,
 CONSTRAINT [PK_crawl] PRIMARY KEY CLUSTERED 
(
	[UrlID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


