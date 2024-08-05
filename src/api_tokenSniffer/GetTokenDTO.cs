namespace api_tokenSniffer
{
    public class GetTokenDTO
    {
        public string message { get; set; } = default!;
        public string status { get; set; } = default!;
        public string chainId { get; set; } = default!;
        public string address { get; set; } = default!;
        public long refreshed_at { get; set; } = default;
        public string name { get; set; } = default!;
        public string symbol { get; set; } = default!;
        public long total_supply { get; set; } = default;
        public int decimals { get; set; } = default;
        public long created_at { get; set; } = default;
        public string deployer_addr { get; set; } = default!;
        public bool is_flagged { get; set; } = default;
        public string[] exploits { get; set; } = default!;
        public Contract contract { get; set; } = default!;
        public int score { get; set; } = default;
        public string riskLevel { get; set; } = default!;
        public Permissions permissions { get; set; } = default!;
        public Swap_Simulation swap_simulation { get; set; } = default!;
        public Balances balances { get; set; } = default!;
        public Pool[] pools { get; set; } = default!;
    }

    public class Contract
    {
        public bool is_source_verified { get; set; } = default;
        public bool has_mint { get; set; } = default;
        public bool has_fee_modifier { get; set; } = default;
        public bool has_max_transaction_amount { get; set; } = default;
        public bool has_blocklist { get; set; } = default;
        public bool has_proxy { get; set; } = default;
        public bool has_pausable { get; set; } = default;
    }

    public class Permissions
    {
        public string owner_address { get; set; } = default!;
        public bool is_ownership_renounced { get; set; } = default;
    }

    public class Swap_Simulation
    {
        public bool is_sellable { get; set; } = default;
        public int buy_fee { get; set; } = default;
        public int sell_fee { get; set; } = default;
    }

    public class Balances
    {
        public int burn_balance { get; set; } = default;
        public int lock_balance { get; set; } = default;
        public float deployer_balance { get; set; } = default;
        public float owner_balance { get; set; } = default;
        public Top_Holders[] top_holders { get; set; } = default!;
    }

    public class Top_Holders
    {
        public string address { get; set; } = default!;
        public float balance { get; set; } = default;
        public bool is_contract { get; set; } = default;
    }

    public class Pool
    {
        public string address { get; set; } = default!;
        public string name { get; set; } = default!;
        public string version { get; set; } = default!;
        public string base_symbol { get; set; } = default!;
        public string base_address { get; set; } = default!;
        public int total_supply { get; set; } = default;
        public int decimals { get; set; } = default;
        public float base_reserve { get; set; } = default;
        public float initial_base_reserve { get; set; } = default;
        public int owner_balance { get; set; } = default;
        public int deployer_balance { get; set; } = default;
        public int burn_balance { get; set; } = default;
        public int lock_balance { get; set; } = default;
        public object[] top_holders { get; set; } = default!;
        public object[] locks { get; set; } = default!;
    }
}
