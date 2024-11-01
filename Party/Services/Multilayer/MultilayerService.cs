using Party.Data;
using Party.Models;
using Party.Services.Multilayer;

namespace Party.Services
{
    public class MultilayerService
    {
        LinkedList<UserAccount> linkedList = new LinkedList<UserAccount>();
        public MultilayerService() {        
        
        }
        public void AddNodeWithClosure(ApplicationDbContext context, string name, int? parentId)
        {
            var newNode = new Node { Name = name };
            context.Nodes.Add(newNode);
            context.SaveChanges(); // 保存節點以獲取其 ID

            if (parentId.HasValue)
            {
                // 獲取父節點的所有祖先
                var ancestors = context.Closures
                    .Where(c => c.Descendant == parentId.Value)
                    .ToList();

                // 插入新的閉包條目
                foreach (var ancestor in ancestors)
                {
                    context.Closures.Add(new Closure
                    {
                        Ancestor = ancestor.Ancestor,
                        Descendant = newNode.Id,
                        Depth = ancestor.Depth + 1
                    });
                }

                // 還需要插入父節點與新節點的關係
                context.Closures.Add(new Closure
                {
                    Ancestor = parentId.Value,
                    Descendant = newNode.Id,
                    Depth = 1
                });
            }

            // 將新節點作為根節點的閉包條目
            context.Closures.Add(new Closure
            {
                Ancestor = newNode.Id,
                Descendant = newNode.Id,
                Depth = 0
            });

            context.SaveChanges();
        }
        public List<Node> GetPathToRootUsingClosure(ApplicationDbContext context, int nodeId)
        {
            var ancestors = context.Closures
                .Where(c => c.Descendant == nodeId)
                .OrderBy(c => c.Depth) // 根據深度排序
                .Select(c => c.Ancestor)
                .ToList();

            return context.Nodes.Where(n => ancestors.Contains(n.Id)).ToList();
        }

    }

}
