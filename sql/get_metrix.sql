  SELECT count(*) as CountOf_Tokens
  FROM [crypta].[dbo].[EthTrainData]

  SELECT count(*) as CountOf_SwapEvents
  FROM [crypta].[dbo].[EthSwapEvents]

  SELECT max(blockNumberInt) as MaxBlockOf_SwapEvents
  FROM [crypta].[dbo].[EthSwapEvents]

  SELECT count(*) as CountOf_Not_Processed_Balance
  FROM [crypta].[dbo].[EthTrainData]
  where [BalanceOnCreating] = -1

  SELECT count(*) as CountOf_Not_Processed_PairAddress
  FROM [crypta].[dbo].[EthTrainData]
  where pairAddress = ''

  SELECT count(*) as CountOf_Not_Processed_ABI
  FROM [crypta].[dbo].[EthTrainData]
  where ABI is null

  SELECT count(*) as CountOf_AbsentABI
  FROM [crypta].[dbo].[EthTrainData]
  where ABI = 'no'

  SELECT count(*) as CountOf_Not_Processed_Wallets
  FROM [crypta].[dbo].[EthTrainData]
  where walletCreated = '0001-01-01 00:00:00.0000000'
  or  walletCreated = '0001-01-02 00:00:00.0000000'

  SELECT count(*) as tlgrm
  FROM [crypta].[dbo].[EthTrainData]
  where blockNumberInt > 20420936 
  and tlgrmNewTokens = 0
  and BalanceOnCreating > -1
  and walletCreated <> '0001-01-01 00:00:00.0000000'
  and walletCreated <> '0001-01-02 00:00:00.0000000'

  SELECT count(*) as CountOfProcessed_Sniffer
  FROM [crypta].[dbo].[EthTrainData]
  where tsFullResponse <> ''