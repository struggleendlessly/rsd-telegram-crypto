namespace ethCommonDB.models
{
    public class TriggerHistory
    {
        public int Id { get; set; }
        public int blockNumberStartInt { get; set; }
        public int blockNumberEndInt { get; set; }

        public string title { get; set; }
    }
}
