SELECT count(*) as eth
  FROM [crypta_eth].[dbo].[swapsETH_TokenEntities]

SELECT count(*) as usd
  FROM [crypta_eth].[dbo].swapsETH_USDEntities

 SELECT count(*) as blocks
  FROM [crypta_eth].[dbo].blocksEntities

 SELECT count(*) as info
  FROM [crypta_eth].[dbo].tokenInfoEntities

 SELECT count(*) as triger
  FROM [crypta_eth].[dbo].triggerHistoriesEntities