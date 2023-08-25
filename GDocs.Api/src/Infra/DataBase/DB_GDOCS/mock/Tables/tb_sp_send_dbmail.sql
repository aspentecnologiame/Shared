CREATE TABLE [mock].[tb_sp_send_dbmail] (
    [id]                    INT           IDENTITY (1, 1) NOT NULL,
    [from_address]          VARCHAR (50)  NOT NULL,
    [profile_name]          VARCHAR (50)  NOT NULL,
    [recipients]            VARCHAR (500) NOT NULL,
    [blind_copy_recipients] VARCHAR (10)  NOT NULL,
    [subject]               VARCHAR (100) NOT NULL,
    [body]                  VARCHAR (MAX) NOT NULL,
    [body_format]           VARCHAR (10)  NOT NULL,
    [date_time_event]       DATETIME      DEFAULT (getdate()) NOT NULL,
    [file_attachments]      VARCHAR (500) NULL,
    PRIMARY KEY CLUSTERED ([id] ASC)
);

