using System.Threading.Tasks;

namespace DogeChat.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Task"/> and <see cref="Task{T}"/>.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Awaits the given <paramref name="task"/> while catching and ignoring all exceptions.
        /// </summary>
        /// <param name="task">The <see cref="Task"/> to be awaited.</param>
        public static void WaitSafely(this Task task)
        {
            try
            {
                task.Wait();
            }
            catch
            {
                // Ignored.
            }
        }

        /// <summary>
        /// Awaits the given <paramref name="task"/> while catching and ignoring all exceptions.
        /// </summary>
        /// <param name="task">The <see cref="Task"/> to be awaited.</param>
        /// <returns>An awaitable <see cref="Task"/> that will always complete successfully.</returns>
        public static async Task AwaitSafely(this Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch
            {
                // Ignored.
            }
        }
    }
}
