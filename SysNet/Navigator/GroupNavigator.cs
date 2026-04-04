namespace SysNet
{
    public static class GroupNavigator
    {
        public static readonly IDictionary<string, List<Action<object>>> pl_dict = new Dictionary<string, List<Action<object>>>();


        public static void Register(string token, Action<object> callback)
        {
            if (!pl_dict.TryGetValue(token, out List<Action<object>> value))
            {
                List<Action<object>> list =
                [
                    callback
                ];
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

        public static void Unregister(string token, Action<object> callback)
        {
            if (pl_dict.TryGetValue(token, out List<Action<object>> value))
                value.Remove(callback);
        }

        public static void NotifyColleagues(string token, object args = null)
        {

            if (pl_dict.TryGetValue(token, out List<Action<object>> value))
                foreach (Action<object> callback in value)
                    callback(args);
        }

        public static void Clear()
        {
            pl_dict.Clear();
        }

        public static int Count => pl_dict.Count;
    }
}
