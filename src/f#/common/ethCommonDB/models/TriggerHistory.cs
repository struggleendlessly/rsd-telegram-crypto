namespace ethCommonDB.models
{
    public class TriggerHistory
    {
        public int Id { get; set; }
        public int blockNumberStartInt { get; set; }
        public int blockNumberEndInt { get; set; }

        public string title { get; set; }

        public static TriggerHistory Default()
        {
            var res = new TriggerHistory();
            res.blockNumberEndInt = 0;

            return res;
        }
    }
}
