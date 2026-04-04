namespace MvvmNet
{
    public static class CassaNavigator
    {
        //public static readonly IDictionary<string, List<Action<object>>> pl_dict = new Dictionary<string, List<Action<object>>>();
        static readonly IDictionary<string, List<Action<object, int>>> pl_dict = new Dictionary<string, List<Action<object, int>>>();

        static public void Register(string token, Action<object, int> callback)
        {
            if (!pl_dict.TryGetValue(token, out List<Action<object, int>> value))
            {
                var list = new List<Action<object, int>>
                {
                    callback
                };
                pl_dict.Add(token, list);
            }
            else
            {
                bool found = false;
                foreach (var item in value)
                    if (item.Method.ToString() == callback.Method.ToString())
                        found = true;
                if (!found)
                    value.Add(callback);
            }
        }

        static public void Unregister(string token, Action<object, int> callback)
        {
            if (pl_dict.TryGetValue(token, out List<Action<object, int>> value))
                value.Remove(callback);
        }

        static public void NotifyColleagues(string token, int param = 0, object args = null)
        {
            if (pl_dict.TryGetValue(token, out List<Action<object, int>> value))
                foreach (var callback in value)
                    callback(args, param);
        }

        public static void Clear()
        {
            pl_dict.Clear();
        }

        public static int Count => pl_dict.Count;
    }
}
