use [FootbalLeague]
go

--select matches.*, hosts.team_name as [host], guests.team_name as [guest],
--case 
--	when matches.host_goals > matches.guest_goals then 3
--	when matches.host_goals = matches.guest_goals then 1
--	when matches.host_goals < matches.guest_goals then 0
--end as host_points
--,case 
--	when matches.guest_goals > matches.host_goals then 3
--	when matches.guest_goals = matches.host_goals then 1
--	when matches.guest_goals < matches.host_goals then 0
--end as guest_points
--from dbo.matches
--join teams as hosts on matches.host_team = hosts.team_id 
--join teams as guests on matches.guest_team = guests.team_id

select a.teamid, teams.team_name, sum(a.num_points) as points from
(
	select hosts.host_team as [teamid], 
	case 
		when hosts.host_goals > hosts.guest_goals then 3
		when hosts.host_goals = hosts.guest_goals then 1
		when hosts.host_goals < hosts.guest_goals then 0
	end as num_points
	from matches as hosts
	union
	select guests.guest_team as [teamid], 
	case 
		when guests.guest_goals > guests.host_goals then 3
		when guests.guest_goals = guests.host_goals then 1
		when guests.guest_goals < guests.host_goals then 0
	end as num_points
	from matches as guests
	union 
	select teams.team_id as [teamid], 0
	from teams 
	where 
	teams.team_id not in (select host_team from matches) and
	teams.team_id not in (select guest_team from matches) 

) as a
right join teams on a.teamid = teams.team_id
group by a.teamid, teams.team_name
order by points desc
