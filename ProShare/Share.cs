using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProShare
{
    public class Share
    {
        private Tuple<Field, Field> t;

        public Share()
        {
            Field x = new Field(0);
            Field y = new Field(0);
            t = new Tuple<Field, Field>(x, y);
        }

        public Share(Share S)
        {
            t = new Tuple<Field, Field>(S.GetX(), S.GetY());
        }

        public Share(Field x, Field y)
        {
            t = new Tuple<Field, Field>(x, y);
        }

        //getters and setter
        public Field GetX()
        {
            return t.Item1;
        }

        public Field GetY()
        {
            return t.Item2;
        }

        public void Set(Field x, Field y)
        {
            t = new Tuple<Field, Field>(x, y);
        }
    }
}
