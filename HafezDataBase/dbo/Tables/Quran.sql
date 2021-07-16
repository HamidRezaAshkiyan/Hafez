CREATE TABLE [dbo].[Quran] (
    [ID]                     INT             NOT NULL,
    [SuraID]                 INT             NOT NULL,
    [AyahID]                 INT             NULL,
    [PageNumber]             INT             NULL,
    [AyahText]               NVARCHAR (1200) NOT NULL,
    [AyahPersianTranslation] NVARCHAR (MAX)  NOT NULL,
    [AyahEnglishTranslation] VARCHAR (MAX)   NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Quran_ToSureList] FOREIGN KEY ([SuraID]) REFERENCES [dbo].[SureList] ([ID])
);



