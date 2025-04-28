namespace rest_api
{
    /*
     * Model of task. Description can be null
     */
    public class Task
    {
        public int Id { get; set; }
        public DateTime expiry {  get; set; }
        public string title { get; set; }
        public string? description { get; set; }
        public int completePercentage { get; set; }
    }
}
    