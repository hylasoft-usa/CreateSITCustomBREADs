select
	CAST(M.MSG_BODY as xml),
	*
from
	DIS_MESSAGES M with(nolock)
	left join DIS_MESSAGES_SCHEDULER S with(nolock) on M.MSG_PK = S.MSG_PK
where
	M.MSG_TYPE = 'ERPOrderT'
order by
	M.MSG_TS desc

--delete from DIS_MESSAGES where MSG_PK > 0
select * from DIS_MESSAGES_STATUS