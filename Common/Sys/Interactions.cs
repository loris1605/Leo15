namespace Common.Sys
{
    public static class Interactions
    {
        public static void FireAndForget(this Task task)
        {
            task.ContinueWith(t => {
                if (t.IsFaulted) { /* Logga l'errore qui */ }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
