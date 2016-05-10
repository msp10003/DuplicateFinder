using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    public class Cluster
    {
        private List<Record> records, subset;
        private String clusterName { get; set; }
        private int clusterNum { get; set; }

        public Cluster(Record r)
        {
            records = new List<Record>();
            records.Add(r);
            clusterNum = r.getID();
            clusterName = "C" + r.getID();
            subset = new List<Record>();
        }

        public void Add(Record r)
        {
            records.Add(r);
        }

        public void merge(Cluster cluster)
        {
            //TODO do this more efficiently, this is horrible, ruins the benefits of union-find
            List<Record> origRecords = cluster.getRecords();

            foreach(Record r in origRecords)
            {
                r.setCluster(this);
                this.records.Add(r);
            }

            clusterName = "C"+records[0].getID();
            clusterNum = records[0].getID();
        }

        public List<Record> getRecords()
        {
            return records;
        }

        public Record getRepresentativeElement()
        {
            return records[0];
        }

        public override string ToString()
        {
            string s = "";
            s = clusterName + " | ";
            foreach(Record r in records)
            {
                s += r.getFullName() + ";";
            }
            s += "\n----------------------------------------\n";
            return s;
        }
    }
}
