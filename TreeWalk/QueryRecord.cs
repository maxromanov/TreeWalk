namespace TreeWalk
{
    public class QueryRecord
    {
        internal bool query_result = true;

        public string query { get; set; }
        public QueryType query_type { get; set; }
        public string script { get; set; }
        public string name { get; internal set; }
    }
}