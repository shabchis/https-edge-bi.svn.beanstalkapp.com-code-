﻿CREATE TABLE [dbo].[7_AdChannel] (
    [AccountID]                    BIGINT          NULL,
    [ChannelID]                    BIGINT          NULL,
    [AdGK]                         BIGINT          NULL,
    [AdgroupGK]                    BIGINT          NULL,
    [CampaignGK]                   BIGINT          NULL,
    [TrackerGK]                    BIGINT          NULL,
    [ImageCreativeGK]              BIGINT          NULL,
    [TitleCreativeGK]              BIGINT          NULL,
    [BodyCreativeGK]               BIGINT          NULL,
    [DisplayURLCreativeGK]         BIGINT          NULL,
    [DestinationURLCreativeGK]     BIGINT          NULL,
    [VideoCreativeGK]              BIGINT          NULL,
    [KeywordGK]                    BIGINT          NULL,
    [KeywordMatchTypeGK]           BIGINT          NULL,
    [KeywordQualityScoreGK]        BIGINT          NULL,
    [SearchQueryGK]                BIGINT          NULL,
    [ThemeGK]                      BIGINT          NULL,
    [LanguageGK]                   BIGINT          NULL,
    [CountryGK]                    BIGINT          NULL,
    [GeographicGK]                 BIGINT          NULL,
    [LandingPageGK]                BIGINT          NULL,
    [TargetPeriodStart]            DATETIME        NULL,
    [TargetPeriodEnd]              DATETIME        NULL,
    [OutputID]                     CHAR (32)       NULL,
    [Currency]                     NVARCHAR (10)   NULL,
    [Cost]                         DECIMAL (18, 2) NULL,
    [OriginalCost]                 DECIMAL (18, 2) NULL,
    [AveragePosition]              DECIMAL (18, 1) NULL,
    [Clicks]                       INT             NULL,
    [Impressions]                  INT             NULL,
    [UniqueImpressions]            INT             NULL,
    [UniqueClicks]                 INT             NULL,
    [TotalConversionsOnePerClick]  INT             NULL,
    [TotalConversionsManyPerClick] INT             NULL,
    [Signups]                      INT             NULL,
    [Purchases]                    INT             NULL,
    [Leads]                        INT             NULL,
    [PageViews]                    INT             NULL,
    [Default]                      INT             NULL
);

