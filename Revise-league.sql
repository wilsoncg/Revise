SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

USE [master];
GO

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'FootbalLeague')
begin
	ALTER DATABASE FootbalLeague
	SET SINGLE_USER WITH
	ROLLBACK AFTER 5 --this will give your current connections 5 seconds to complete

	DROP DATABASE FootbalLeague
end

-- Create the FootbalLeague database.
CREATE DATABASE FootbalLeague;
GO

-- Specify a simple recovery model 
-- to keep the log growth to a minimum.
ALTER DATABASE FootbalLeague 
SET RECOVERY SIMPLE;
GO

USE FootbalLeague;
GO

CREATE TABLE [dbo].[teams] (
  [team_id]			INT				NOT NULL,
  [team_name]		varchar(30)		NOT NULL
  PRIMARY KEY CLUSTERED ([team_id] ASC)
);

CREATE TABLE [dbo].[matches] (
  [match_id]		INT		NOT NULL,
  [host_team]		int		NOT NULL,
  [guest_team]		int		NOT NULL,
  [host_goals]		int		NOT NULL,
  [guest_goals]		int		NOT NULL
  PRIMARY KEY CLUSTERED ([match_id] ASC)
);

insert into teams values(10, 'Give')
insert into teams values(20, 'Never')
insert into teams values(30, 'You')
insert into teams values(40, 'Up')
insert into teams values(50, 'Gonna')

insert into matches values(1, 30, 20, 1, 0)
insert into matches values(2, 10, 20, 1, 2)
insert into matches values(3, 20, 50, 2, 2)
insert into matches values(4, 10, 30, 1, 0)
insert into matches values(5, 30, 50, 0, 1)

-- Expected
--
-- team_id	| teaam_name | num_points
-- 20		| Never		 | 4 
-- 50		| Gonna		 | 4 
-- 10		| Give		 | 3 
-- 30		| You		 | 3 
-- 40		| Up		 | 0 