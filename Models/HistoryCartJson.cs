namespace WEB_Shop_Ajax.Models
{
    public class HistoryCartJson
    {

        public List<Historyall> History { get; set; }
    }
    
    public class Historyall
    {
        public Info info { get; set; }
    }

    public class Info
    {
        public string id { get; set; }
        public int count { get; set; }
    }

}
