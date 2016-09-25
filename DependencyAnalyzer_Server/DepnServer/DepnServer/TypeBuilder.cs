using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepnServer
{
   /* public class TypeElem
    {
        public string namespaceName { get; set; }
        public string typeType { get; set; }

    } */
    class TypeBuilder
    {

        Repository rep = Repository.getInstance();

        public void addTypes()
        {
            
            List<Elem> table = rep.locations;

            List<Elem> temp = rep.locations;

            //Build the type table
            foreach (Elem e in table)
            {
                if (!(e.type.Equals("namespace") || e.type.Equals("function") || e.type.Equals("braceless") || e.type.Equals("control")))
                {
                    foreach (Elem e1 in temp)
                    {
                        if (e1.type.Equals("namespace"))
                        {
                            if ((e1.begin < e.begin) && (e1.end > e.end))
                            {
                                add(e.name, e1.name, e.type);
                            }
                        }
                    }
                }
            }


        }

        //function to add into the type table
        public void add(string typeName, string NameSpaceName, string type)
        {
            TypeElem insert = new TypeElem();
            insert.namespaceName = NameSpaceName;
            insert.typeType = type;
            rep.typeTable[typeName] = insert;

        }
    }
}
