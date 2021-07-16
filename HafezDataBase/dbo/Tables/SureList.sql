CREATE TABLE [dbo].[SureList] (
    [ID]            INT          NOT NULL,
    [SuraName]      NVARCHAR (8) NOT NULL,
    [AyahCount]     INT          NOT NULL,
    [NozolSort]     INT          NOT NULL,
    [NozolLocation] NVARCHAR (5) NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);

