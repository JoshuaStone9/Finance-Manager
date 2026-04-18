using System.ComponentModel;  // BindingList

namespace FM.Data
{
    public static class BillStore
    {
        public static readonly BindingList<BillRecord> Bills = new BindingList<BillRecord>();
    }
}
