using System;
using System.Collections.Generic;
using System.Text;

namespace ArkImportTools.Entities
{
    public class ClassListEntry
    {
        public string Name;
        public string DisplayName;
        public string Tooltip;
        public bool IsClassPlaceable;
        public ClassListEntry[] Children;

        public List<ClassListEntry> GetAllChildren()
        {
            List<ClassListEntry> oc = new List<ClassListEntry>();
            foreach (var c in Children)
            {
                oc.Add(c);
                oc.AddRange(c.GetAllChildren());
            }
            return oc;
        }

        public int GetAllChildrenCount()
        {
            int oc = 0;
            foreach (var c in Children)
            {
                oc += 1 + c.GetAllChildrenCount();
            }
            return oc;
        }

        public ClassListEntry SearchForClass(string name)
        {
            if (Name == name)
                return this;
            foreach (var c in Children)
            {
                ClassListEntry e = c.SearchForClass(name);
                if (e != null)
                    return e;
            }
            return null;
        }
    }
}
