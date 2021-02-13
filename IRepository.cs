using FarmacyControl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FarmacyControl
{
    public interface IRepository
    {
        public bool GetMissedOrders();
        public List<Mrc> GetMrc();
        public void UpdateDb(Mrc mrcs);
        public void InsertDb(Mrc mrcs);
        public string  GetSomeTestData();

    }
}
