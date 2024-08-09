namespace api_tokenSniffer
{
    public class GetTokenDTO
    {
        public string message { get; set; }
        public string status { get; set; }
        public string chainId { get; set; }
        public string address { get; set; }
        public long refreshed_at { get; set; }
        public string name { get; set; }
        public string symbol { get; set; }
        public long total_supply { get; set; }
        public int decimals { get; set; }
        public long created_at { get; set; }
        public string deployer_addr { get; set; }
        public bool is_flagged { get; set; }
        public string[] exploits { get; set; }
        public Contract contract { get; set; }
        public int score { get; set; }
        public string riskLevel { get; set; }
        public Test[] tests { get; set; }
        public Permissions permissions { get; set; }
        public Swap_Simulation swap_simulation { get; set; }
        public Balances balances { get; set; }
        public Pool[] pools { get; set; }
        public Similar[] similar { get; set; }
    }

    public class Contract
    {
        public bool is_source_verified { get; set; }
        public bool has_fee_modifier { get; set; }
        public bool has_max_transaction_amount { get; set; }
        public bool has_blocklist { get; set; }
        public bool has_proxy { get; set; }
        public bool has_pausable { get; set; }
    }

    public class Permissions
    {
        public string owner_address { get; set; }
        public bool is_ownership_renounced { get; set; }
    }

    public class Swap_Simulation
    {
        public bool is_sellable { get; set; }
        public int buy_fee { get; set; }
        public int sell_fee { get; set; }
    }

    public class Balances
    {
        public double burn_balance { get; set; }
        public double lock_balance { get; set; }
        public double deployer_balance { get; set; }
        public double owner_balance { get; set; }
        public Top_Holders[] top_holders { get; set; }
    }

    public class Top_Holders
    {
        public string address { get; set; }
        public double balance { get; set; }
        public bool is_contract { get; set; }
    }

    public class Test
    {
        public string id { get; set; }
        public string description { get; set; }
        public bool result { get; set; }
        public float value { get; set; }
        public float valuePct { get; set; }
        public Datum[] data { get; set; }
        public string currency { get; set; }
    }

    public class Datum
    {
        public string address { get; set; }
        public double balance { get; set; }
        public bool is_contract { get; set; }
    }

    public class Pool
    {
        public string address { get; set; }
        public string name { get; set; }
        public string version { get; set; }
        public string base_symbol { get; set; }
        public string base_address { get; set; }
        public long total_supply { get; set; }
        public int decimals { get; set; }
        public float base_reserve { get; set; }
        public float initial_base_reserve { get; set; }
        public double owner_balance { get; set; }
        public double deployer_balance { get; set; }
        public double burn_balance { get; set; }
        public double lock_balance { get; set; }
        public object[] top_holders { get; set; }
        public object[] locks { get; set; }
    }

    public class Similar
    {
        public string chainId { get; set; }
        public string address { get; set; }
        public int stcore { get; set; }
    }
}
