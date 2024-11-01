namespace Party.Services.Multilayer
{
    public class Node
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }

        // 對應到自我關聯的導覽屬性
        public virtual Node Parent { get; set; }
        public virtual ICollection<Node> Children { get; set; } = new List<Node>();
    }

}
