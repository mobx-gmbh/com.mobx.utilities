using System;
using System.Threading;

namespace MobX.Utilities.Task
{
    public static class TaskUtility
    {
        public static async System.Threading.Tasks.Task WaitWhile(Func<bool> condition, CancellationToken cancellationToken = new())
        {
            while (condition())
            {
                await System.Threading.Tasks.Task.Delay(25, cancellationToken);
            }
        }

        public static async System.Threading.Tasks.Task WaitUntil(Func<bool> condition, CancellationToken cancellationToken = new())
        {
            while (!condition())
            {
                await System.Threading.Tasks.Task.Delay(25, cancellationToken);
            }
        }
    }
}
