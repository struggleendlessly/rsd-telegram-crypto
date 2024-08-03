  SELECT count(*) as allCount
  FROM [crypta].[dbo].[EthTrainData]

  SELECT count(*) as [BalanceOnCreating]
  FROM [crypta].[dbo].[EthTrainData]
  where [BalanceOnCreating] = -1


  SELECT count(*) as pairAddress
  FROM [crypta].[dbo].[EthTrainData]
  where pairAddress = ''

  SELECT count(*) as ABI
  FROM [crypta].[dbo].[EthTrainData]
  where ABI is null

  SELECT count(*) as walletCreated
  FROM [crypta].[dbo].[EthTrainData]
  where walletCreated = '0001-01-01 00:00:00.0000000'
  and  walletCreated = '0001-01-02 00:00:00.0000000'

  SELECT count(*) as tlgrm
  FROM [crypta].[dbo].[EthTrainData]
  where blockNumberInt > 20420936 
  and tlgrmNewTokens = 0
  and BalanceOnCreating > 0
  and walletCreated <> '0001-01-01 00:00:00.0000000'
  and walletCreated <> '0001-01-02 00:00:00.0000000'