using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuplicateFinder
{
    class Cluster
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
            foreach(Record r in cluster.records)
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
