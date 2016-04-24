using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            NameParser parser = new NameParser();
            String[] name = parser.parseName("Michael B. Jordan");
            for(int i =0; i<3; i++)
            {
                if(i==0) Console.Out.Write("Last Name:");
                else if (i == 1) Console.Out.Write("First Name:");
                else if (i == 2) Console.Out.Write("Middle Name:");
                Console.Out.WriteLine(name[i]);
            }
            Console.Read();
        }
    }
}
